using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct ReadItemActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _dataStore;
    private SkillDataStore _skillDataStore;
    private ItemDataStore _itemDataStore;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.READ_ITEM);
        state.RequireForUpdate<ArmsMentalActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<ArmsMentalActionFunction>());
        _mainPhaseQuery = state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsFunctionPerformQuery<ArmsMentalActionFunction>());


        _startPhaseQuery.SetSharedComponentFilter(new ArmsMentalActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType}});
        _mainPhaseQuery.SetSharedComponentFilter(new ArmsMentalActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _skillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        _itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();

        _passiveCompLookup.Update(ref state);

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var time = SystemAPI.GetSingleton<InGameTime>();
        var rand = SystemAPI.GetSingletonRW<RandomComponent>();
        var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var activeEffectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();

        var h1 = new ItemActionFunctionPerformJob(){
            Ecb = ecb.AsParallelWriter(),
            TotalInGameSeconds = time.TotalInGameSeconds,
            ActionType = _actionType,
            ActionDataStore = _dataStore,
            ItemDataStore = _itemDataStore,
            NextPhase = 1,
            ItemBaseLookup = itemBaseLookup,
        }.Schedule(_startPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new TriggerItemEffectsJob(){
            Random = rand,
            ActionDataStore = _dataStore,
            DynamicActionType = _actionType,
            ActiveEffectSpawn = activeEffectSpawn,
            SkillDataStore = _skillDataStore,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            ItemBaseLookup = itemBaseLookup,
            ItemDataStore = _itemDataStore,
            PassiveCompLookup = _passiveCompLookup,
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}