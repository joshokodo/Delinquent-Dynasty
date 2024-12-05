  
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct PerformAssignmentActionJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public double TotalInGameSeconds;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public ComponentLookup<AssignmentDocumentComponent> AssignmentDocLookup;
    [ReadOnly] public ComponentLookup<ClassroomAssignmentComponent> ClassAssignmentLookup;
    
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;


    public EntityCommandBuffer Ecb;
    
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
                TargetType.TARGET_ASSIGNMENT_DOCUMENT, out ActiveActionElement activeAction, out Entity target, out int index)){

            var doc = AssignmentDocLookup[target];
            var ogDoc = ClassAssignmentLookup[doc.Assignment];

            var actData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

            if (actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, ogDoc.ProblemTimeInSecs, Ecb, e,
                    PhaseAfterComplete, out _)){
                passiveEffectsUtils.TriggerOnActionPerformEffects(comp.CharacterEntity, ActiveEffectSpawnSpawn, actData.SkillUsed, actData.ActionType, comp.CharacterEntity);
            }
        }
    }
}

public struct PerformAssignmentActionUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public EntityCommandBuffer Ecb;
    public InGameTime InGameTime;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;

    public EntityQuery Query;

    private ComponentLookup<AssignmentDocumentComponent> _docAssignmentLookup;
    private ComponentLookup<ClassroomAssignmentComponent> _classAssignmentLookup;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;
        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _classAssignmentLookup = state.GetComponentLookup<ClassroomAssignmentComponent>();
        _docAssignmentLookup = state.GetComponentLookup<AssignmentDocumentComponent>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _docAssignmentLookup.Update(ref state);
        _classAssignmentLookup.Update(ref state);
    }

    public PerformAssignmentActionJob GetPerformAssignmentActionJob(int nextPhase){
        return new PerformAssignmentActionJob(){
            Ecb = Ecb,
            TotalInGameSeconds = InGameTime.TotalInGameSeconds,
            DynamicActionType = ActionType,
            ActionDataStore = ActionDataStore,
            PhaseAfterComplete = nextPhase,
            PassiveCompLookup = _passiveCompLookup,
            PassivesLookup = _passivesLookup,
            ActiveEffectSpawnSpawn = ActiveEffectsSpawn,
            SkillsLookup = _skillsLookup,
            AssignmentDocLookup = _docAssignmentLookup,
            ClassAssignmentLookup = _classAssignmentLookup,
        };
    }
}