public struct TechXIsOnConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (utils.ItemCellPhoneLookup.TryGetComponent(targetItem,
                out ItemCellPhoneComponent cell)){
            return cell.IsOn == conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}