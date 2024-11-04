using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct UnlockSecurityLockActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _dataStore;
    private TraitDataStore _traitDataStore;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;


    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.UNLOCK_SECURITY_LOCK);
        state.RequireForUpdate<ArmsActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<ArmsActionFunction>());
        _mainPhaseQuery =
            state.GetEntityQuery(
                CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<ArmsActionFunction>());

        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();

        _startPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType}});
        _mainPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _traitDataStore = SystemAPI.GetSingleton<TraitDataStore>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var time = SystemAPI.GetSingleton<InGameTime>();
        var securityLocksLookup = SystemAPI.GetComponentLookup<ItemSecurityLockComponent>();
        var socketLookup = SystemAPI.GetComponentLookup<SecurityLockSocket>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var passivesLookup = SystemAPI.GetBufferLookup<PassiveEffectElement>();
        _passiveCompLookup.Update(ref state);


        var h1 = new PerformActionJob(){
            Ecb = ecb,
            TotalInGameSeconds = time.TotalInGameSeconds,
            DynamicActionType = _actionType,
            ActionDataStore = _dataStore,
            PhaseAfterComplete = 1,
            PassiveCompLookup = _passiveCompLookup,
            PassivesLookup = passivesLookup,
            StateChangeSpawn = stateChangeSpawnElements,
        }.Schedule(_startPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new UnlockSecurityLockMainThreadJob(){
            DynamicActionType = _actionType,
            Ecb = ecb,
            SecurityLockLookup = securityLocksLookup,
            SocketLookup = socketLookup,
            ActionDataStore = _dataStore,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            StateChangeSpawnElements = stateChangeSpawnElements
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}

[BurstCompile]
public partial struct UnlockSecurityLockMainThreadJob : IJobEntity {
    public DynamicActionType DynamicActionType;
    public EntityCommandBuffer Ecb;
    public ComponentLookup<ItemSecurityLockComponent> SecurityLockLookup;
    public ComponentLookup<SecurityLockSocket> SocketLookup;
    public ActionDataStore ActionDataStore;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore,
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var targetData = new TargetsGroup();
            targetData.SetTargets(e, activeTargets);

            var targetLock =
                targetData.GetTargetEntity(TargetType.TARGET_SECURITY_LOCK);

            if (SocketLookup.TryGetComponent(targetLock, out SecurityLockSocket lockItemSocket)){
                targetLock = lockItemSocket.LockEntity;
            }

            if (SecurityLockLookup.TryGetComponent(targetLock,
                    out ItemSecurityLockComponent lockComponent)){
                lockComponent.IsLocked = false;
                Ecb.SetComponent(targetLock, lockComponent);
            }

            StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
                InteractableChanged = true,
            });

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });
            actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
            actionUtils.CompleteAction(activeAction, actions, index);
        }
    }
}