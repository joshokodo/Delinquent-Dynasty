using Unity.Entities;

public struct XHasAnyItemWithPropertyConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var hasItem = false;
        if (utils.ItemsLookup.TryGetBuffer(target, out DynamicBuffer<ItemElement> inventory)){
            foreach (var itemElement in inventory){
                var itemType = utils.ItemBaseLookup[itemElement.ItemEntity].ItemType;
                if (utils.ItemDataStore.ItemBlobAssets.Value.HasItemProperty(itemType,
                        conditionData.PrimaryEnumValue.ItemProperty)){
                    hasItem = true;
                    break;
                }
            }
        }
   

        return hasItem == conditionData.ExpectedConditionValue;
    }
}