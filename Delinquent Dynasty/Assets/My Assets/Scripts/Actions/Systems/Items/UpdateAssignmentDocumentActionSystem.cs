using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct UpdateAssignmentDocumentActionSystem : ISystem {
    private PerformAssignmentActionUtil _perform;
    private TriggerAssignmentEffectsUtil _triggerAssignmentEffects;
    private TriggerActionEffectsUtil _triggerEffects;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesMentalActionFunction>();
        
        _perform.SetUp<ArmsEyesMentalActionFunction>(ref state, new DynamicActionType(CommonItemActionType.UPDATE_ASSIGNMENT_DOCUMENT),
            0);
        
        _triggerAssignmentEffects.SetUp<ArmsEyesMentalActionFunction>(ref state,
            new DynamicActionType(CommonItemActionType.UPDATE_ASSIGNMENT_DOCUMENT), 1);
        
        _triggerEffects.SetUp<ArmsEyesMentalActionFunction>(ref state,
            new DynamicActionType(CommonItemActionType.UPDATE_ASSIGNMENT_DOCUMENT), 2);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var rand = SystemAPI.GetSingletonRW<RandomComponent>();
        var skillData = SystemAPI.GetSingleton<SkillDataStore>();
        var actKnowSpawn = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        
        _perform.Ecb = ecb;
        _perform.InGameTime = SystemAPI.GetSingleton<InGameTime>();
        _perform.ActionDataStore = actionDataStore;
        _perform.ActiveEffectsSpawn = effectSpawn;

        _triggerAssignmentEffects.Ecb = ecb;
        _triggerAssignmentEffects.Random = rand;
        _triggerAssignmentEffects.ActionDataStore = actionDataStore;
        _triggerAssignmentEffects.SkillDataStore = skillData;
        _triggerAssignmentEffects.ActionKnowledgeSpawnElements = actKnowSpawn;
        _triggerAssignmentEffects.StateChangeSpawnElements = stateChangeSpawn;
        
        _triggerEffects.Ecb = ecb;
        _triggerEffects.Random = rand;
        _triggerEffects.ActionDataStore = actionDataStore;
        _triggerEffects.SkillDataStore = skillData;
        _triggerEffects.ActionKnowledgeSpawnElements = actKnowSpawn;
        _triggerEffects.StateChangeSpawnElements = stateChangeSpawn;
        _triggerEffects.ActiveEffectSpawnElements = effectSpawn;

        _perform.UpdateBufferLookups(ref state);
        _triggerAssignmentEffects.UpdateBufferLookups(ref state);
        _triggerEffects.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformAssignmentActionJob(1);
        var triggerJob = _triggerAssignmentEffects.GetTriggerAssignmentEffectsJob(2);
        var lastTriggerJob = _triggerEffects.GetTriggerActionEffectsJob(-1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_triggerAssignmentEffects.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
        
        var h3 = lastTriggerJob.Schedule(_triggerEffects.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h3, state.Dependency);
    }
}