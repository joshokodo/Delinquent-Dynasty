using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct CloseBusinessActionSystem : ISystem {
    private PerformActionUtil _perform;
    private EntityQuery _setBusinessQuery;
    private DynamicActionType _actionType;
    private ComponentLookup<CommercableLocationComponent> _commercableLookup;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.CLOSE_BUSINESS);
        state.RequireForUpdate<ArmsActionFunction>();

        _perform.SetUp<ArmsActionFunction>(ref state, _actionType, 0);
        _setBusinessQuery =
            state.GetEntityQuery(CommonSystemUtils
                .BuildCommonActionTargetsFunctionPerformQuery<ArmsActionFunction>());

        _setBusinessQuery.SetSharedComponentFilter(new ArmsActionFunction(){
            State = new ActionFunctionBase(){
                ActionType = _actionType, Phase = 1
            }
        });

        _commercableLookup = state.GetComponentLookup<CommercableLocationComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        _perform.Ecb = ecb;
        _perform.InGameTime = SystemAPI.GetSingleton<InGameTime>();
        _perform.ActionDataStore = actionDataStore;
        _perform.StateChangeSpawn = stateChangeSpawn;
        _perform.ActiveEffectsSpawn = effectSpawn;
        _perform.PassiveEffectsSpawn = passiveSpawn;

        _perform.UpdateBufferLookups(ref state);
        _commercableLookup.Update(ref state);

        var performJob = _perform.GetPerformActionJob(1);

        var h1 = performJob.Schedule(_perform.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new SetBusinessJob(){
            Ecb = ecb,
            ActionType = _actionType,
            CommercableLookup = _commercableLookup,
            StateChangeSpawnElements = stateChangeSpawnElements,
            SetValue = false,
            ActionDataStore = actionDataStore,
            NextPhase = -1
        }.Schedule(_setBusinessQuery, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}