using Unity.Collections;
using Unity.Entities;

public struct ItemCraftingToolAssetData {
    public DynamicItemType ItemType;
    public FixedList64Bytes<CraftingToolType> ToolTypes;
}