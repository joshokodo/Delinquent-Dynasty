public struct ActionActiveEffectAssetData {
    public int SkillLevelRequirement;
    public NumericComparisonSign SkillLevelComparisonSign;
    public ActiveEffectAssetData EffectAssetData;
    public bool NoSkillRequired => SkillLevelRequirement <= 0;
}