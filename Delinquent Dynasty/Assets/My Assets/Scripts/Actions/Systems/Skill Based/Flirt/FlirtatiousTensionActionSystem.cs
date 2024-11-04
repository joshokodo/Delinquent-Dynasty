using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct FlirtatiousTensionActionSystem : ISystem {
    private PerformActionUtil _perform;
    private TriggerActionEffectsUtil _triggerEffects;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<MentalMouthActionFunction>();
        _perform.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(SkillBasedSocialActionType.FLIRTATIOUS_TENSION), 0);
        _triggerEffects.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(SkillBasedSocialActionType.FLIRTATIOUS_TENSION), 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        
        _perform.Ecb = ecb;
        _perform.InGameTime = SystemAPI.GetSingleton<InGameTime>();
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

        _perform.UpdateBufferLookups(ref state);
        _triggerEffects.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformActionJob(1);
        var triggerJob = _triggerEffects.GetTriggerActionEffectsJob(-1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_triggerEffects.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}