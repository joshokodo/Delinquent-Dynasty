public struct XIsCharacterTypeConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (utils.CharacterBioLookup.TryGetComponent(target, out CharacterBio bio)){
            return (bio.CharacterType == conditionData.PrimaryEnumValue.CharacterType) ==
                   conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}