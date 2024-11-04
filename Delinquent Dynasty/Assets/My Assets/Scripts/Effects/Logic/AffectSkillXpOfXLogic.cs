using Unity.Collections;
using Unity.Entities;

public struct AffectSkillXpOfXLogic : IApplyActiveEffect {
    [ReadOnly] public PassiveEffectsUtils PrimaryPassiveUtils;
    [ReadOnly] public bool Display;
    [ReadOnly] public SkillUtils SkillUtils;
    public DynamicBuffer<SkillElement> Skills;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        var number =
            PrimaryPassiveUtils.OnAffectSkillXp(data, nextIntValue, sourceEntity, primaryTarget, ActiveEffectsSpawn);

        var resultingSkillLevel = SkillUtils.AddXp(number,
            data.PrimaryEnumValue.SkillType, Skills);

        // if (Display){
        //     DisplayDamageSpawn.Add(new DisplayDamageSpawnElement(){
        //         CharacterEntity = primaryTarget,
        //         Text = new FixedString64Bytes(data.PrimaryEnumValue.SkillType.ToString()),
        //         Value = number
        //     });
        // }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = sourceEntity,
                KnowledgeType = KnowledgeType.LAST_KNOWN_SKILL,
                PrimaryTarget = primaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = resultingSkillLevel // todo if we ever do bonus skill levels get total level
            });
        }
    }
}