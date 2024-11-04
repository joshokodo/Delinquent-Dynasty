using System;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TriggerInfluencedActionEffectsJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;


    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<RelationshipElement> RelationshipsLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;

    public EntityCommandBuffer Ecb;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;

    public int NextPhase;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> RandomComponent;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        var skills = SkillsLookup[comp.CharacterEntity];

        var passiveUtils = new PassiveEffectsUtils(){
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            Skills = skills,
        };


        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

            var targetsData = new TargetsGroup();
            targetsData.SetTargets(comp.CharacterEntity, activeTargets);
            var targetCharacter = targetsData.GetTargetEntity(TargetType.TARGET_CHARACTER);

            actionUtils.ParseMainEnumTargets(activeTargets, out DynamicGameEnum primaryGameEnum,
                out DynamicGameEnum secondaryGameEnum, out DynamicGameEnum tertiaryGameEnum);

            var isSuccessful = false;

            if (targetCharacter != Entity.Null){
                var targetPassiveUtils = new PassiveEffectsUtils(){
                    Passives = PassivesLookup[targetCharacter],
                    PassiveCompLookup = PassiveCompLookup,
                };

                var chance = new RelationshipUtils()
                    .GetInfluenceSuccessChance(actionData, RelationshipsLookup, comp.CharacterEntity, targetCharacter, passiveUtils,
                        targetPassiveUtils);

                isSuccessful = RandomComponent.ValueRW.IsSuccessful(chance);

                var skillLevel = passiveUtils.GetNaturalAndBonusSkillLevel(actionData.SkillUsed);
                actionUtils.SetEffectsForTarget(actionData, comp.CharacterEntity, targetsData, isSuccessful, ActiveEffectSpawnSpawn,
                    primaryGameEnum, secondaryGameEnum, tertiaryGameEnum, skillLevel);
            }

            actionUtils.SetCostEffects(comp.CharacterEntity, passiveUtils, actionData, ActiveEffectSpawnSpawn);


            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = actionData.ActionType,
                IsSuccessful = isSuccessful,
                TargetsData = targetsData
            });

            var newState = new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
            };

            if (isSuccessful){
                newState.PerformedSuccessfulAction = true;
            }
            else{
                newState.PerformedFailedAction = true;
            }

            StateChangeSpawnElements.Add(newState);

            if (NextPhase < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(activeAction, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
        }
    }

}

public struct TriggerInfluencedActionEffectsUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public EntityCommandBuffer Ecb;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;

    public RefRW<RandomComponent> RandomComponent;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;

    private BufferLookup<PassiveEffectElement> _passivesLookup;

    private BufferLookup<RelationshipElement> _relationshipLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query = state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();


        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _relationshipLookup = state.GetBufferLookup<RelationshipElement>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _relationshipLookup.Update(ref state);
    }

    public TriggerInfluencedActionEffectsJob GetInfluencedTriggeredActionEffectsJob(int nextPhase){
        return new TriggerInfluencedActionEffectsJob(){
            Ecb = Ecb,
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            ActiveEffectSpawnSpawn = ActiveEffectSpawnElements,
            PassiveCompLookup = _passiveCompLookup,
            RelationshipsLookup = _relationshipLookup,
            PassivesLookup = _passivesLookup,
            NextPhase = nextPhase,
            StateChangeSpawnElements = StateChangeSpawnElements,
            RandomComponent = RandomComponent,
            SkillsLookup = _skillsLookup
        };
    }
}