public struct TechXHasBatteryLifeValueConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (utils.ItemCellPhoneLookup.TryGetComponent(targetItem, out ItemCellPhoneComponent cell)){
            var value = (int) conditionData.PrimaryNumberValue;
            var batteryLife = cell.BatteryLife;
            return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, batteryLife, value) ==
                   conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}