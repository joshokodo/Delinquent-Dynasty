using System;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TriggerActionEffectsJob : IJobEntity {
// todo try to make this job parallel?
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;

    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<VisibleCharacterElement> VisionLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        bool isSuccessful;
        var vision = VisionLookup[comp.CharacterEntity];

        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore,
        };

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        
        var passiveUtils = new PassiveEffectsUtils(){
            CharacterAttributes = AttributesLookup[comp.CharacterEntity],
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            SkillUtils = skillUtils,
            Skills = SkillsLookup[comp.CharacterEntity],
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);


            if (actionData.SkillUsed == SkillType.COMMON){
                isSuccessful = true;
            }
            else{
                var chance = passiveUtils.GetSkillSuccessChance(actionData.SkillUsed,
                    actionData.DifficultyLevel);
                isSuccessful = Random.ValueRW.IsSuccessful(chance);
            }

            var skillLevel = passiveUtils.GetNaturalAndBonusSkillLevel(actionData.SkillUsed);

            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            var performActionKnowledge = new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = isSuccessful,
                TargetsData = targetData
            };

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

            actionUtils.SetEffectsForTarget(actionData, comp.CharacterEntity, targetData, isSuccessful, ActiveEffectSpawnSpawn, skillLevel);

            actionUtils.SetCostEffects(comp.CharacterEntity, passiveUtils, actionData, ActiveEffectSpawnSpawn);

            var learningTargets = new FixedList4096Bytes<Entity>();
            
            var targetCharacter = targetData.GetTargetEntity(TargetType.TARGET_CHARACTER);
            
            if (!actionData.HideKnowledgeFromTargets && targetCharacter != Entity.Null){
                learningTargets.Add(targetCharacter);
            }

            if (actionData.HasTargetType(TargetType.TARGET_VISIBLE_CHARACTERS_IN_AREA)){
                foreach (var visibleCharacterElement in vision){
                    var withinRange = !visibleCharacterElement.IsHidden &&
                                      visibleCharacterElement.Distance <= actionData.MaxAreaOfEffectRange;
                    if (withinRange) {
                        
                        targetData.OverwriteSingleTarget(TargetType.TARGET_CHARACTER,
                            visibleCharacterElement.VisibleCharacter);
                        
                        actionUtils.SetEffectsForTarget(actionData, comp.CharacterEntity, targetData, isSuccessful, ActiveEffectSpawnSpawn, skillLevel);

                        if (!actionData.HideKnowledgeFromTargets && targetCharacter != Entity.Null){
                            learningTargets.Add(visibleCharacterElement.VisibleCharacter);
                        }
                    }
                }
            }

            performActionKnowledge.TargetCharacters = learningTargets;

            ActionKnowledgeSpawnElements.Add(performActionKnowledge);

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

public struct TriggerActionEffectsUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public RefRW<RandomComponent> Random;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;

    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;
    private BufferLookup<VisibleCharacterElement> _visionLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _visionLookup = state.GetBufferLookup<VisibleCharacterElement>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _visionLookup.Update(ref state);
        _skillsLookup.Update(ref state);
    }

    public TriggerActionEffectsJob GetTriggerActionEffectsJob(int nextPhase){
        return new TriggerActionEffectsJob(){
            Random = Random,
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            SkillDataStore = SkillDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            ActiveEffectSpawnSpawn = ActiveEffectSpawnElements,
            PassiveCompLookup = _passiveCompLookup,
            StateChangeSpawnElements = StateChangeSpawnElements,
            PassivesLookup = _passivesLookup,
            NextPhase = nextPhase,
            AttributesLookup = _attributesLookup,
            SkillsLookup = _skillsLookup,
            Ecb = Ecb,
            VisionLookup = _visionLookup,
        };
    }
}