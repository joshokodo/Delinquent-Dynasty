using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct FollowPathActionSystem : ISystem {
    private ActionDataStore _dataStore;
    private EntityQuery _initPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private BufferLookup<PathPointElement> _pointsLookup;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<LegsActionFunction>();
        _initPhaseQuery = state.GetEntityQuery(CommonSystemUtils
            .BuildCommonActionTargetsComponentFunctionPerformQuery<LegsActionFunction>());

        _mainPhaseQuery = state.GetEntityQuery(CommonSystemUtils
            .BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<LegsActionFunction>());

        _pointsLookup = state.GetBufferLookup<PathPointElement>();

        _initPhaseQuery.SetSharedComponentFilter(new LegsActionFunction()
            {State = new ActionFunctionBase(){ActionType = new DynamicActionType(LocomotionActionType.FOLLOW_PATH)}});

        _mainPhaseQuery.SetSharedComponentFilter(new LegsActionFunction(){
            State = new ActionFunctionBase()
                {ActionType = new DynamicActionType(LocomotionActionType.FOLLOW_PATH), Phase = 1}
        });
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _pointsLookup.Update(ref state);
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var bodyLookup = SystemAPI.GetComponentLookup<AgentBody>();
        var stateLookup = SystemAPI.GetComponentLookup<CharacterWorldStateComponent>();
        
        // todo: used to have both jobs logic combined into one. wasnt working at the time, after tweaking, combining logic should work now
        JobHandle initJh = new InitFollowPathJob(){
            DataStore = _dataStore,
            PointsLookup = _pointsLookup,
            Ecb = ecb,
            AgentBodyLookup = bodyLookup,
            WorldStateLookup = stateLookup,
        }.Schedule(_initPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(initJh, state.Dependency);

        JobHandle mainJh = new FollowPathJob(){
            DataStore = _dataStore,
            PointsLookup = _pointsLookup,
            Ecb = ecb,
            AgentBodyLookup = bodyLookup,
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(mainJh, state.Dependency);
    }
}

[BurstCompile]
public partial struct InitFollowPathJob : IJobEntity {
    [ReadOnly] public ActionDataStore DataStore;
    [ReadOnly] public BufferLookup<PathPointElement> PointsLookup;
    public EntityCommandBuffer Ecb;
    public ComponentLookup<CharacterWorldStateComponent> WorldStateLookup;
    public ComponentLookup<AgentBody> AgentBodyLookup;


    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions, DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = DataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(new DynamicActionType(LocomotionActionType.FOLLOW_PATH),
                actions, targets, TargetType.TARGET_PATH, out ActiveActionElement activeAction, out Entity target,
                out int index)){
            if (PointsLookup.TryGetBuffer(target, out DynamicBuffer<PathPointElement> points)){
                if (!activeAction.HasStarted){
                    activeAction.HasStarted = true;
                    actions[index] = activeAction;
                    var state = WorldStateLookup[comp.CharacterEntity];
                    state.LocomotionState = LocomotionState.STANDING;
                    Ecb.SetComponent(comp.CharacterEntity, state);
                }
  
                var agentBody = AgentBodyLookup[comp.CharacterEntity];
                agentBody.SetDestination(points[0].Position);
                Ecb.SetComponent(comp.CharacterEntity, agentBody);
                
                actionUtils.StartPhase(new DynamicActionType(LocomotionActionType.FOLLOW_PATH), Ecb, e, 1);
            }
        }
    }
}

[BurstCompile]
public partial struct FollowPathJob : IJobEntity {
    [ReadOnly] public ActionDataStore DataStore;
    [ReadOnly] public BufferLookup<PathPointElement> PointsLookup;
    public ComponentLookup<AgentBody> AgentBodyLookup;
    
    public EntityCommandBuffer Ecb;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = DataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(new DynamicActionType(LocomotionActionType.FOLLOW_PATH),
                actions, targets, TargetType.TARGET_PATH, out ActiveActionElement activeAction, out Entity target,
                out int index)){
            if (PointsLookup.TryGetBuffer(target, out DynamicBuffer<PathPointElement> points)){
                var atLastPoint = false;
                var agentBody = AgentBodyLookup[comp.CharacterEntity];
                
                if (agentBody.RemainingDistance <= 5){
                    var lastPoint = points.Length - 1;
                    for (var i = 0; i < points.Length; i++){
                        var pathPointElement = points[i];
                        if (Vector3.Distance(pathPointElement.Position, agentBody.Destination) <= 1){
                            if (i == lastPoint){
                                atLastPoint = true;
                            }
                            else{
                                agentBody.SetDestination(points[i + 1].Position);
                                Ecb.SetComponent(comp.CharacterEntity, agentBody);
                            }

                            break;
                        }
                    }
                }

                if (atLastPoint){
                    agentBody.Stop();
                    Ecb.SetComponent(comp.CharacterEntity, agentBody);
                    actionUtils.StartPhase(activeAction.ActionType, Ecb, e, -1);
                    actionUtils.CompleteAction(activeAction, actions, index);
                }
            }
        }
    }
}