using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct HeadbuttActionSystem : ISystem {
    private PerformActionWhileMovingToTargetUtil _moveToTarget;
    private TriggerActionEffectsUtil _triggerEffects;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesLegsMentalActionFunction>();
        _moveToTarget.SetUp<ArmsEyesLegsMentalActionFunction>(ref state,
            new DynamicActionType(FightingActionType.HEADBUTT), 0);
        _triggerEffects.SetUp<ArmsEyesLegsMentalActionFunction>(ref state,
            new DynamicActionType(FightingActionType.HEADBUTT), 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();
        
        _moveToTarget.Ecb = ecb;
        _moveToTarget.InGameTime = SystemAPI.GetSingleton<InGameTime>();
        _moveToTarget.ActionDataStore = actionDataStore;
        _moveToTarget.StateChangeSpawn = stateChangeSpawn;
        _moveToTarget.PassiveEffectsSpawn = passiveSpawn;

        _triggerEffects.Ecb = ecb;
        _triggerEffects.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _triggerEffects.ActionDataStore = actionDataStore;
        _triggerEffects.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _triggerEffects.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _triggerEffects.StateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        _triggerEffects.ActiveEffectSpawnElements = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();

        _moveToTarget.UpdateBufferLookups(ref state);
        _triggerEffects.UpdateBufferLookups(ref state);

        var performJob = _moveToTarget.GetMoveToTargetPerformJob(1);
        var triggerJob = _triggerEffects.GetTriggerActionEffectsJob(-1);

        var h1 = performJob.Schedule(_moveToTarget.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_triggerEffects.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}