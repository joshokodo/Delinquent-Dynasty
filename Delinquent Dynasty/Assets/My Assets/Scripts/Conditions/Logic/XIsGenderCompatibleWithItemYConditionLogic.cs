public struct XIsGenderCompatibleWithItemYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var targetItem = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        var itemType = utils.ItemBaseLookup[targetItem].ItemType;
        var isCompatible = utils.ItemDataStore.ItemBlobAssets.Value.HasEquippable(itemType)
                           && utils.ItemDataStore.ItemBlobAssets.Value.GetItemEquippableData(itemType).IsFemale ==
                           utils.CharacterBioLookup[target]
                               .IsFemale;
        return isCompatible == conditionData.ExpectedConditionValue;
    }
}