using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct TriggerCraftingRecipePrepJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public CraftingDataStore CraftingDataStore;

    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public ItemDataStore ItemDataStore;

    public EntityCommandBuffer Ecb;
    public BufferLookup<ItemElement> InventoryLookup;
    public ComponentLookup<ItemStackComponent> StackLookup;
    [ReadOnly] public ComponentLookup<ItemCraftingIngredientComponent> IngredientLookup;
    public DynamicBuffer<ItemSpawnElement> ItemSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var itemUtils = new ItemUtils{
            ItemDataStore = ItemDataStore
        };

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            // var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

            var ingredients = actionUtils.GetAllTargetData(TargetType.TARGET_INGREDIENT, activeTargets);
            var targetInventory = actionUtils.GetTargetEntity(TargetType.TARGET_INVENTORY, activeTargets);
            var recipe = actionUtils.GetTargetEnum(TargetType.TARGET_RECIPE, activeTargets);

            var originItems = InventoryLookup[targetInventory];

            // destroy ingredients
            foreach (var ingredient in ingredients){
                if (itemUtils.RemoveItemFromInventoryBuffer(ingredient.TargetEntity, originItems, StackLookup,
                        Ecb, out Entity removedItemEntity,
                        ingredient.CountValue)){
                    if (removedItemEntity != Entity.Null){
                        Ecb.AddComponent(removedItemEntity, new TransformItemComponent(){
                            ItemType = default,
                        });
                    }
                }

                // create waste
                var ingredientComponent = IngredientLookup[ingredient.TargetEntity];
                if (!ingredientComponent.WasteProduced.IsNull){
                    itemUtils.TriggerCreateItemFor(targetInventory, ItemSpawnElements,
                        ingredientComponent.WasteProduced, ingredient.CountValue);
                }
            }


            // create wip item
            var recipeData = CraftingDataStore.CraftingBlobAssets.Value.GetRecipeData(recipe);
            itemUtils.TriggerCreateItemFor(targetInventory, ItemSpawnElements, recipeData.SuccessfulProduct,
                isWip: true);

            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = true,
                TargetsData = targetData
            });

            StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
                PerformedSuccessfulAction = true,
            });

            if (NextPhase < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(act, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
        }
    }
}

public struct TriggerCraftingRecipePrepUtil {
    public ActionDataStore ActionDataStore;
    public CraftingDataStore CraftingDataStore;
    public ItemDataStore ItemDataStore; // todo might not even need. if its worth it, check for this and other consumers

    public EntityCommandBuffer Ecb;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;
    public DynamicBuffer<ItemSpawnElement> ItemSpawnElements;

    public EntityQuery Query;
    private DynamicActionType _actionType;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private ComponentLookup<ItemStackComponent> _itemStackLookup;
    private ComponentLookup<ItemCraftingIngredientComponent> _ingredientLookup;
    private BufferLookup<ItemElement> _itemsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        _actionType = actionType;

        Query =
            state.GetEntityQuery(
                CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _itemStackLookup = state.GetComponentLookup<ItemStackComponent>();
        _ingredientLookup = state.GetComponentLookup<ItemCraftingIngredientComponent>();
        _itemsLookup = state.GetBufferLookup<ItemElement>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, _actionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _itemStackLookup.Update(ref state);
        _itemsLookup.Update(ref state);
        _ingredientLookup.Update(ref state);
    }

    public TriggerCraftingRecipePrepJob GetTriggerCraftingRecipePrepJob(int nextPhase){
        return new TriggerCraftingRecipePrepJob(){
            ActionDataStore = ActionDataStore,
            DynamicActionType = _actionType,
            CraftingDataStore = CraftingDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            Ecb = Ecb,
            NextPhase = nextPhase,
            ItemDataStore = ItemDataStore,
            InventoryLookup = _itemsLookup,
            StackLookup = _itemStackLookup,
            ItemSpawnElements = ItemSpawnElements,
            StateChangeSpawnElements = StateChangeSpawnElements,
            IngredientLookup = _ingredientLookup
        };
    }
}