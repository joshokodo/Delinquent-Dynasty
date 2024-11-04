using System;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TriggerNextActionPhaseJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;

    public DynamicActionType DynamicActionType;
    public int NextPhase;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute(Entity e, [EntityIndexInQuery] int sortKey){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        actionUtils.StartPhase(DynamicActionType, Ecb, sortKey, e, NextPhase);
    }
}

public struct TriggerNextActionPhaseUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public EntityQuery Query;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public TriggerNextActionPhaseJob GetTriggerNextActionPhaseJob(int nextPhase){
        return new TriggerNextActionPhaseJob(){
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            NextPhase = nextPhase,
            Ecb = Ecb,
        };
    }
}