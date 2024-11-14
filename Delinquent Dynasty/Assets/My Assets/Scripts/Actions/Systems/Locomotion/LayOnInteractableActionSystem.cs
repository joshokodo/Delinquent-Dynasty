using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct LayOnInteractableActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _endPhaseQuery;
    private ActionDataStore _dataStore;
    private ComponentLookup<InteractableLocationComponent> _locCompLookup;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(LocomotionActionType.LAY_ON_INTERACTABLE);
        state.RequireForUpdate<LegsActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<LegsActionFunction>());
        _endPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils
                .BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<LegsActionFunction>());

        _locCompLookup = state.GetComponentLookup<InteractableLocationComponent>();

        _startPhaseQuery.SetSharedComponentFilter(new LegsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType}});
        _endPhaseQuery.SetSharedComponentFilter(new LegsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        //TODO: update to private fields and .update(ref state)
        var selCharLookup = SystemAPI.GetComponentLookup<SelectedCharacter>();
        var invTagLookup = SystemAPI.GetComponentLookup<InteractableInventoryComponent>();
        var doorTagLookup = SystemAPI.GetComponentLookup<DoorTag>();
        var sinkTagLookup = SystemAPI.GetComponentLookup<SinkTag>();
        var toiletTagLookup = SystemAPI.GetComponentLookup<ToiletTag>();
        var bodyLookup = SystemAPI.GetComponentLookup<AgentBody>();
        var locoLookup = SystemAPI.GetComponentLookup<AgentLocomotion>();
        var stateLookup = SystemAPI.GetComponentLookup<CharacterWorldStateComponent>();
        var transLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        
        _locCompLookup.Update(ref state);

        JobHandle destinationJh = new TravelToInteractableLocomotionJob(){
            DynamicActionType = _actionType,
            InteractableLocationComponentLookup = _locCompLookup,
            DataStore = _dataStore,
            Ecb = ecb,
            GoToOccupyingPoint = true,
            TargetInteractable = TargetType.TARGET_BED,
            AgentBodyLookup = bodyLookup,
            AgentLocomotionLookup = locoLookup,
            FinalLocomotionState = LocomotionState.STANDING,
            OccupyLocation = false,
            SelectedLookup = selCharLookup,
            InventoryTagLookup = invTagLookup,
            DoorTagLookup = doorTagLookup,
            SinkTagLookup = sinkTagLookup,
            ToiletTagLookup = toiletTagLookup,
            TransformLookup = transLookup,
            WorldStateLookup = stateLookup,
        }.Schedule(_startPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(destinationJh, state.Dependency);
    }
}