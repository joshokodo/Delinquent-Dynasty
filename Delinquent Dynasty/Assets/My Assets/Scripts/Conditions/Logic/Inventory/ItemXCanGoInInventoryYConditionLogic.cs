public struct ItemXCanGoInInventoryYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var targetInventory = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        if (utils.ItemBaseLookup.TryGetComponent(targetItem, out ItemBaseComponent itemBaseComponent)){
            var canGo = false;
            if (utils.CharacterBioLookup.HasComponent(targetInventory)){
                canGo = true;
            }
            else if (utils.InteractableInventoryLookup.TryGetComponent(targetInventory,
                         out InteractableInventoryComponent viewableInventoryComponent)){
                if (viewableInventoryComponent.AllowedItemProperties.IsEmpty){
                    canGo = true;
                }
                else{
                    foreach (var componentDataAllowedItemProperty in viewableInventoryComponent.AllowedItemProperties){
                        if (utils.ItemDataStore.ItemBlobAssets.Value.HasItemProperty(itemBaseComponent.ItemType,
                                componentDataAllowedItemProperty)){
                            canGo = true;
                            break;
                        }
                    }
                }
            }
            else if (utils.ItemInventoryCompLookup.TryGetComponent(targetInventory,
                         out ItemInventoryComponent itemInventoryComponent)){
                if (itemInventoryComponent.AllowedItemProperties.IsEmpty){
                    canGo = true;
                }
                else{
                    foreach (var componentDataAllowedItemProperty in itemInventoryComponent.AllowedItemProperties){
                        if (utils.ItemDataStore.ItemBlobAssets.Value.HasItemProperty(itemBaseComponent.ItemType,
                                componentDataAllowedItemProperty)){
                            canGo = true;
                            break;
                        }
                    }
                }
            }

            return conditionData.ExpectedConditionValue == canGo;
        }

        return !conditionData.ExpectedConditionValue;
    }
}