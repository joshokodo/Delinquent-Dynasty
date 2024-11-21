using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct InclusivelyAffectRelationshipStatForXAndYLogic : IApplyActiveEffect {

    public PassiveEffectsUtils PrimaryPassives;
    public PassiveEffectsUtils SecondaryPassives;

    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    [ReadOnly] public bool Display;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;
    public DynamicBuffer<PassiveEffectSpawnElement> PassivesSpawn;
    public CharactersDataStore CharacterDataStore;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        Entity secondaryTarget = default){
        var number = nextIntValue;
        var isGain = number > 0;
        var isLost = number < 0;
        var utils = new RelationshipUtils();
        
        primaryStateChange = default;
        secondaryStateChange = default;
        
        primaryStateChange.RelationshipChanged = true;
        secondaryStateChange.RelationshipChanged = true;

        number = PrimaryPassives.OnAffectOtherRelationshipStat(data, data.PrimaryEnumValue.RelationshipStatType, number,
            primaryTarget, secondaryTarget, ActiveEffectsSpawn);
        number = SecondaryPassives.OnRelationshipStatAffected(data, number, primaryTarget, secondaryTarget,
            ActiveEffectsSpawn, out int totalBonus);

        var resultingStatVal = utils.AffectRelationshipStat(secondaryTarget, primaryTarget,
            data.PrimaryEnumValue.RelationshipStatType, number, SecondaryPassives.Relationships, PassivesSpawn,
            CharacterDataStore, RelationshipMainTitleType.ACQUAINTANCE);

        if (Display){
            NumberUtils.SetRelationshipStatDisplay(number, totalBonus, isLost, isGain, secondaryTarget, data,
                DisplayDamageSpawn);
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = primaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                PrimaryTarget = secondaryTarget,
                SecondaryTarget = primaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = resultingStatVal
            });
        }

        number = nextIntValue;
        number = SecondaryPassives.OnAffectOtherRelationshipStat(data, data.PrimaryEnumValue.RelationshipStatType,
            number, secondaryTarget, primaryTarget, ActiveEffectsSpawn);
        number = PrimaryPassives.OnRelationshipStatAffected(data, number, secondaryTarget, primaryTarget,
            ActiveEffectsSpawn, out int secondTotalBonus);

        var secondResultingStatVal = utils.AffectRelationshipStat(primaryTarget, secondaryTarget,
            data.PrimaryEnumValue.RelationshipStatType, number, PrimaryPassives.Relationships, PassivesSpawn, CharacterDataStore,
            RelationshipMainTitleType.ACQUAINTANCE);

        if (Display){
            NumberUtils.SetRelationshipStatDisplay(number, secondTotalBonus, isLost, isGain, primaryTarget, data,
                DisplayDamageSpawn);
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = secondaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                PrimaryTarget = primaryTarget,
                SecondaryTarget = secondaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                IntValue = secondResultingStatVal
            });
        }
    }
}