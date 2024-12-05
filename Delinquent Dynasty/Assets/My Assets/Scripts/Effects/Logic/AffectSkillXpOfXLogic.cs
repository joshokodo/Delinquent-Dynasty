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

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        var number =
            PrimaryPassiveUtils.OnAffectSkillXp(data, nextIntValue, sourceEntity, primaryTarget, ActiveEffectsSpawn);

        var resultingSkillLevel = SkillUtils.AddXp(number,
            data.PrimaryEnumValue.SkillType, Skills, out bool leveledUp);
        
        primaryStateChange.SkillsChanged = true;
        if (leveledUp){
            primaryStateChange.SkillTypeLeveledUp = data.PrimaryEnumValue.SkillType;
            primaryStateChange.SkillHasLeveledUp = true;
        }

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