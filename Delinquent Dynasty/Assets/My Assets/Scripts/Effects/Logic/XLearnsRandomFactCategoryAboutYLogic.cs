using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public struct XLearnsRandomFactCategoryAboutYLogic : IApplyActiveEffect {
    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> RandomComponent;

    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    [ReadOnly] public SkillUtils SkillUtils;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;

    [ReadOnly] public PassiveEffectsUtils SecondaryPassives;
    [ReadOnly] public DynamicBuffer<InterestElement> SecondaryInterest;
    [ReadOnly] public DynamicBuffer<WellnessElement> SecondaryWellness;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        switch (data.PrimaryEnumValue.KnowledgeType){
            case KnowledgeType.LAST_KNOWN_SKILL:
                //TODO: maybe just call existing logic struct like learn specific with params? do the same with learn random knowledge
                // skill
                var skill = RandomComponent.ValueRW.Random.NextInt(1, SkillUtils.SkillDataStore.SkillsCount + 1);

                if (SkillUtils.TryGetSkillElement((SkillType) skill, SkillsLookup[secondaryTarget],
                        out SkillElement skillInfo)){
                    KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                        LearningEntity = primaryTarget,
                        PrimaryTarget = secondaryTarget,
                        Source = sourceEntity,
                        KnowledgeType = KnowledgeType.LAST_KNOWN_SKILL,
                        PrimaryEnumValue = new DynamicGameEnum{
                            IntValue = skill,
                            Type = GameEnumType.SKILL_TYPE
                        },
                        IntValue = skillInfo.CurrentLevel
                    });
                }

                break;
            case KnowledgeType.LAST_KNOWN_ATTRIBUTE:
                //attribute
                // todo: store the number of attributes somewhere otherwise update this as the number increases. maybe a general data store for random info like this
                var attributeType = RandomComponent.ValueRW.Random.NextInt(1, 7);
                var attribute = SecondaryPassives.GetNaturalAndBonusAttributeTotal((AttributeType) attributeType);
                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = primaryTarget,
                    PrimaryTarget = secondaryTarget,
                    Source = sourceEntity,
                    KnowledgeType = KnowledgeType.LAST_KNOWN_ATTRIBUTE,
                    PrimaryEnumValue = new DynamicGameEnum{
                        IntValue = attributeType,
                        Type = GameEnumType.ATTRIBUTE_TYPE
                    },
                    IntValue = attribute
                });
                break;
            case KnowledgeType.LAST_KNOWN_WELLNESS:
                //wellness
                // todo: store the number of wellness stats somewhere otherwise update this as the number increases. maybe a general data store for random info like this
                var wellnessType = RandomComponent.ValueRW.Random.NextInt(1, 8);
                var wellnessUtils = new WellnessUtils(){
                    Wellness = SecondaryWellness
                };
                var wellnessVal = wellnessUtils.GetCurrentValue((WellnessType) wellnessType);
                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = primaryTarget,
                    PrimaryTarget = secondaryTarget,
                    Source = sourceEntity,
                    KnowledgeType = KnowledgeType.LAST_KNOWN_WELLNESS,
                    PrimaryEnumValue = new DynamicGameEnum{
                        IntValue = wellnessType,
                        Type = GameEnumType.WELLNESS_TYPE
                    },
                    IntValue = wellnessVal
                });
                break;
            case KnowledgeType.LAST_KNOWN_TRAIT:
                //trait
                var validTraits = new FixedList512Bytes<DynamicTraitType>();
                foreach (var trait in SecondaryPassives.Traits){
                    var type = trait.TraitType;
                    var isStatus = type.Category == TraitCategory.STATUS;
                    var isDefault = type.Category == TraitCategory.DEFAULT;
                    if (!isStatus && !isDefault){
                        validTraits.Add(type);
                    }
                }

                if (validTraits.Length > 0){
                    var rand = RandomComponent.ValueRW.Random.NextInt(0, validTraits.Length);
                    var intensity = SecondaryPassives.GetTotalTraitIntensity(validTraits[rand]);
                    var trait = validTraits[rand];
                    var primaryEnumVal = new DynamicGameEnum();
                    primaryEnumVal.IntValue = trait.TypeValue;

                    switch (trait.Category){
                        case TraitCategory.PERSONALITY:
                            primaryEnumVal.Type = GameEnumType.PERSONALITY_TRAIT_CATEGORY;
                            break;
                        case TraitCategory.PERSONAL_MAINTENANCE:
                            primaryEnumVal.Type = GameEnumType.PERSONAL_MAINTENANCE_TRAIT_CATEGORY;
                            break;
                        case TraitCategory.GENETIC:
                            primaryEnumVal.Type = GameEnumType.GENETIC_TRAIT_CATEGORY;
                            break;
                    }

                    KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                        LearningEntity = primaryTarget,
                        PrimaryTarget = secondaryTarget,
                        Source = sourceEntity,
                        KnowledgeType = KnowledgeType.LAST_KNOWN_TRAIT,
                        PrimaryEnumValue = primaryEnumVal,
                        IntValue = intensity
                    });
                }

                break;
            case KnowledgeType.LAST_KNOWN_INTEREST:
                //interest
                if (SecondaryInterest.Length > 0){
                    var randInterest = RandomComponent.ValueRW.Random.NextInt(0, SecondaryInterest.Length);
                    var randInterestElement = SecondaryInterest[randInterest];
                    KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                        LearningEntity = primaryTarget,
                        PrimaryTarget = secondaryTarget,
                        Source = sourceEntity,
                        KnowledgeType = KnowledgeType.LAST_KNOWN_INTEREST,
                        PrimaryEnumValue = new DynamicGameEnum{
                            IntValue = (int) randInterestElement.SubjectType,
                            Type = GameEnumType.INTEREST_SUBJECT_TYPE
                        },
                        SecondaryEnumValue = randInterestElement.EnumValue,
                        IntValue = randInterestElement.InterestValue
                    });
                }

                break;
        }
    }
}