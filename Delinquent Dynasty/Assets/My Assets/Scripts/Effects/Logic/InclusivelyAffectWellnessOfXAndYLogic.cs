using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public struct InclusivelyAffectWellnessOfXAndYLogic : IApplyActiveEffect {
    [ReadOnly] public bool Display;
    [ReadOnly] public PassiveEffectsUtils PrimaryPassivesUtil;
    [ReadOnly] public PassiveEffectsUtils SecondaryPassivesUtil;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> RandomComponent;

    public DynamicBuffer<WellnessElement> PrimaryWellness;
    public DynamicBuffer<WellnessElement> SecondaryWellness;
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
        
        primaryStateChange.WellnessChanged = true;
        secondaryStateChange.WellnessChanged = true;
        
        number = PrimaryPassivesUtil.OnAffectOtherWellness(data, data.PrimaryEnumValue.WellnessType, number,
            primaryTarget, secondaryTarget, ActiveEffectsSpawn);

        number = SecondaryPassivesUtil.OnWellnessAffected(data, number, primaryTarget, secondaryTarget,
            ActiveEffectsSpawn,
            RandomComponent, out int secondaryTotalBonus);

        var secondaryBonusMax =
            SecondaryPassivesUtil.GetBonusWellnessMaxTotal(data.PrimaryEnumValue.WellnessType);

        var secondaryWellnessUtils = new WellnessUtils(){
            Wellness = SecondaryWellness
        };
        var secondaryCurrentValue =
            secondaryWellnessUtils.Affect(data.PrimaryEnumValue.WellnessType, number, secondaryBonusMax);

        if (Display){
            NumberUtils.SetWellnessDisplay(number, secondaryTotalBonus, isDamage, isRestore, secondaryTarget, data,
                DisplayDamageSpawn);
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = primaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_WELLNESS,
                PrimaryTarget = secondaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = secondaryCurrentValue
            });
        }

        number = nextIntValue;
        number = SecondaryPassivesUtil.OnAffectOtherWellness(data,
            data.PrimaryEnumValue.WellnessType, number, secondaryTarget, primaryTarget, ActiveEffectsSpawn);

        number = PrimaryPassivesUtil.OnWellnessAffected(data, number, secondaryTarget, primaryTarget,
            ActiveEffectsSpawn,
            RandomComponent, out int totalBonus);

        var primaryBonusMax =
            PrimaryPassivesUtil.GetBonusWellnessMaxTotal(data.PrimaryEnumValue.WellnessType);

        var primaryWellnessUtils = new WellnessUtils(){
            Wellness = PrimaryWellness
        };
        var primaryCurrentValue =
            primaryWellnessUtils.Affect(data.PrimaryEnumValue.WellnessType, number, primaryBonusMax);

        if (Display){
            NumberUtils.SetWellnessDisplay(number, primaryBonusMax, isDamage, isRestore, primaryTarget, data,
                DisplayDamageSpawn);
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = secondaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_WELLNESS,
                PrimaryTarget = primaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = primaryCurrentValue
            });
        }
    }
}