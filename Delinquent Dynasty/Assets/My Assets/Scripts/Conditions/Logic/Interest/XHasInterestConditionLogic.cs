using UnityEngine;

public struct XHasInterestConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        foreach (var interestElement in utils.InterestLookup[target]){
            if (interestElement.SubjectType == conditionData.PrimaryEnumValue.InterestSubjectType
                && interestElement.EnumValue.Matches(conditionData.SecondaryEnumValue)){
                return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign,
                           interestElement.InterestValue, (int) conditionData.PrimaryNumberValue) ==
                       conditionData.ExpectedConditionValue;
            }
        }

        return !conditionData.ExpectedConditionValue;
    }
}