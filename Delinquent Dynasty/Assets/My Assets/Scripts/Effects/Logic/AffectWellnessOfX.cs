using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct AffectWellnessOfX : IApplyActiveEffect {
    [ReadOnly] public bool Display;
    [ReadOnly] public bool CheckSourceForPassives;
    [ReadOnly] public PassiveEffectsUtils PrimaryPassivesUtil;
    [ReadOnly] public PassiveEffectsUtils SourcePassivesUtil;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> RandomComponent;

    public DynamicBuffer<WellnessElement> PrimaryWellness;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        Entity secondaryTarget = default){
        var number = nextIntValue;
        var isDamage = number < 0;
        var isRestore = number > 0;
        
        primaryStateChange = default;
        secondaryStateChange = default;

        if (CheckSourceForPassives){
            number = SourcePassivesUtil.OnAffectOtherWellness(data,
                data.PrimaryEnumValue.WellnessType, number, sourceEntity, primaryTarget, ActiveEffectsSpawn);
        }

        number = PrimaryPassivesUtil.OnWellnessAffected(data, number, sourceEntity, primaryTarget, ActiveEffectsSpawn,
            RandomComponent, out int totalBonus);

        var bonusMax =
            PrimaryPassivesUtil.GetBonusWellnessMaxTotal(data.PrimaryEnumValue.WellnessType);

        var wellnessUtils = new WellnessUtils(){
            Wellness = PrimaryWellness
        };
        var currentValue = wellnessUtils.Affect(data.PrimaryEnumValue.WellnessType, number, bonusMax);

        if (number != 0){
            primaryStateChange.WellnessChanged = true;
        }
        
        if (Display){
            NumberUtils.SetWellnessDisplay(number, totalBonus, isDamage, isRestore, primaryTarget, data,
                DisplayDamageSpawn);
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = sourceEntity,
                KnowledgeType = KnowledgeType.LAST_KNOWN_WELLNESS,
                PrimaryTarget = primaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = currentValue
            });
        }
    }
}