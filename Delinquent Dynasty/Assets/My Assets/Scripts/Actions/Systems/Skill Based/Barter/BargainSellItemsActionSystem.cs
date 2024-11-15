using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct BargainSellItemsActionSystem : ISystem {
    private PerformActionUtil _triggerPassives;
    private EntityQuery _startSellingQuery;
    private DynamicActionType _actionType;
    private ComponentLookup<ItemSellableComponent> _sellableLookup;
    private ComponentLookup<ItemBaseComponent> _itemBaseLookup;
    private BufferLookup<ItemElement> _itemsLookup;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(SkillBasedItemActionType.BARGAIN_SELL_ITEMS);
        state.RequireForUpdate<ArmsLegsActionFunction>();
        _triggerPassives.SetUp<ArmsLegsActionFunction>(ref state, _actionType, 0);

        _startSellingQuery =
            state.GetEntityQuery(CommonSystemUtils
                .BuildCommonActionTargetsFunctionPerformQuery<ArmsLegsActionFunction>());

        _startSellingQuery.SetSharedComponentFilter(new ArmsLegsActionFunction(){
            State = new ActionFunctionBase(){
                ActionType = _actionType, Phase = 1
            }
        });

        _sellableLookup = state.GetComponentLookup<ItemSellableComponent>();
        _itemBaseLookup = state.GetComponentLookup<ItemBaseComponent>();
        _itemsLookup = state.GetBufferLookup<ItemElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        var itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();

        _triggerPassives.ActionDataStore = actionDataStore;
        _triggerPassives.Ecb = ecb;
        _triggerPassives.PassiveEffectsSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        _triggerPassives.UpdateBufferLookups(ref state);
        _sellableLookup.Update(ref state);
        _itemBaseLookup.Update(ref state);
        _itemsLookup.Update(ref state);

        var passiveJob = _triggerPassives.GetPerformActionJob(1);

        var h1 = passiveJob.Schedule(_triggerPassives.Query, state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        // var h2 = new BargainSellItemsJob(){
        //     Ecb = ecb,
        //     ActionType = _actionType,
        //     SellableLookup = _sellableLookup,
        //     ActionDataStore = actionDataStore,
        //     NextPhase = -1,
        //     ItemBaseLookup = _itemBaseLookup,
        //     ItemDataStore = itemDataStore,
        //     ItemsLookup = _itemsLookup,
        // }.Schedule(_startSellingQuery, state.Dependency);
        // state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}

[BurstCompile]
public partial struct BargainSellItemsJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public ItemDataStore ItemDataStore;
    public DynamicActionType ActionType;
    public BufferLookup<ItemElement> ItemsLookup;

    public ComponentLookup<ItemSellableComponent> SellableLookup;
    public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, LocalTransform localTransform){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        var inventory = ItemsLookup[e];
        foreach (var itemElement in inventory){
            if (SellableLookup.TryGetComponent(itemElement.ItemEntity, out ItemSellableComponent sellableComponent) &&
                sellableComponent.ForSell){
                var type = ItemBaseLookup[itemElement.ItemEntity].ItemType;
                var data = ItemDataStore.ItemBlobAssets.Value.GetItemSellableData(type);
                var streetValue = data.GetStreetValue(sellableComponent.StreetValueModifiers, out _);
                sellableComponent.SellValue = streetValue < data.BaseSellValue ? streetValue : data.BaseSellValue;
                Ecb.SetComponent(itemElement.ItemEntity, sellableComponent);
            }
        }

        Ecb.AddComponent(e, new CommercableLocationComponent(){
            IsCharacter = true,
            IsSelling = true
        });

        Ecb.AddComponent(e, new InteractableLocationComponent(){
            Location = localTransform.Position,
            InteractableType = InteractableType.CHARACTER
        });

        Ecb.AddComponent<InteractableInventoryComponent>(e);

        actionUtils.StartPhase(ActionType, Ecb, e, NextPhase);
    }
}