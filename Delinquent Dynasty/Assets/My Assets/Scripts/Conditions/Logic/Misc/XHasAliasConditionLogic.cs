public struct XHasAliasConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);

        if (utils.InteractableLocationLookup.TryGetComponent(x, out InteractableLocationComponent locationComponent)){
            return conditionData.PrimaryStringValue.Equals(locationComponent.Alias) == conditionData.ExpectedConditionValue;
        }
        return !conditionData.ExpectedConditionValue;
    }
}