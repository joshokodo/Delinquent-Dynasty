using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct EquipItemActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _actionDataStore;
    private ItemDataStore _itemDataStore;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.EQUIP_ITEM);
        state.RequireForUpdate<ArmsActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<ArmsActionFunction>());
        _mainPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<ArmsActionFunction>());

        _startPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType}});
        _mainPhaseQuery.SetSharedComponentFilter(new ArmsActionFunction()
            {State = new ActionFunctionBase(){ActionType = _actionType, Phase = 1}});
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        _actionDataStore = SystemAPI.GetSingleton<ActionDataStore>();
        _itemDataStore = SystemAPI.GetSingleton<ItemDataStore>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var time = SystemAPI.GetSingleton<InGameTime>();
        var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        var equipLookup = SystemAPI.GetComponentLookup<ItemEquippableComponent>();
        var itemsLookup = SystemAPI.GetBufferLookup<ItemElement>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();

        var h1 = new EquipActionFunctionPerformJob(){
            IsEquipping = true,
            ActionDataStore = _actionDataStore,
            Ecb = ecb.AsParallelWriter(),
            ItemDataStore = _itemDataStore,
            ItemBaseLookup = itemBaseLookup,
            PhaseAfterComplete = 1,
            TotalInGameSeconds = time.TotalInGameSeconds,
        }.Schedule(_startPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new EquipItemMainThreadJob(){
            DynamicActionType = _actionType,
            Ecb = ecb,
            ActionDataStore = _actionDataStore,
            ItemBaseLookup = itemBaseLookup,
            EquipLookup = equipLookup,
            ItemsLookup = itemsLookup,
            ItemDataStore = _itemDataStore,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            StateChangeSpawnElements = stateChangeSpawnElements
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}

[BurstCompile]
public partial struct EquipItemMainThreadJob : IJobEntity {
    public EntityCommandBuffer Ecb;
    public DynamicActionType DynamicActionType;
    public ComponentLookup<ItemEquippableComponent> EquipLookup;
    public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public BufferLookup<ItemElement> ItemsLookup;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;
    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        var itemUtils = new ItemUtils(){
            ItemDataStore = ItemDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            var equipmentEntity = targetData.GetTargetEntity(TargetType.TARGET_ITEM);
            var equipment = EquipLookup[equipmentEntity];
            var equipmentBase = ItemBaseLookup[equipmentEntity];

            equipment.isEquipped = true;

            Ecb.SetComponent(equipmentEntity, equipment);
            var characterInventory = ItemsLookup[e];

            foreach (var itemElement in characterInventory){
                if (itemElement.ItemEntity != equipmentEntity &&
                    EquipLookup.TryGetComponent(itemElement.ItemEntity, out ItemEquippableComponent nextEquip)){
                    if (itemUtils.HasConflictingEquipmentTypes(equipmentBase.ItemType,
                            ItemBaseLookup[itemElement.ItemEntity].ItemType)){
                        nextEquip.isEquipped = false;
                        Ecb.SetComponent(itemElement.ItemEntity, nextEquip);
                    }
                }
            }

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });

            Ecb.AddComponent(comp.CharacterEntity, new UpdateCharacterEquipmentTag());

            StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
                PerformedSuccessfulAction = true,
            });
            actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
            actionUtils.CompleteAction(activeAction, actions, index);
        }
    }
}