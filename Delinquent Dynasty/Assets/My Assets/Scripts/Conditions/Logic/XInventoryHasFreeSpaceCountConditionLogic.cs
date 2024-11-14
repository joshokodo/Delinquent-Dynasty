using System;
using Unity.Entities;

public struct XInventoryHasFreeSpaceCountConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);

        if (conditionData.UseGodVision){
            if (utils.ItemsLookup.TryGetBuffer(x, out DynamicBuffer<ItemElement> items)){
                var limit = 0;
                var carrying = items.Length;
                var space = 0;
                
                if (utils.InteractableInventoryLookup.TryGetComponent(x, out InteractableInventoryComponent inv)){
                    limit = inv.CarryLimit;
                }
                else if (utils.ItemInventoryCompLookup.TryGetComponent(x, out ItemInventoryComponent itemInv)){
                    limit = itemInv.CarryLimit;
                }

                if (limit == 0){
                    limit = Int32.MaxValue;
                }
                
                if (carrying < limit){
                    space = carrying - limit;
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