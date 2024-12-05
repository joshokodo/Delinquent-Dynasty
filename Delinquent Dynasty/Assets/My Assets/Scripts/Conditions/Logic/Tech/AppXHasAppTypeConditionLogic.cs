public struct AppXHasAppTypeConditionLogic : IConditionCheck{
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var app = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var hasApp = false;
        if (utils.AppComponentLookup.TryGetComponent(app, out AppComponent appComp)){
            foreach (var appType in appComp.AppTypes){
                if (conditionData.PrimaryEnumValue.AppType == appType){
                    hasApp = true;
                    break;
                }
            }
        }

        return hasApp == conditionData.ExpectedConditionValue;
    }
}