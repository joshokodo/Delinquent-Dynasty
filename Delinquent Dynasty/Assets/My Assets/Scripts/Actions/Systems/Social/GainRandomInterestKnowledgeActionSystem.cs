using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct GainRandomInterestKnowledgeActionSystem : ISystem {
  private PerformActionUtil _perform;
    private TriggerInfluencedActionEffectsUtil _triggerInfluenced;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<MentalMouthActionFunction>();
        _perform.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(CommonSocialActionType.GAIN_RANDOM_INTEREST_KNOWLEDGE), 0);
        _triggerInfluenced.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(CommonSocialActionType.GAIN_RANDOM_INTEREST_KNOWLEDGE), 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        _perform.Ecb = ecb;
        _perform.StateChangeSpawn = stateChangeSpawn;
        _perform.ActiveEffectsSpawn = effectSpawn;
        _perform.PassiveEffectsSpawn = passiveSpawn;
        _perform.ActionDataStore = actStore;
        
        _perform.InGameTime = SystemAPI.GetSingleton<InGameTime>();

        _triggerInfluenced.Ecb = ecb;
        _triggerInfluenced.ActionDataStore = actStore;
        _triggerInfluenced.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _triggerInfluenced.StateChangeSpawnElements = stateChangeSpawn;
        _triggerInfluenced.ActiveEffectSpawnElements = effectSpawn;
        _triggerInfluenced.RandomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        _perform.UpdateBufferLookups(ref state);
        _triggerInfluenced.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformActionJob(1);
        var influencedTriggeredJob = _triggerInfluenced.GetInfluencedTriggeredActionEffectsJob(-1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = influencedTriggeredJob.Schedule(_triggerInfluenced.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}