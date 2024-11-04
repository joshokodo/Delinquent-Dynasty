using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct ReloadRangeWeaponActionSystem : ISystem {
    private EntityQuery _performPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _dataStore;
    private ItemDataStore _itemDataStore;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.RELOAD_RANGED_WEAPON);
        state.RequireForUpdate<ArmsActionFunction>();
        // TODO: if start and main query are the same this actiond doesnt do anything. seems like if you have the same type of query, even a sharedComponent filter change doesnt
        // differentiate. confirm this but for now just making the querys slighty different makes it work. seems like there is a lot to queries i dont know, learn!
        _performPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<ArmsActionFunction>());
        _mainPhaseQuery = state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<ArmsActionFunction>());

        _performPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType}});
        _mainPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _dataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var time = SystemAPI.GetSingleton<InGameTime>();
        var parallelWriter = ecb.AsParallelWriter();

        var itemElements = SystemAPI.GetBufferLookup<ItemElement>();
        var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        var rangedWeaponLookup = SystemAPI.GetComponentLookup<ItemRangedWeaponComponent>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();

        var h1 = new RangedWeaponReloadActionFunctionPerformJob(){
            DynamicActionType = _actionType,
            Ecb = parallelWriter,
            TotalInGameSeconds = time.TotalInGameSeconds,
            ActionDataStore = _dataStore,
            PhaseAfterComplete = 1,
            ItemBaseLookup = itemBaseLookup,
            ItemDataStore = _itemDataStore,
        }.Schedule(_performPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new ReloadWeaponResultJob(){
            DynamicActionType = _actionType,
            Ecb = ecb,
            ActionDataStore = _dataStore,
            InventoryLookup = itemElements,
            ItemDataStore = _itemDataStore,
            RangedWeaponLookup = rangedWeaponLookup,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            ItemBaseLookup = itemBaseLookup,
            StateChangeSpawnElements = stateChangeSpawnElements
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}

[BurstCompile]
public partial struct RangedWeaponReloadActionFunctionPerformJob : IJobEntity {
    public DynamicActionType DynamicActionType;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public double TotalInGameSeconds;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public ItemDataStore ItemDataStore;
    public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<ItemBaseComponent> ItemBaseLookup;

    public void Execute(Entity e, [EntityIndexInQuery] int sortKey, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(DynamicActionType, actions, targets, TargetType.TARGET_ITEM,
                out ActiveActionElement activeAction, out Entity target,
                out int index)){
            var reloadTime = ItemDataStore.ItemBlobAssets.Value
                .GetItemRangedWeaponData(ItemBaseLookup[target].ItemType).ReloadTime;

            actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, reloadTime, Ecb, sortKey, e,
                PhaseAfterComplete);
        }
    }
}

[BurstCompile]
public partial struct ReloadWeaponResultJob : IJobEntity {
    public DynamicActionType DynamicActionType;
    public EntityCommandBuffer Ecb;
    public BufferLookup<ItemElement> InventoryLookup;
    public ComponentLookup<ItemRangedWeaponComponent> RangedWeaponLookup;
    public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        var itemUtils = new ItemUtils{
            ItemDataStore = ItemDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            var targetItem =
                targetData.GetTargetEntity(TargetType.TARGET_ITEM);

            var targetAmmo =
                targetData.GetTargetEntity(TargetType.TARGET_AMMO);

            var inventory = InventoryLookup[e];

            var weaponComponent = RangedWeaponLookup[targetItem];

            if (weaponComponent.LoadedAmmo.ItemEntity != Entity.Null){
                inventory.Add(weaponComponent.LoadedAmmo);
                weaponComponent.LoadedAmmo = default; // TODO: do we neeed?
            }

            if (itemUtils.RemoveItemElementFromInventoryBuffer(targetAmmo, inventory, out ItemElement newAmmo)){
                weaponComponent.LoadedAmmo = newAmmo;
                Ecb.SetComponent(targetItem, weaponComponent);
            }

            var weaponData =
                ItemDataStore.ItemBlobAssets.Value.GetItemRangedWeaponData(ItemBaseLookup[targetItem].ItemType);

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });

            StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
                InteractableChanged = true,
            });

            act.HasCompletedIteration = true;
            actions[index] = act;
        }
    }
}