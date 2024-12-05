using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct TakeSelfieActionSystem : ISystem {
    private PerformActionUtil _perform;
    private TriggerActionEffectsUtil _triggerEffects;
    private TriggerPhotoEffectUtil _triggerPhoto;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ArmsEyesActionFunction>();
        _perform.SetUp<ArmsEyesActionFunction>(ref state,
            new DynamicActionType(SkillBasedTechActionType.TAKE_SELFIE), 0);
        _triggerEffects.SetUp<ArmsEyesActionFunction>(ref state,
            new DynamicActionType(SkillBasedTechActionType.TAKE_SELFIE), 1);
        _triggerPhoto.SetUp<ArmsEyesActionFunction>(ref state,
            new DynamicActionType(SkillBasedTechActionType.TAKE_SELFIE), 2);
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
        
        _triggerEffects.Ecb = ecb;
        _triggerEffects.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _triggerEffects.ActionDataStore = actionDataStore;
        _triggerEffects.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _triggerEffects.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _triggerEffects.StateChangeSpawnElements = stateChangeSpawn;
        _triggerEffects.ActiveEffectSpawnElements = effectSpawn;

        _triggerPhoto.Ecb = ecb;
        _triggerPhoto.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _triggerPhoto.ActionDataStore = actionDataStore;
        _triggerPhoto.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _triggerPhoto.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _triggerPhoto.StateChangeSpawnElements = stateChangeSpawn;
        _triggerPhoto.ActiveEffectSpawnElements = effectSpawn;
        _triggerPhoto.InGameTime = time;

        _perform.UpdateBufferLookups(ref state);
        _triggerEffects.UpdateBufferLookups(ref state);
        _triggerPhoto.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformActionJob(1);
        var triggerJob = _triggerEffects.GetTriggerActionEffectsJob(2);
        var triggerPhotoJob = _triggerPhoto.GetTriggerPhotoEffectJob(-1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_triggerEffects.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
        
        var h3 = triggerPhotoJob.Schedule(_triggerPhoto.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h3, state.Dependency);
    }
}