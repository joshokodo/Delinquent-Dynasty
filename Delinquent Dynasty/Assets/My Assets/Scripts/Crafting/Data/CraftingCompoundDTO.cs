using System;

[Serializable]
public struct CraftingCompoundDTO {
    public DynamicItemTypeDTO ItemType;
    public CraftingIngredientType IngredientType;
    public CraftingToolType ToolType;
    public int Count;

    public CraftingCompoundData ToData(){
        return new CraftingCompoundData(){
            Count = Count,
            ItemType = ItemType.ToData(),
            IngredientType = IngredientType,
            ToolType = ToolType,
        };
    }
}