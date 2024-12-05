using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

[BurstCompile]
public partial struct PerformRecordVideoJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;
    [ReadOnly] public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<ActiveActionTargetElement> TargetsLookup;
    [ReadOnly] public ComponentLookup<CharacterBehaviorComponent> BehaviorCompLookup;

    [ReadOnly] public InGameTime InGameTime;

    [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

    public BufferLookup<ActiveActionElement> ActionsLookup;
    public BufferLookup<VideoElement> VideoLookup;
    public BufferLookup<VideoFrameElement> VideoFrameLookup;

    public DynamicBuffer<VideoSpawnElement> VideoSpawn;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveSpawn;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;

    public EntityCommandBuffer Ecb;
    public bool TriggerPassivesAfterPerformance;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp){
        var actions = ActionsLookup[e];
        var targets = TargetsLookup[e];
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        
        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore
        };
        
        var passiveEffectsUtils = new PassiveEffectsUtils(){
            CharacterAttributes = AttributesLookup[comp.CharacterEntity],
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            Skills = SkillsLookup[comp.CharacterEntity],
            SkillUtils = skillUtils
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var activeTarget = actionUtils.GetTargetEntity(TargetType.TARGET_CHARACTER, activeTargets);
            var actData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);
            var performTime = passiveEffectsUtils.GetNaturalAndBonusPerformTime(actData.PerformTime,
                actData.SkillUsed);

            var skillLevel = passiveEffectsUtils.GetNaturalAndBonusSkillLevel(actData.SkillUsed);

            if (!TriggerPassivesAfterPerformance && !activeAction.HasStarted){
                ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e,
                    comp.CharacterEntity, activeTarget, actData, skillLevel,
                    PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
            }

            if (actionUtils.HandlePerformance(activeAction, actions, index, InGameTime.TotalInGameSeconds, performTime,
                    Ecb, e,
                    PhaseAfterComplete, out activeAction)){
                // todo when or how oftenn are we triggerinng passives after? might not need this, check in the future
                if (TriggerPassivesAfterPerformance){
                    ActionDataStore.ActionsBlobAssets.Value.SetActionPassives(activeAction.ActionId, e,
                        comp.CharacterEntity, activeTarget, actData, skillLevel,
                        PassivesLookup, PassiveCompLookup, passiveEffectsUtils, PassiveSpawn);
                }

                passiveEffectsUtils.TriggerOnActionPerformEffects(comp.CharacterEntity, ActiveEffectSpawnSpawn,
                    actData.SkillUsed, actData.ActionType, comp.CharacterEntity);
            }
            else if (activeAction.LastEffectIterationTime != InGameTime.TotalInGameSeconds &&
                     (InGameTime.TotalInGameSeconds - activeAction.StartTime) % 5 == 0){
                activeAction.LastEffectIterationTime = InGameTime.TotalInGameSeconds;
                actions[index] = activeAction;

                var targetData = new TargetsGroup();
                targetData.SetTargets(comp.CharacterEntity, activeTargets);
                var isSuccessful = true;
                
                var originElectronic = targetData.GetTargetEntity(TargetType.TARGET_ITEM);

                if (VideoLookup.TryGetBuffer(originElectronic, out DynamicBuffer<VideoElement> videos)){

                    var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

                    var frame = new VideoFrameElement(){
                        Data = new ImageData(){
                            Timestamp = new EventTimestamp(InGameTime, originElectronic)
                        }
                    };

                    if (DynamicActionType.Category == ActionCategory.SKILL_TECH){
                        switch (DynamicActionType.SkillBasedTechActionType){
                            case SkillBasedTechActionType.RECORD_VIDEO_OF_SELF:
                                var selfie = actionUtils.CaptureMediaMoment(TransformLookup, BehaviorCompLookup,
                                    ActionsLookup,
                                    TargetsLookup, comp.CharacterEntity, new MediaMomentData(){
                                        ActionPerformed = new DynamicActionType(SkillBasedTechActionType.RECORD_VIDEO_OF_SELF),
                                        ActionTarget = comp.CharacterEntity,
                                        ActionTargetType = TargetType.SELF
                                    }, true);
                                frame.Data.CharactersInView.Add(selfie);
                                break;
                            case SkillBasedTechActionType.RECORD_VIDEO_OF_CHARACTER:
                                var targ = targetData.GetTargetEntity(TargetType.TARGET_CHARACTER);
                                var data = actionUtils.CaptureMediaMoment(TransformLookup, BehaviorCompLookup,
                                    ActionsLookup,
                                    TargetsLookup, targ);
                                frame.Data.CharactersInView.Add(data);

                                break;
                            default:
                                if (PhaseAfterComplete < 0){
                                    actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                                    actionUtils.CompleteAction(activeAction, actions, index);
                                }
                                else{
                                    actionUtils.StartPhase(DynamicActionType, Ecb, e, PhaseAfterComplete);
                                }

                                return;
                        }
                    }

                    // todo not doing anythinng right now
                    if (actionData.SkillUsed == SkillType.COMMON){
                        isSuccessful = true;
                    }
                    else{
                        var chance = passiveEffectsUtils.GetSkillSuccessChance(actionData.SkillUsed,
                            actionData.DifficultyLevel);
                        // isSuccessful = Random.ValueRW.IsSuccessful(chance);
                        isSuccessful = true;
                    }

                    var found = false;
                    foreach (var videoElement in videos){
                        if (videoElement.GroupId == activeAction.PerformanceId){
                            found = true;
                            var frames = VideoFrameLookup[videoElement.ImagesEntity];
                            frames.Add(frame);
                        }
                    }

                    if (!found){
                        VideoSpawn.Add(new VideoSpawnElement(){
                            GroupId = activeAction.PerformanceId,
                            InitialFrame = frame,
                            TargetEntity = originElectronic
                        });
                    }
                    

                    actionUtils.SetCostEffects(comp.CharacterEntity, passiveEffectsUtils, actionData, ActiveEffectSpawnSpawn);

                    var performActionKnowledge = new ActionKnowledgeSpawnElement(){
                        PerformingCharacter = comp.CharacterEntity,
                        DynamicActionType = DynamicActionType,
                        IsSuccessful = isSuccessful,
                        TargetsData = targetData,
                    };

                    var learningTargets = new FixedList4096Bytes<Entity>();
                    learningTargets.Add(comp.CharacterEntity);

                    performActionKnowledge.TargetCharacters = learningTargets;

                    ActionKnowledgeSpawnElements.Add(performActionKnowledge);
                }
            }

            // if (activeTarget != Entity.Null){
            //     StateChangeSpawn.Add(new CharacterStateChangeSpawnElement(){
            //         Character = activeTarget,
            //         IsTargetOfExternalEvent = true,
            //     });
            // }
        }
    }

    
}

