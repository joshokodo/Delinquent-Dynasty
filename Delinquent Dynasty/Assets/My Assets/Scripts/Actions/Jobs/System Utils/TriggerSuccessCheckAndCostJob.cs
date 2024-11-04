using System;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TriggerSuccessCheckAndCostJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;

    public DynamicActionType DynamicActionType;
    public int NextPhase;
    public EntityCommandBuffer Ecb;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;

    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions){
        bool isSuccessful;

        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore,
        };

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        var passiveUtils = new PassiveEffectsUtils(){
            Skills = SkillsLookup[comp.CharacterEntity],
            CharacterAttributes = AttributesLookup[comp.CharacterEntity],
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            SkillUtils = skillUtils
        };

        if (actionUtils.TryGetActiveAction(DynamicActionType, actions, out ActiveActionElement act, out int index)){
            var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);


            if (actionData.SkillUsed == SkillType.COMMON){
                isSuccessful = true;
            }
            else{
                var chance = passiveUtils.GetSkillSuccessChance(actionData.SkillUsed,
                    actionData.DifficultyLevel);
                isSuccessful = Random.ValueRW.IsSuccessful(chance);
            }

            actionUtils.SetCostEffects(e, passiveUtils, actionData, ActiveEffectSpawnSpawn);

            if (isSuccessful){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
            else{
                actionUtils.CompleteAction(act, actions, index);
            }
        }
    }
}

public struct TriggerSuccessCheckAndCostUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public RefRW<RandomComponent> Random;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnElements;

    public EntityQuery Query;

    private RefRW<RandomComponent> _randomComponent;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionComponentFunctionPerformQuery<T>());

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();

        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
    }

    public TriggerSuccessCheckAndCostJob GetActionSuccessCheckAndPayCostJob(int nextPhase){
        return new TriggerSuccessCheckAndCostJob(){
            Random = Random,
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            SkillDataStore = SkillDataStore,
            PassiveCompLookup = _passiveCompLookup,
            NextPhase = nextPhase,
            ActiveEffectSpawnSpawn = ActiveEffectSpawnElements,
            Ecb = Ecb,
            AttributesLookup = _attributesLookup,
            PassivesLookup = _passivesLookup,
            SkillsLookup = _skillsLookup
        };
    }
}