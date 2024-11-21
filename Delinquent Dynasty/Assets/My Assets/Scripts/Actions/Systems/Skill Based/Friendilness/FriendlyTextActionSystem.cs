using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct FriendlyTextActionSystem : ISystem {
    private PerformActionUtil _perform;
    private TriggerSendTextMessageUtil _triggerSend;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<MentalMouthActionFunction>();
        _perform.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(SkillBasedSocialActionType.FRIENDLY_TEXT), 0);
        _triggerSend.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(SkillBasedSocialActionType.FRIENDLY_TEXT), 1);
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

        _triggerSend.Ecb = ecb;
        _triggerSend.Random = SystemAPI.GetSingletonRW<RandomComponent>();
        _triggerSend.ActionDataStore = actionDataStore;
        _triggerSend.SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _triggerSend.ActionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        _triggerSend.StateChangeSpawnElements = stateChangeSpawn;
        _triggerSend.ActiveEffectSpawnElements = effectSpawn;
        _triggerSend.KnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<KnowledgeSpawnElement>();

        _perform.UpdateBufferLookups(ref state);
        _triggerSend.UpdateBufferLookups(ref state);

        var performJob = _perform.GetPerformActionJob(1);
        var triggerJob = _triggerSend.GetTriggerActionEffectsJob(-1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = triggerJob.Schedule(_triggerSend.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}