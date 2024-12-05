using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[BurstCompile]
public partial struct TriggerAssignmentEffectsJob : IJobEntity {
// todo try to make this job parallel?
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;

    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    
    [ReadOnly] public ComponentLookup<AssignmentDocumentComponent> AssignmentDocLookup;
    [ReadOnly] public ComponentLookup<ClassroomAssignmentComponent> ClassAssignmentLookup;

    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
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
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);
            var target = targetData.GetTargetEntity(TargetType.TARGET_ASSIGNMENT_DOCUMENT);
            
            var doc = AssignmentDocLookup[target];
            var ogDoc = ClassAssignmentLookup[doc.Assignment];
            
            var chance = passiveUtils.GetSkillSuccessChance(ogDoc.SkillType, ogDoc.DifficultyLevel);
            isSuccessful = Random.ValueRW.IsSuccessful(chance);

            if (isSuccessful){
                doc.CorrectAnswers += 1;
            }
            else{
                doc.WrongAnswers += 1;
            }
            
            Ecb.SetComponent(target, doc);

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

public struct TriggerAssignmentEffectsUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public RefRW<RandomComponent> Random;
    
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;

    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;
    private ComponentLookup<AssignmentDocumentComponent> _docAssignmentLookup;
    private ComponentLookup<ClassroomAssignmentComponent> _classAssignmentLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _classAssignmentLookup = state.GetComponentLookup<ClassroomAssignmentComponent>();
        _docAssignmentLookup = state.GetComponentLookup<AssignmentDocumentComponent>();

        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _docAssignmentLookup.Update(ref state);
        _classAssignmentLookup.Update(ref state);
    }

    public TriggerAssignmentEffectsJob GetTriggerAssignmentEffectsJob(int nextPhase){
        return new TriggerAssignmentEffectsJob(){
            Random = Random,
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            SkillDataStore = SkillDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            PassiveCompLookup = _passiveCompLookup,
            StateChangeSpawnElements = StateChangeSpawnElements,
            PassivesLookup = _passivesLookup,
            NextPhase = nextPhase,
            AttributesLookup = _attributesLookup,
            SkillsLookup = _skillsLookup,
            Ecb = Ecb,
            AssignmentDocLookup = _docAssignmentLookup,
            ClassAssignmentLookup = _classAssignmentLookup,
        };
    }
}