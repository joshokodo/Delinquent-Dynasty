using System;
using System.Collections;
using System.Collections.Generic;
using DistantLands;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ItemActionFunctionPerformJob : IJobEntity {
    public DynamicActionType ActionType;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public double TotalInGameSeconds;

    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;

    public int NextPhase;
    [ReadOnly] public ComponentLookup<ItemBaseComponent> ItemBaseLookup;

    public void Execute(Entity e, [EntityIndexInQuery] int sortKey, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore,
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(ActionType, actions, targets, TargetType.TARGET_ITEM,
                out ActiveActionElement activeAction, out Entity target, out int index)){
            var actData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(ActionType);

            var itemType = ItemBaseLookup[target].ItemType;

            var performTime = actData.PerformTime;

            if (ActionType.Matches(new DynamicActionType(CommonItemActionType.READ_ITEM))){
                performTime += ItemDataStore.ItemBlobAssets.Value.GetItemReadData(itemType).PerformTime;
            }
            else if (ActionType.Matches(new DynamicActionType(CommonItemActionType.CONSUME_ITEM))){
                performTime += ItemDataStore.ItemBlobAssets.Value.GetItemConsumeData(itemType).PerformTime;
            }

            new ActionUtils(){
                ActionDataStore = ActionDataStore
            }.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, performTime, Ecb, sortKey, e,
                NextPhase);
        }
    }
}

public struct ItemActionFunctionPerformUtil {
    public EntityQuery Query;
    private DynamicActionType _actionType;

    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;
    public EntityCommandBuffer Ecb;
    public InGameTime InGameTime;

    private ComponentLookup<ItemBaseComponent> _itemBaseLookup;
    private ComponentLookup<ItemStackComponent> _itemStackLookup;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;


    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        _actionType = actionType;

        Query =
            state.GetEntityQuery(
                CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _itemBaseLookup = state.GetComponentLookup<ItemBaseComponent>();

        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, _actionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _itemBaseLookup.Update(ref state);
    }

    public ItemActionFunctionPerformJob GetItemActionFunctionPerformJob(int nextPhase){
        return new ItemActionFunctionPerformJob(){
            ActionDataStore = ActionDataStore,
            Ecb = Ecb.AsParallelWriter(),
            NextPhase = nextPhase,
            ItemDataStore = ItemDataStore,
            ItemBaseLookup = _itemBaseLookup,
            TotalInGameSeconds = InGameTime.TotalInGameSeconds,
            ActionType = _actionType,
        };
    }
}