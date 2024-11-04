using System;
using Unity.Collections;

public struct CraftingRecipeAssetData {
    public Guid Id;
    public SkillType RequiredSkillType;
    public int RequiredSkillLevel;
    public CraftingLocationType RequiredLocation;

    // todo check all data with double fields. do all of them need to be doubles? we can make some ints im sure
    public double BuildIterationTime;
    public double PrepTime;
    public double ExpireTime;

    public DynamicItemType SuccessfulProduct;
    public DynamicItemType FailedProduct;
    public DynamicItemType ExpiredProduct;

    public FixedList512Bytes<CraftingCompoundData> Ingredients;
    public FixedList512Bytes<CraftingCompoundData> PrepTools;
    public FixedList512Bytes<CraftingCompoundData> BuildTools;
}