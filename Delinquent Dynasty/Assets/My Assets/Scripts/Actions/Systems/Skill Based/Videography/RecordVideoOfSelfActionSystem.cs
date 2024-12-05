using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct RecordVideoOfSelfActionSystem : ISystem {
    private PerformRecordVideoUtil _perform;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesActionFunction>();
        _perform.SetUp<ArmsEyesActionFunction>(ref state, new DynamicActionType(SkillBasedTechActionType.RECORD_VIDEO_OF_SELF), 0);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();
        var time = SystemAPI.GetSingleton<InGameTime>();

        _perform.Ecb = ecb;
        _perform.InGameTime = time;
        _perform.ActionDataStore = actionDataStore;
        _perform.StateChangeSpawn = stateChangeSpawn;
        _perform.ActiveEffectsSpawn = effectSpawn;
        _perform.PassiveEffectsSpawn = passiveSpawn;
        _perform.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _perform.VideoSpawnElements = SystemAPI.GetSingletonBuffer<VideoSpawnElement>();
        _perform.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();

        _perform.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformRecordVideoJob(-1);
        
        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);
    }
}