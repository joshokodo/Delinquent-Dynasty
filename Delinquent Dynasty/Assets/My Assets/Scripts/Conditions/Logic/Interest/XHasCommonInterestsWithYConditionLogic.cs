using Unity.Entities;

public struct XHasCommonInterestsWithYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);

        if (utils.InterestLookup.TryGetBuffer(x, out DynamicBuffer<InterestElement> xInt)
            && utils.InterestLookup.TryGetBuffer(y, out DynamicBuffer<InterestElement> yInt)){
            var count = 0;
            foreach (var xi in xInt){
                foreach (var yi in yInt){
                    if ((xi.InterestValue > 0 && yi.InterestValue > 0) || xi.InterestValue < 0 && yi.InterestValue < 0){
                        count++;
                    }
                }
            }

            return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, count,
                (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}