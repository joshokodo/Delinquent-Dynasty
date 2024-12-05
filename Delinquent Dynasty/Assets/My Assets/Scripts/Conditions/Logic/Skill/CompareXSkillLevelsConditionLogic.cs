public struct CompareXSkillLevelsConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        if (utils.CharacterBioLookup.HasComponent(x) && utils.CharacterBioLookup.HasComponent(y)){
            var xPassives = utils.CreatePassiveUtils(x);
            var yPassives = utils.CreatePassiveUtils(y);
            var xTotal = xPassives.GetNaturalAndBonusSkillLevel(conditionData.PrimaryEnumValue.SkillType);
            var yTotal = yPassives.GetNaturalAndBonusSkillLevel(conditionData.SecondaryEnumValue.SkillType);
            return conditionData.ExpectedConditionValue ==
                   NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, xTotal, yTotal);
        }

        return !conditionData.ExpectedConditionValue;
    }
}