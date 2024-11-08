﻿using Unity.Entities;

public struct XHasAnyItemWithPropertyConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var inventory = utils.ItemsLookup[target];
        var hasItem = false;
        foreach (var itemElement in inventory){
            var itemType = utils.ItemBaseLookup[itemElement.ItemEntity].ItemType;
            if (utils.ItemDataStore.ItemBlobAssets.Value.HasItemProperty(itemType,
                    conditionData.PrimaryEnumValue.ItemProperty)
                && utils.MeetsAliasCheck(target, itemElement.ItemEntity, conditionData)){
                hasItem = true;
                break;
            }
        }

        return hasItem == conditionData.ExpectedConditionValue;
    }
}