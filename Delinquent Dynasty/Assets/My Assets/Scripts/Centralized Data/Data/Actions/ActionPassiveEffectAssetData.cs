using Unity.Collections;

public struct ActionPassiveEffectAssetData {
    public FixedString64Bytes PassiveTitle;
    public int SkillLevelRequirement;
    public NumericComparisonSign SkillLevelComparisonSign;
    public PassiveEffectAssetData PassiveEffectAssetData;
    public bool NoSkillRequired => SkillLevelRequirement <= 0;
}