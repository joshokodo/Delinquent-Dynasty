using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ItemXHasPropertyConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var hasProp = utils.ItemDataStore.ItemBlobAssets.Value
            .HasItemProperty(utils.ItemBaseLookup[targetItem].ItemType, conditionData.PrimaryEnumValue.ItemProperty);
        return hasProp == conditionData.ExpectedConditionValue;
    }
}