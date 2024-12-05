using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(ActionsGroup))]
[BurstCompile]
public partial struct UnequipItemActionSystem : ISystem {
    private EntityQuery _startPhaseQuery;
    private EntityQuery _mainPhaseQuery;
    private ActionDataStore _actionDataStore;
    private ItemDataStore _itemDataStore;
    private DynamicActionType _actionType;

    public void OnCreate(ref SystemState state){
        _actionType = new DynamicActionType(CommonItemActionType.UNEQUIP_ITEM);
        state.RequireForUpdate<ArmsActionFunction>();
        _startPhaseQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<ArmsActionFunction>());
        _mainPhaseQuery = state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<ArmsActionFunction>());


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
        var equipLookup = SystemAPI.GetComponentLookup<ItemEquippableComponent>();
        var itemBaseLookup = SystemAPI.GetComponentLookup<ItemBaseComponent>();
        var actionKnowledgeSpawnElements = SystemAPI.GetSingletonBuffer<ActionKnowledgeSpawnElement>();
        var stateChangeSpawnElements = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();


        var h1 = new EquipActionFunctionPerformJob(){
            IsEquipping = false,
            ActionDataStore = _actionDataStore,
            Ecb = ecb.AsParallelWriter(),
            ItemDataStore = _itemDataStore,
            ItemBaseLookup = itemBaseLookup,
            PhaseAfterComplete = 1,
            TotalInGameSeconds = time.TotalInGameSeconds,
        }.Schedule(_startPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);

        var h2 = new UnequipItemMainThreadJob(){
            DynamicActionType = _actionType,
            Ecb = ecb,
            ActionDataStore = _actionDataStore,
            ItemDataStore = _itemDataStore,
            EquipLookup = equipLookup,
            BaseLookup = itemBaseLookup,
            ActionKnowledgeSpawnElements = actionKnowledgeSpawnElements,
            StateChangeSpawnElements = stateChangeSpawnElements
        }.Schedule(_mainPhaseQuery, state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(h2, state.Dependency);
    }
}

[BurstCompile]
public partial struct UnequipItemMainThreadJob : IJobEntity {
    public EntityCommandBuffer Ecb;
    public DynamicActionType DynamicActionType;
    public ComponentLookup<ItemEquippableComponent> EquipLookup;
    public ComponentLookup<ItemBaseComponent> BaseLookup;
    public ActionDataStore ActionDataStore;
    public ItemDataStore ItemDataStore;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
            var targetData = new TargetsGroup();
            targetData.SetTargets(e, activeTargets);

            var equipmentEntity = targetData.GetTargetEntity(TargetType.TARGET_ITEM);

            var equipment = EquipLookup[equipmentEntity];
            var equipmentBase = BaseLookup[equipmentEntity];

            equipment.isEquipped = false;

            Ecb.SetComponent(equipmentEntity, equipment);

            // todo do something
            var equipData = ItemDataStore.ItemBlobAssets.Value.GetItemEquippableData(equipmentBase.ItemType);

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });

            Ecb.SetComponentEnabled<UpdateCharacterModelComponent>(comp.CharacterEntity, true);

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