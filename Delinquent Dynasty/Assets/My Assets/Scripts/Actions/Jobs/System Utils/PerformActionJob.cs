using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
// TODO: important!!!!!!!! bulk of action time takes place here. making this parallel should improve performance greatly!!
public partial struct PerformActionJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public double TotalInGameSeconds;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveSpawn;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;
    
    public EntityCommandBuffer Ecb;
    
    public bool TriggerPassivesAfterPerformance; // todo double check hard bc think we have to do it like this for a specific reason but if not, split back out into separate job/utils that we can reuse. im pretty sure we were doing that before but for some reason I did this. do for the other perform job/utils if we do it here

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions, DynamicBuffer<ActiveActionTargetElement> targets){
        
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        var passiveEffectsUtils = new PassiveEffectsUtils(){
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            Skills = SkillsLookup[comp.CharacterEntity]
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(DynamicActionType, actions, targets,
                TargetType.TARGET_CHARACTER, out ActiveActionElement activeAction, out Entity target, out int index)){
         
            var actData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);
            var performTime = passiveEffectsUtils.GetNaturalAndBonusPerformTime(actData.PerformTime,
                actData.SkillUsed);

            var skillLevel = passiveEffectsUtils.GetNaturalAndBonusSkillLevel(actData.SkillUsed);

            if (!TriggerPassivesAfterPerformance && !activeAction.HasStarted){
                
                ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e, comp.CharacterEntity, target, actData, skillLevel,
                    PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
            }
            
            if (actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, performTime, Ecb, e,
                    PhaseAfterComplete, out _)){
                
                if (TriggerPassivesAfterPerformance){
                    ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e, comp.CharacterEntity, target, actData, skillLevel,
                        PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
                }
                passiveEffectsUtils.TriggerOnActionPerformEffects(comp.CharacterEntity, ActiveEffectSpawnSpawn, actData.SkillUsed, actData.ActionType, comp.CharacterEntity);
            }

            if (target != Entity.Null){
                StateChangeSpawn.Add(new CharacterStateChangeSpawnElement(){
                    Character = target,
                    IsTargetOfExternalEvent = true,
                });
            }
        }
    }
}

public struct PerformActionUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public EntityCommandBuffer Ecb;
    public InGameTime InGameTime;

    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveEffectsSpawn;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;
        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
    }

    public PerformActionJob GetPerformActionJob(int nextPhase, bool triggerPassivesAfterPerformance = false){
        return new PerformActionJob(){
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
            TriggerPassivesAfterPerformance = triggerPassivesAfterPerformance
        };
    }
}