
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct PerformElectronicRequestJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public double TotalInGameSeconds;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<RequestComponent> RequestLookup;
    [ReadOnly] public ComponentLookup<ItemCellPhoneComponent> CellLookup;
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;

    public DynamicBuffer<PassiveEffectSpawnElement> PassiveSpawn;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;

    public EntityCommandBuffer Ecb;
    public bool TriggerPassivesAfterPerformance;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        var passiveEffectsUtils = new PassiveEffectsUtils(){
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            Skills = SkillsLookup[comp.CharacterEntity]
        };

        var endPhase = false;
        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var actData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);
            var performTime = passiveEffectsUtils.GetNaturalAndBonusPerformTime(actData.PerformTime,
                actData.SkillUsed);

            var originItem = actionUtils.GetTargetEntity(TargetType.TARGET_ITEM, activeTargets);

            Entity targ = default;
            Entity targElectronic = default;
            ItemCellPhoneComponent targCell = default;
            RequestType requestType = RequestType.NONE;
            switch (DynamicActionType.Category){
                case ActionCategory.COMMON_ITEMS:
                    switch (DynamicActionType.CommonItemActionType){
                        case CommonItemActionType.CALL_PHONE:
                            var phone = actionUtils.GetTargetEntity(TargetType.TARGET_PHONE, activeTargets);
                            targElectronic = phone;
                            targCell = CellLookup[phone];
                            targ = targCell.Owner;
                            requestType = RequestType.PHONE_CALL;
                            break;
                    }
                    break;
            }

            if (targElectronic != Entity.Null){
                if (RequestLookup.TryGetComponent(comp.CharacterEntity, out RequestComponent requestComp)){
                    if (requestComp.ChoiceType == YesNoChoiceType.NO || TotalInGameSeconds >= requestComp.ExpireTime){
                        endPhase = true;
                    } else if (requestComp.ChoiceType == YesNoChoiceType.YES){
                        endPhase = true;
                        switch (requestType){
                            case RequestType.PHONE_CALL:
                                var originCell = CellLookup[originItem];
                                originCell.PhoneCallWith = targElectronic;
                                if (targCell.PhoneCallWith != Entity.Null){
                                    var other = CellLookup[targCell.PhoneCallWith];
                                    other.PhoneCallWith = Entity.Null;
                                    Ecb.SetComponent(targCell.PhoneCallWith, other);
                                }
                                targCell.PhoneCallWith = originItem;
                                Ecb.SetComponent(targElectronic, targCell);
                                Ecb.SetComponent(originItem, originCell);
                                break;
                        }
                    }

                    if (endPhase){
                        Ecb.RemoveComponent<RequestComponent>(comp.CharacterEntity);
                    }
                } else {
                    requestComp = new RequestComponent(){
                        OriginAction = activeAction.ActionId,
                        RequestOrigin = originItem,
                        RequestTarget = targElectronic,
                        ExpireTime = TotalInGameSeconds + performTime,
                        StartTime = TotalInGameSeconds,
                        RequestType = requestType
                    };
                    
                    Ecb.AddComponent(comp.CharacterEntity, requestComp);
                }

                if (!endPhase){
                    var skillLevel = passiveEffectsUtils.GetNaturalAndBonusSkillLevel(actData.SkillUsed);
                    
                    if (!TriggerPassivesAfterPerformance && !activeAction.HasStarted){
                        ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e,
                            comp.CharacterEntity, targ, actData, skillLevel,
                            PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
                    }

                    if (actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, performTime, Ecb, e,
                            PhaseAfterComplete)){
                        if (TriggerPassivesAfterPerformance){
                            ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e,
                                comp.CharacterEntity, targ, actData, skillLevel,
                                PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
                        }
                        passiveEffectsUtils.TriggerOnActionPerformEffects(comp.CharacterEntity, ActiveEffectSpawnSpawn,
                            actData.SkillUsed, actData.ActionType, comp.CharacterEntity);
                    }
            
            

                    // if (target != Entity.Null){
                    //     StateChangeSpawn.Add(new CharacterStateChangeSpawnElement(){
                    //         Character = target,
                    //         IsTargetOfExternalEvent = true,
                    //     });
                    // }
                }
            } 
        }
        
        if (endPhase){
            if (PhaseAfterComplete < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(activeAction, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, PhaseAfterComplete);
            }
        }
    }
}

public struct PerformElectronicRequestUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public EntityCommandBuffer Ecb;
    public InGameTime InGameTime;

    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveEffectsSpawn;

    public EntityQuery Query;

    private ComponentLookup<RequestComponent> _requestCompLookup;
    private ComponentLookup<ItemCellPhoneComponent> _cellLookup;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;
        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _cellLookup = state.GetComponentLookup<ItemCellPhoneComponent>();
        _requestCompLookup = state.GetComponentLookup<RequestComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _requestCompLookup.Update(ref state);
        _cellLookup.Update(ref state);
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
    }

    public PerformElectronicRequestJob GetPerformActionJob(int nextPhase, bool triggerPassivesAfterPerformance = false){
        return new PerformElectronicRequestJob(){
            Ecb = Ecb,
            TotalInGameSeconds = InGameTime.TotalInGameSeconds,
            DynamicActionType = ActionType,
            ActionDataStore = ActionDataStore,
            PhaseAfterComplete = nextPhase,
            PassiveCompLookup = _passiveCompLookup,
            PassivesLookup = _passivesLookup,
            StateChangeSpawn = StateChangeSpawn,
            ActiveEffectSpawnSpawn = ActiveEffectsSpawn,
            SkillsLookup = _skillsLookup,
            PassiveSpawn = PassiveEffectsSpawn,
            TriggerPassivesAfterPerformance = triggerPassivesAfterPerformance,
            RequestLookup = _requestCompLookup,
            CellLookup = _cellLookup,
        };
    }
}