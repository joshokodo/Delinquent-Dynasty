using Unity.Collections;
using Unity.Entities;

public struct XLearnsSkillLogic : IApplyActiveEffect {
    [ReadOnly] public SkillUtils SkillUtils;
    public DynamicBuffer<SkillElement> Skills;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        SkillUtils.AddSkill(Skills, data.PrimaryEnumValue.SkillType,
            1, nextIntValue);
    }
}