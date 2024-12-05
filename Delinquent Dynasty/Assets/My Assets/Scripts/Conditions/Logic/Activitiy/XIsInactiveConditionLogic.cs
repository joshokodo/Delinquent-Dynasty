public struct XIsInactiveConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var actionUtils = new ActionUtils(){ActionDataStore = utils.ActionDataStore};
        // return !actionUtils.HasAnyActiveActions(plans, utils.ActionsLookup) == conditionData.BoolValue;
        return true;
    }
}