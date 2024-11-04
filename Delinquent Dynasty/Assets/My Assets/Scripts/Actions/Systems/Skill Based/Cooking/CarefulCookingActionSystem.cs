using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct CarefulCookingActionSystem : ISystem {
    private PerformRecipeBuildActionUtil _perform;
    private TriggerCraftingBuildUtil _triggerBuild;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesActionFunction>();
        _perform.SetUp<ArmsEyesActionFunction>(ref state,
            new DynamicActionType(SkillBasedItemActionType.CAREFUL_COOKING), 0);
        _triggerBuild.SetUp<ArmsEyesActionFunction>(ref state,
            new DynamicActionType(SkillBasedItemActionType.CAREFUL_COOKING), 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var skillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        // var itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();
        var actKnowSpawn = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();

        _perform.CraftingDataStore = SystemAPI.GetSingleton<CraftingDataStore>();
        _perform.Ecb = ecb.AsParallelWriter();
        _perform.InGameTime = SystemAPI.GetSingleton<InGameTime>();
        _perform.ActionDataStore = actionDataStore;
        _perform.PerformTimeModifier = 1.5f;

        _triggerBuild.Ecb = ecb;
        _triggerBuild.ActionDataStore = actionDataStore;
        _triggerBuild.SkillDataStore = skillDataStore;
        _triggerBuild.ActionKnowledgeSpawnElements = actKnowSpawn;
        _triggerBuild.StateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        _triggerBuild.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _triggerBuild.ActiveEffectsSpawnElements = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();


        _perform.UpdateBufferLookups(ref state);
        _triggerBuild.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformRecipeBuildActionJob(1);
        var triggerPrepJob = _triggerBuild.GetTriggerCraftingBuildJob(-1);

        var h1 = performJob.ScheduleParallel(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerPrepJob.Schedule(_triggerBuild.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}