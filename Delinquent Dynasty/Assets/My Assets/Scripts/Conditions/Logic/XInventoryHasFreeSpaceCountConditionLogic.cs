using Unity.Entities;

public struct XInventoryHasFreeSpaceCountConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);

        if (conditionData.UseGodVision){
            if (utils.ItemsLookup.TryGetBuffer(x, out DynamicBuffer<ItemElement> items)){
                var limit = 0;
                var carrying = items.Length;
                var space = 0;

                if (utils.CharacterBioLookup.TryGetComponent(x, out CharacterBio bio)){
                    limit = bio.CarryLimit;
                }
                else if (utils.InteractableInventoryLookup.TryGetComponent(x, out InteractableInventoryComponent inv)){
                    limit = inv.CarryLimit;
                }
                else if (utils.ItemInventoryCompLookup.TryGetComponent(x, out ItemInventoryComponent itemInv)){
                    limit = itemInv.CarryLimit;
                }

                if (limit == 0){
                    space = -1;
                }
                else if (carrying < limit){
                    space = limit - carrying;
                }

                return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, space,
                    (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
            }

            return !conditionData.ExpectedConditionValue;
        }
        else{ }

        return !conditionData.ExpectedConditionValue;
    }
}