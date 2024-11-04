using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct HeadlockActionSystem : ISystem {
    private PerformActionWhileMovingToTargetUtil _moveToTarget;
    private TriggerSuccessCheckAndCostUtil _successCheck;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesLegsMentalActionFunction>();
        _moveToTarget.SetUp<ArmsEyesLegsMentalActionFunction>(ref state,
            new DynamicActionType(GrapplingActionType.HEADLOCK), 0);
        _successCheck.SetUp<ArmsEyesLegsMentalActionFunction>(ref state,
            new DynamicActionType(GrapplingActionType.HEADLOCK), 1);
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

        _successCheck.Ecb = ecb;
        _successCheck.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _successCheck.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _successCheck.ActionDataStore = actionDataStore;
        _successCheck.ActiveEffectSpawnElements = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();

        _successCheck.UpdateBufferLookups(ref state);
        _moveToTarget.UpdateBufferLookups(ref state);

        var performJob = _moveToTarget.GetMoveToTargetPerformJob(1, true);
        var triggerJob = _successCheck.GetActionSuccessCheckAndPayCostJob(2);

        var h1 = performJob.Schedule(_moveToTarget.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_successCheck.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}