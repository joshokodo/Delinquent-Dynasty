using Unity.Collections;
using Unity.Entities;

public struct ItemCraftingToolComponent : IComponentData {
    public FixedList64Bytes<CraftingToolType> ToolTypes;
}