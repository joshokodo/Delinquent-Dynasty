using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct SetItemSellingJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    public bool SetValue;
    public DynamicActionType ActionType;

    public ComponentLookup<ItemSellableComponent> SellableLookup;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, DynamicBuffer<ActiveActionElement> actions, DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargets(ActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var targetItems
                = actionUtils.GetAllTargetData(TargetType.TARGET_ITEM, activeTargets);

            foreach (var item in targetItems){
                if (SellableLookup.TryGetComponent(item.TargetEntity,
                        out ItemSellableComponent sellableComponent)){
                    sellableComponent.ForSell = SetValue;
                    Ecb.SetComponent(item.TargetEntity, sellableComponent);
                }
            }

            if (NextPhase < 0){
                actionUtils.StartPhase(ActionType, Ecb, e, -1);
                actionUtils.CompleteAction(act, actions, index);
            }
            else{
                actionUtils.StartPhase(ActionType, Ecb, e, NextPhase);
            }
        }
    }
}