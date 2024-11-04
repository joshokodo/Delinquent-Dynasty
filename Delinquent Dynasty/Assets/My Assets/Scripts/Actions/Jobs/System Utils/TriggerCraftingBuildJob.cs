using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[BurstCompile]
public partial struct TriggerCraftingBuildJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;

    [ReadOnly] public DynamicActionType DynamicActionType;

    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassiveLookup;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;

    public EntityCommandBuffer Ecb;
    public ComponentLookup<ItemBuildInProgressComponent> BuildInProgressLookup;
    public ComponentLookup<ItemDurabilityComponent> DurablitiyLookup;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions
        , DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore,
        };

        var passiveUtils = new PassiveEffectsUtils(){
            Passives = PassiveLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            SkillUtils = skillUtils,
            Skills = SkillsLookup[comp.CharacterEntity],
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

            var targetBuildInProgress = actionUtils.GetTargetEntity(TargetType.TARGET_BUILD_IN_PROGRESS, activeTargets);

            if (BuildInProgressLookup.TryGetComponent(targetBuildInProgress,
                    out ItemBuildInProgressComponent buildComp)){
                bool isSuccessful;
                if (actionData.SkillUsed == SkillType.COMMON){
                    isSuccessful = true;
                }
                else{
                    var chance = passiveUtils.GetSkillSuccessChance(actionData.SkillUsed,
                        actionData.DifficultyLevel);
                    isSuccessful = Random.ValueRW.IsSuccessful(chance);
                }

                var targetData = new TargetsGroup();
                targetData.SetTargets(comp.CharacterEntity, activeTargets);

                var skillLevel = passiveUtils.GetNaturalAndBonusSkillLevel(actionData.SkillUsed);
                actionUtils.SetCostEffects(comp.CharacterEntity, passiveUtils, actionData, ActiveEffectsSpawnElements);

                buildComp = actionUtils.AffectCraftingBuildInProgress(Ecb, actionData, comp.CharacterEntity, targetData, isSuccessful,
                    ActiveEffectsSpawnElements, skillLevel, buildComp, DurablitiyLookup, Random);
                if (buildComp.DefectPercentage >= 100){
                    Ecb.AddComponent(targetBuildInProgress, new TransformItemComponent(){
                        ItemType = buildComp.FailedProduct,
                        ActualInventorySource = comp.CharacterEntity // todo not right could be interacting inventory technically
                    });
                }
                else if (buildComp.BuildPercentage >= 100){
                    Ecb.AddComponent(targetBuildInProgress, new TransformItemComponent(){
                        ItemType = buildComp.SuccessfulProduct,
                        ActualInventorySource = comp.CharacterEntity, // todo not right could be interacting inventory technically,
                        CraftedQualityType = new CraftingUtils().GetFinalQualityType(buildComp.QualityPercentage,
                            buildComp.DefectPercentage)
                    });
                }
                else{
                    Ecb.SetComponent(targetBuildInProgress, buildComp);
                }

                ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                    PerformingCharacter = comp.CharacterEntity,
                    DynamicActionType = DynamicActionType,
                    IsSuccessful = true,
                    TargetsData = targetData
                });

                var newState = new CharacterStateChangeSpawnElement(){
                    Character = comp.CharacterEntity,
                    InventoryChanged = true,
                };

                if (isSuccessful){
                    newState.PerformedSuccessfulAction = true;
                }
                else{
                    newState.PerformedFailedAction = true;
                }

                StateChangeSpawnElements.Add(newState);
            }

            if (NextPhase < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(act, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
        }
    }

    
}

public struct TriggerCraftingBuildUtil {
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;

    public EntityCommandBuffer Ecb;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;
    private DynamicActionType _actionType;
    public RefRW<RandomComponent> Random;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;

    private ComponentLookup<ItemBuildInProgressComponent> _itemBuildInProgressLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<SkillElement> _skillsLookup;


    private ComponentLookup<ItemDurabilityComponent> _durabilityLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        _actionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _itemBuildInProgressLookup = state.GetComponentLookup<ItemBuildInProgressComponent>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();

        _durabilityLookup = state.GetComponentLookup<ItemDurabilityComponent>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, _actionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _itemBuildInProgressLookup.Update(ref state);
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _durabilityLookup.Update(ref state);
    }

    public TriggerCraftingBuildJob GetTriggerCraftingBuildJob(int nextPhase){
        return new TriggerCraftingBuildJob(){
            ActionDataStore = ActionDataStore,
            DynamicActionType = _actionType,
            SkillDataStore = SkillDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            Ecb = Ecb,
            NextPhase = nextPhase,
            ActiveEffectsSpawnElements = ActiveEffectsSpawnElements,
            PassiveCompLookup = _passiveCompLookup,
            PassiveLookup = _passivesLookup,
            SkillsLookup = _skillsLookup,
            BuildInProgressLookup = _itemBuildInProgressLookup,
            Random = Random,
            DurablitiyLookup = _durabilityLookup,
            StateChangeSpawnElements = StateChangeSpawnElements
        };
    }
}