public struct PerformRecordVideoUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public InGameTime InGameTime;

    public DynamicBuffer<VideoSpawnElement> VideoSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveEffectsSpawn;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<SkillElement> _skillsLookup;
    
    private ComponentLookup<CharacterBehaviorComponent> _behaviorLookup;
    private ComponentLookup<LocalTransform> _transformLookup;
    
    private BufferLookup<ActiveActionElement> _actionsLookup;
    private BufferLookup<ActiveActionTargetElement> _targetsLookup;

    private BufferLookup<VideoElement> _videoLookup;
    private BufferLookup<VideoFrameElement> _vidFramesLookup;
    
    private BufferLookup<CharacterAttributeElement> _attributesLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;
        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonsComponentFunctionPerformQuery<T>());

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _videoLookup = state.GetBufferLookup<VideoElement>();
        _vidFramesLookup = state.GetBufferLookup<VideoFrameElement>();
        _behaviorLookup = state.GetComponentLookup<CharacterBehaviorComponent>();
        _transformLookup = state.GetComponentLookup<LocalTransform>();
        _actionsLookup = state.GetBufferLookup<ActiveActionElement>();
        _targetsLookup = state.GetBufferLookup<ActiveActionTargetElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _videoLookup.Update(ref state);
        _vidFramesLookup.Update(ref state);
        _actionsLookup.Update(ref state);
        _targetsLookup.Update(ref state);
        _behaviorLookup.Update(ref state);
        _transformLookup.Update(ref state);
        _attributesLookup.Update(ref state);
    }

    public PerformRecordVideoJob GetPerformRecordVideoJob(int nextPhase, bool triggerPassivesAfterPerformance = false){
        return new PerformRecordVideoJob(){
            Ecb = Ecb,
            InGameTime = InGameTime,
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
            ActionsLookup = _actionsLookup,
            VideoLookup = _videoLookup,
            VideoFrameLookup = _vidFramesLookup,
            TargetsLookup = _targetsLookup,
            TransformLookup = _transformLookup,
            BehaviorCompLookup = _behaviorLookup,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            SkillDataStore = SkillDataStore,
            AttributesLookup = _attributesLookup,
            VideoSpawn = VideoSpawnElements
        };
    }
}