using Unity.Collections;
using Unity.Entities;

//TODO: should we just keep data in store and search for it or keep here and make data quicker available at the cost of space?
// weigh the options and do the same for others like tools. maybe go this route for all andwe store more data in components for less store lookup
public struct ItemCraftingIngredientComponent : IComponentData {
    public FixedList64Bytes<CraftingIngredientType> IngredientTypes;
    public DynamicItemType WasteProduced;
}