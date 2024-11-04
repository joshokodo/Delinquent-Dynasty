using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct EquipActionFunctionPerformJob : IJobEntity {
    public EntityCommandBuffer.ParallelWriter Ecb;
    public bool IsEquipping;
    public double TotalInGameSeconds;
    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;
    [ReadOnly] public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public int PhaseAfterComplete;

    public void Execute(Entity e, [EntityIndexInQuery] int sortKey, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionType = IsEquipping
            ? new DynamicActionType(CommonItemActionType.EQUIP_ITEM)
            : new DynamicActionType(CommonItemActionType.UNEQUIP_ITEM);

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(actionType, actions, targets, TargetType.TARGET_ITEM,
                out ActiveActionElement activeAction, out Entity target, out int index)){
            var itemType = ItemBaseLookup[target].ItemType;
            var equipData = ItemDataStore.ItemBlobAssets.Value.GetItemEquippableData(itemType);

            var performanceTime = IsEquipping ? equipData.EquipTime : equipData.UnEquipTime;


            actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, performanceTime, Ecb,
                sortKey, e, PhaseAfterComplete);
        }
    }
}