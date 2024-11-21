using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct TriggerPhotoEffectJob : IJobEntity {
// todo try to make this job parallel?
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;
    [ReadOnly] public InGameTime InGameTime;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random; // todo: might need if there is a chance to fail. if not, remove and other stuff here we dont need
    
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<ActiveActionTargetElement> TargetsLookup;
    [ReadOnly] public ComponentLookup<CharacterBehaviorComponent> BehaviorCompLookup;

    public BufferLookup<ActiveActionElement> ActionsLookup;
    public BufferLookup<PhotoElement> PhotoLookup;
    public ComponentLookup<LocalTransform> TransformLookup;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp){
        var actions = ActionsLookup[e];
        var targets = TargetsLookup[e];
        
        bool isSuccessful;

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
        
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);


            var originPhone = targetData.GetTargetEntity(TargetType.TARGET_ITEM);

            if (PhotoLookup.TryGetBuffer(originPhone, out DynamicBuffer<PhotoElement> photos)){
                
                var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

                var photo = new PhotoElement(){
                    Timestamp = new EventTimestamp(InGameTime, originPhone)
                };
                
                if (DynamicActionType.Category == ActionCategory.SKILL_ITEMS){
                    switch (DynamicActionType.SkillBasedItemActionType){
                        case SkillBasedItemActionType.TAKE_SELFIE:
                            var pos = TransformLookup[comp.CharacterEntity].Position;
                            photo.CharactersInView.Add(new MediaMomentData(){
                                Character = comp.CharacterEntity,
                                ActionPerformed = new DynamicActionType(SkillBasedItemActionType.TAKE_SELFIE),
                                Position = pos,
                                ActionTarget = comp.CharacterEntity,
                                ActionTargetType = TargetType.SELF
                            });
                            break;
                        case SkillBasedItemActionType.TAKE_PHOTO_OF_CHARACTER:
                            var targ = targetData.GetTargetEntity(TargetType.TARGET_CHARACTER);
                            var targPos = TransformLookup[targ].Position;
                            var data = new MediaMomentData(){
                                Character = targetData.GetTargetEntity(TargetType.TARGET_CHARACTER),
                                ActionPerformed = new DynamicActionType(SkillBasedItemActionType.TAKE_SELFIE),
                                Position = targPos,
                                ActionTarget = comp.CharacterEntity,
                                ActionTargetType = TargetType.SELF
                            };
                            
                            var behavior = BehaviorCompLookup[targ];
                            var activeActs = ActionsLookup[behavior.BehaviorEntity];
                            var activeTargs = TargetsLookup[behavior.BehaviorEntity];
                            foreach (var act in activeActs){
                                if (act.HasStarted){
                                    switch (act.ActionType.Category){
                                        case ActionCategory.FIGHTING:
                                        case ActionCategory.MISC:
                                        case ActionCategory.GRAPPLING:
                                        case ActionCategory.SKILL_SOCIAL:
                                        case ActionCategory.COMMON_ITEMS:
                                        case ActionCategory.COMMON_SOCIAL:
                                        case ActionCategory.SKILL_ITEMS:
                                            data.ActionPerformed = act.ActionType;
                                            
                                            foreach (var at in activeTargs){
                                                if (at.ActionId == act.ActionId){
                                                    switch (at.Data.TargetType){
                                                        case TargetType.TARGET_CHARACTER:
                                                        case TargetType.TARGET_ITEM:
                                                            data.ActionTarget = at.Data.TargetEntity;
                                                            data.ActionTargetType = at.Data.TargetType;
                                                            break;
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case ActionCategory.LOCOMOTION:
                                            break;
                                    }
                                }

                                if (!data.ActionPerformed.IsNull){
                                    break;
                                }
                            }
                            
                    
                            photo.CharactersInView.Add(data);
                            
                            break;
                        default:
                            if (NextPhase < 0){
                                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                                actionUtils.CompleteAction(activeAction, actions, index);
                            }
                            else{
                                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
                            }

                            return;
                    }
                }

                // todo not doing anythinng right now
                if (actionData.SkillUsed == SkillType.COMMON){
                    isSuccessful = true;
                }
                else{
                    var chance = passiveUtils.GetSkillSuccessChance(actionData.SkillUsed,
                        actionData.DifficultyLevel);
                    // isSuccessful = Random.ValueRW.IsSuccessful(chance);
                    isSuccessful = true;
                }

                photos.Add(photo);

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

                actionUtils.SetCostEffects(comp.CharacterEntity, passiveUtils, actionData, ActiveEffectSpawnSpawn);
                
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

public struct TriggerPhotoEffectUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public RefRW<RandomComponent> Random;
    public InGameTime InGameTime;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private ComponentLookup<AccessKnowledgeComponent> _accessCompLookup;
    private ComponentLookup<CharacterBehaviorComponent> _behaviorLookup;
    private ComponentLookup<LocalTransform> _transformLookup;
    
    private BufferLookup<ActiveActionElement> _actionsLookup;
    private BufferLookup<ActiveActionTargetElement> _targetsLookup;

    private BufferLookup<PhotoElement> _photoLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonsComponentFunctionPerformQuery<T>());

        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _photoLookup = state.GetBufferLookup<PhotoElement>();
        _accessCompLookup = state.GetComponentLookup<AccessKnowledgeComponent>();
        _behaviorLookup = state.GetComponentLookup<CharacterBehaviorComponent>();
        _transformLookup = state.GetComponentLookup<LocalTransform>();
        _actionsLookup = state.GetBufferLookup<ActiveActionElement>();
        _targetsLookup = state.GetBufferLookup<ActiveActionTargetElement>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _photoLookup.Update(ref state);
        _accessCompLookup.Update(ref state);
        _actionsLookup.Update(ref state);
        _targetsLookup.Update(ref state);
        _behaviorLookup.Update(ref state);
        _transformLookup.Update(ref state);
    }

    public TriggerPhotoEffectJob GetTriggerPhotoEffectJob(int nextPhase){
        return new TriggerPhotoEffectJob(){
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
            PhotoLookup = _photoLookup,
            ActionsLookup = _actionsLookup,
            TargetsLookup = _targetsLookup,
            TransformLookup = _transformLookup,
            BehaviorCompLookup = _behaviorLookup,
        };
    }
}