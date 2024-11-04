using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct AppealingDismissalActionSystem : ISystem {
    private PerformActionUtil _triggerPassives;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<MentalMouthActionFunction>();
        _triggerPassives.SetUp<MentalMouthActionFunction>(ref state,
            new DynamicActionType(SkillBasedSocialActionType.APPEALING_DISMISSAL), 0);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _triggerPassives.ActionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _triggerPassives.Ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        _triggerPassives.PassiveEffectsSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        _triggerPassives.UpdateBufferLookups(ref state);
        var passiveJob = _triggerPassives.GetPerformActionJob(1);

        var h1 = passiveJob.Schedule(_triggerPassives.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);
    }
}