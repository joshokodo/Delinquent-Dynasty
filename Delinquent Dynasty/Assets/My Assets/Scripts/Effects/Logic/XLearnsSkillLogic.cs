using Unity.Collections;
using Unity.Entities;

public struct XLearnsSkillLogic : IApplyActiveEffect {
    [ReadOnly] public SkillUtils SkillUtils;
    public DynamicBuffer<SkillElement> Skills;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        
        var added = SkillUtils.AddSkill(Skills, data.PrimaryEnumValue.SkillType,
            1, nextIntValue);

        if (added){
            primaryStateChange.SkillsChanged = true;
            primaryStateChange.SkillLearned = true;
            primaryStateChange.LearnedSkillType = data.PrimaryEnumValue.SkillType;
        }
    }
}