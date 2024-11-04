using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct UseItemActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _dataStore;
    private SkillDataStore _skillDataStore;
    private ItemDataStore _itemDataStore;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        // _actionType = new DynamicActionType(CommonItemActionType.USE_ITEM);
        // state.RequireForUpdate<ArmsActionFunction>();
        //
        // _startPhaseQuery =
        //     state.GetEntityQuery(
        //         CommonSystemUtils.BuildCommonActionFunctionPerformQuery<ArmsActionFunction>());
        // _mainPhaseQuery =
        //     state.GetEntityQuery(CommonSystemUtils.BuildTriggerEffectsQuery(typeof(ArmsActionFunction)));
        //
        // _startPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
        //     {State = new ActionFunctionBase(){ActionType = _actionType}});
        // _mainPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
        //     {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
        //
        // _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        //  _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        //  _skillDataStore = SystemAPI.GetSingleton<SkillDataStore>();
        //  _itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();
        //  
        //  _passiveCompLookup.Update(ref state);
        //  
        //  var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
        //      .CreateCommandBuffer(state.WorldUnmanaged);
        //  var time = SystemAPI.GetSingleton<InGameTime>();
        //  var rand = SystemAPI.GetSingletonRW<RandomComponent>();
        //  var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        //  var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        //  var targetsLookup = SystemAPI.GetBufferLookup<TargetElement>();
        //  var actionsLookup = SystemAPI.GetBufferLookup<ActionElement>();
        //  var activeEffectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        //  var traitIntensityLookup = SystemAPI.GetBufferLookup<TraitIntensityElement>();
        //  var traitBaseLookup = SystemAPI.GetComponentLookup<TraitBaseComponent>();
        //  
        //  var h1 = new ItemActionFunctionPerformJob(){
        //          Ecb = ecb.AsParallelWriter(),
        //          TotalInGameSeconds = time.TotalInGameSeconds,
        //          ActionType = _actionType,
        //          ActionDataStore = _dataStore,
        //          ItemDataStore = _itemDataStore,
        //          NextPhase = 1,
        //          ItemBaseLookup = itemBaseLookup,
        //          ActionsLookup = actionsLookup,
        //          TargetsLookup = targetsLookup,
        //      }.Schedule(_startPhaseQuery, state.Dependency);
        //  
        //  state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);
        //  
        // var h2 = new TriggerItemEffectsJob(){
        //      Random = rand,
        //      ActionDataStore = _dataStore,
        //      DynamicActionType = _actionType,
        //      SkillDataStore = _skillDataStore,
        //      ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
        //      ActionsLookup = actionsLookup,
        //      TargetsLookup = targetsLookup,
        //      ItemBaseLookup = itemBaseLookup,
        //      ActiveEffectSpawn = activeEffectSpawn,
        //      ItemDataStore = _itemDataStore,
        //      PassiveCompLookup = _passiveCompLookup,
        //      TraitCompLookup = traitBaseLookup,
        //      TraitIntensityLookup = traitIntensityLookup
        //  }.Schedule(_mainPhaseQuery, state.Dependency);
        //  
        //  state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}