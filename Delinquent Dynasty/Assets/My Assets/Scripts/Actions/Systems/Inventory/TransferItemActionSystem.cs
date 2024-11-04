using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct TransferItemActionSystem : ISystem {
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _actionDataStore;
    private ItemDataStore _itemDataStore;
    private TraitDataStore _traitDataStore;
    private DynamicActionType _actionType;
    private PerformActionUtil _performActionUtil;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.TRANSFER_ITEM);
        state.RequireForUpdate<ArmsActionFunction>();

        _performActionUtil.SetUp<ArmsActionFunction>(ref state, _actionType, 0);
        _mainPhaseQuery =
            state.GetEntityQuery(
                CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<ArmsActionFunction>());

        _mainPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();
        _traitDataStore = SystemAPI.GetSingleton<TraitDataStore>();

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var time = SystemAPI.GetSingleton<InGameTime>();
        var inventoryLookup = SystemAPI.GetBufferLookup<ItemElement>();
        var stackLookup = SystemAPI.GetComponentLookup<ItemStackComponent>();
        var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        var itemSpawn = SystemAPI.GetSingletonBuffer<ItemSpawnElement>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();
        var effectSpawn = SystemAPI.GetSingletonBuffer<ActiveEffectSpawnElement>();
        var passiveSpawn = SystemAPI.GetSingletonBuffer<PassiveEffectSpawnElement>();

        _performActionUtil.Ecb = ecb;
        _performActionUtil.ActionDataStore = _actionDataStore;
        _performActionUtil.InGameTime = time;
        _performActionUtil.StateChangeSpawn = stateChangeSpawnElements;
        _performActionUtil.ActiveEffectsSpawn = effectSpawn;
        _performActionUtil.PassiveEffectsSpawn = passiveSpawn;

        _performActionUtil.UpdateBufferLookups(ref state);
        var performJob = _performActionUtil.GetPerformActionJob(1);

        var performJh = performJob.Schedule(_performActionUtil.Query, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(performJh, state.Dependency);

        var transferJh = new TransferItemJob(){
            DynamicActionType = _actionType,
            Ecb = ecb,
            InventoryLookup = inventoryLookup,
            ActionDataStore = _actionDataStore,
            StackLookup = stackLookup,
            ItemBaseLookup = itemBaseLookup,
            ItemSpawnElements = itemSpawn,
            ItemDataStore = _itemDataStore,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            StateChangeSpawnElements = stateChangeSpawnElements
        }.Schedule(_mainPhaseQuery, performJh);

        state.Dependency = JobHandle.CombineDependencies(transferJh, state.Dependency);
    }
}


// TODO: this and other jobs without their own file should be moved to dedicated files. or is it that big of a deal?
// TODO: MAYBE IT SHOULD BE THE OTHER WAY AROUND?! find all the separete job files that are only being used once in one place and just move them there?
[BurstCompile]
public partial struct TransferItemJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public ItemDataStore ItemDataStore;

    public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public EntityCommandBuffer Ecb;
    public BufferLookup<ItemElement> InventoryLookup;
    public ComponentLookup<ItemStackComponent> StackLookup;
    public DynamicBuffer<ItemSpawnElement> ItemSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };


        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            var targetInventoryEntity =
                targetData.GetTargetEntity(TargetType.TARGET_INVENTORY);

            var targetItem =
                actionUtils.GetTargetData(TargetType.TARGET_ITEM, activeTargets);

            var originInventoryEntity = targetData.GetTargetEntity(TargetType.ORIGIN_INVENTORY);

            var originItems = InventoryLookup[originInventoryEntity];
            var itemUtils = new ItemUtils{ItemDataStore = ItemDataStore};
            var targetItemType = ItemBaseLookup[targetItem.TargetEntity].ItemType;

            if (itemUtils.RemoveItemFromInventoryBuffer(targetItem.TargetEntity, originItems, StackLookup,
                    Ecb, out Entity removedItemEntity,
                    targetItem.CountValue)){
                if (removedItemEntity != Entity.Null){
                    InventoryLookup[targetInventoryEntity].Add(new ItemElement(){
                        ItemEntity = removedItemEntity
                    });

                    var baseItem = ItemBaseLookup[removedItemEntity];
                    baseItem.InventoryEntity = comp.CharacterEntity;
                    Ecb.SetComponent(removedItemEntity, baseItem);
                }
                else{
                    itemUtils.TriggerCreateItemFor(targetInventoryEntity, ItemSpawnElements, targetItemType,
                        targetItem.CountValue);
                }
            }

            StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
                PerformedSuccessfulAction = true,
            });

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });

            act.HasCompletedIteration = true;
            actions[index] = act;
        }
    }
}