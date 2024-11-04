using Unity.Collections;

public struct ItemCraftingIngredientAssetData {
    public DynamicItemType ItemType;
    public FixedList64Bytes<CraftingIngredientType> IngredientTypes;
    public DynamicItemType WasteProduced;
}