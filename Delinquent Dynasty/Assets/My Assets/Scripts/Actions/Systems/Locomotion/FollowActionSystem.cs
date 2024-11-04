using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct FollowActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private ActionDataStore _dataStore;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<LegsActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<LegsActionFunction>());

        _startPhaseQuery.SetSharedComponentFilter(new LegsActionFunction()
            {State = new ActionFunctionBase(){ActionType = new DynamicActionType(LocomotionActionType.FOLLOW)}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var bodyLookup = SystemAPI.GetComponentLookup<AgentBody>();
        var locoLookup = SystemAPI.GetComponentLookup<AgentLocomotion>();
        var stateLookup = SystemAPI.GetComponentLookup<CharacterWorldStateComponent>();
      
        var transLookup = SystemAPI.GetComponentLookup<LocalTransform>();

        JobHandle jobHandle = new FollowTargetJob(){
            DataStore = _dataStore,
            TransformLookup = transLookup,
            Ecb = ecb,
            AgentBodyLookup = bodyLookup,
            AgentLocomotionLookup = locoLookup,
            WorldStateLookup = stateLookup,
        }.Schedule(_startPhaseQuery, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(jobHandle, state.Dependency);
    }
}

[BurstCompile]
public partial struct FollowTargetJob : IJobEntity {
    public EntityCommandBuffer Ecb;
    public ActionDataStore DataStore;
    [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
    public ComponentLookup<CharacterWorldStateComponent> WorldStateLookup;
    public ComponentLookup<AgentBody> AgentBodyLookup;
    public ComponentLookup<AgentLocomotion> AgentLocomotionLookup;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var trip = new LocomotionUtils();

        var actionUtils = new ActionUtils(){
            ActionDataStore = DataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(new DynamicActionType(LocomotionActionType.FOLLOW), actions,
                targets, TargetType.TARGET_CHARACTER, out ActiveActionElement activeAction, out Entity target,
                out int index)){
            
            var body = AgentBodyLookup[comp.CharacterEntity];
            var loco = AgentLocomotionLookup[comp.CharacterEntity];
            var worldState = WorldStateLookup[comp.CharacterEntity];
            
            trip.ContinuousLocomotion(activeAction, actions, index, TransformLookup[target].Position, 2,
                loco, body, worldState,Ecb, comp.CharacterEntity );
        }
    }
}