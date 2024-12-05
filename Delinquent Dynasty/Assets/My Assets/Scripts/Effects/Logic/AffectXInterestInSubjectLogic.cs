using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct AffectXInterestInSubjectLogic : IApplyActiveEffect {
    public bool Display;
    public DynamicBuffer<InterestElement> PrimaryInterests;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;
    public InterestDataStore InterestDataStore;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveSpawn;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        var foundInterest = false;
        var value = 0;
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        for (int i = 0; i < PrimaryInterests.Length; i++){
            var element = PrimaryInterests[i];
            if (element.SubjectType == data.PrimaryEnumValue.InterestSubjectType
                && element.EnumValue.IntValue == data.SecondaryEnumValue.IntValue){
                element.Add(nextIntValue);
                value = element.InterestValue;
                PrimaryInterests[i] = element;
                foundInterest = true;
                break;
            }
        }

        if (!foundInterest){
            PrimaryInterests.Add(new InterestElement(){
                SubjectType = data.PrimaryEnumValue.InterestSubjectType,
                EnumValue = data.SecondaryEnumValue,
                InterestValue = nextIntValue,
            });
            value = nextIntValue;

            // todo: instead of adding all of the passives, since we are planning on making interest sections based, only add passives based on section in logic maybe (e.i. set passives for interest 50% - 75%)
            InterestDataStore.InterestBlobAssets.Value
                .SetPassiveEffects(primaryTarget, PassiveSpawn, data.PrimaryEnumValue.InterestSubjectType,
                    data.SecondaryEnumValue);
        }
        
        primaryStateChange.InterestChanged = true;

        if (Display){
            FixedString128Bytes display = default;
            DisplayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                CharacterEntity = primaryTarget,
                DisplayEnum = data.SecondaryEnumValue,
                Value = nextIntValue,
                DisplayNumber = true,
                DisplayColor = nextIntValue >= 0 ? Color.green : Color.red
            });
        }

        if (data.SourceLearnsKnowledge){
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = sourceEntity,
                KnowledgeType = KnowledgeType.LAST_KNOWN_INTEREST,
                PrimaryTarget = primaryTarget,
                PrimaryEnumValue = data.PrimaryEnumValue,
                SecondaryEnumValue = data.SecondaryEnumValue,
                IntValue = value
            });
        }
    }
}