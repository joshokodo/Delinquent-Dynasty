using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct XConflictsfWithYLogic : IApplyActiveEffect {
    public DynamicBuffer<RelationshipElement> PrimaryRelationships;
    public DynamicBuffer<InterestElement> PrimaryInterest;
    public DynamicBuffer<WellnessElement> PrimaryWellness;
    public DynamicBuffer<CharacterAttributeElement> PrimaryAttributes;
    public DynamicBuffer<SkillElement> PrimarySkills;
    public PassiveEffectsUtils PrimaryPassives;

    public DynamicBuffer<RelationshipElement> SecondaryRelationships;
    public DynamicBuffer<InterestElement> SecondaryInterest;
    public DynamicBuffer<WellnessElement> SecondaryWellness;
    public DynamicBuffer<CharacterAttributeElement> SecondaryAttributes;
    public DynamicBuffer<SkillElement> SecondarySkills;
    public PassiveEffectsUtils SecondaryPassives;

    public bool Display;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplaySpawnElements;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> RandomComponent;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        var foundInterest = new FixedList4096Bytes<EffectCommonStatData>();
        var found = false;
        var limit = nextIntValue <= 1 ? 1 : nextIntValue;

        foreach (var pi in PrimaryInterest){
            foreach (var si in SecondaryInterest){
                if (pi.SubjectType == si.SubjectType && pi.EnumValue.Matches(si.EnumValue) && ((pi.InterestValue < 0 &&
                        si.InterestValue > 0) || (si.InterestValue < 0 &&
                                                  pi.InterestValue > 0))){
                    var absPi = Mathf.Abs(pi.InterestValue);
                    var absSi = Mathf.Abs(si.InterestValue);

                    var diff = Mathf.Max(absPi, absSi) - Mathf.Min(absPi, absSi);
                    if (diff <= 50){
                        found = true;

                        foundInterest.Add(new EffectCommonStatData(){
                            PrimaryEnum = new DynamicGameEnum(pi.SubjectType),
                            SecondaryEnum = pi.EnumValue,
                            ValueX = pi.InterestValue,
                            ValueY = si.InterestValue
                        });
                    }
                }
            }
        }

        foreach (var pa in PrimaryAttributes){
            foreach (var sa in SecondaryAttributes){
                if (pa.AttributeType == sa.AttributeType){
                    var primAtt = PrimaryPassives.GetNaturalAndBonusAttributeTotal(pa.AttributeType);
                    var secdAtt = SecondaryPassives.GetNaturalAndBonusAttributeTotal(sa.AttributeType);

                    var diff = Mathf.Max(primAtt, secdAtt) -
                               Mathf.Min(primAtt, secdAtt);
                    if (diff >= 5){
                        found = true;

                        foundInterest.Add(new EffectCommonStatData(){
                            PrimaryEnum = new DynamicGameEnum(pa.AttributeType),
                            ValueX = primAtt,
                            ValueY = secdAtt
                        });
                    }
                }
            }
        }

        foreach (var ps in PrimarySkills){
            foreach (var ss in SecondarySkills){
                if (ps.SkillType == ss.SkillType){
                    var primSkl = PrimaryPassives.GetNaturalAndBonusSkillLevel(ps.SkillType);
                    var secdSkill = SecondaryPassives.GetNaturalAndBonusSkillLevel(ss.SkillType);

                    var diff = Mathf.Max(primSkl, secdSkill) -
                               Mathf.Min(primSkl, secdSkill);
                    if (diff > 1){
                        found = true;

                        foundInterest.Add(new EffectCommonStatData(){
                            PrimaryEnum = new DynamicGameEnum(ps.SkillType),
                            ValueX = primSkl,
                            ValueY = secdSkill
                        });
                    }
                }
            }
        }

        foreach (var pw in PrimaryWellness){
            foreach (var sw in SecondaryWellness){
                if (pw.WellnessType == sw.WellnessType){
                    var primWel = pw.CurrentValue;
                    var secdWel = sw.CurrentValue;

                    var diff = Mathf.Max(primWel, secdWel) -
                               Mathf.Min(primWel, secdWel);
                    if (diff > 1){
                        found = true;

                        foundInterest.Add(new EffectCommonStatData(){
                            PrimaryEnum = new DynamicGameEnum(pw.WellnessType),
                            ValueX = primWel,
                            ValueY = secdWel
                        });
                    }
                }
            }
        }

        if (found){
            RelationshipElement relX = default;
            var relXIndex = 0;
            var xHasRel = false;
            RelationshipElement relY = default;
            var relYIndex = 0;
            var yHasRel = false;

            for (var i = 0; i < PrimaryRelationships.Length; i++){
                if (PrimaryRelationships[i].Character == secondaryTarget){
                    relX = PrimaryRelationships[i];
                    relXIndex = i;
                    xHasRel = true;
                    break;
                }
            }

            for (var i = 0; i < SecondaryRelationships.Length; i++){
                if (SecondaryRelationships[i].Character == primaryTarget){
                    relY = SecondaryRelationships[i];
                    relYIndex = i;
                    yHasRel = true;
                    break;
                }
            }

            for (int i = 0; i < limit; i++){
                var next = RandomComponent.ValueRW.Random.NextInt(0, foundInterest.Length);
                var learningKnowledge = foundInterest[next];
                foundInterest.RemoveAt(next);
                var knowledgeType = learningKnowledge.GetKnowledgeType();

                if (xHasRel){
                    relX.AffectStat(RelationshipStatType.RESENTMENT, 1);
                }

                if (yHasRel){
                    relY.AffectStat(RelationshipStatType.RESENTMENT, 1);
                }

                if (Display){
                    DisplaySpawnElements.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = new DynamicGameEnum(RelationshipStatType.RESENTMENT),
                        Value = 1,
                        DisplayNumber = true,
                        DisplayColor = Color.green
                    });
                    DisplaySpawnElements.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = secondaryTarget,
                        DisplayEnum = new DynamicGameEnum(RelationshipStatType.RESENTMENT),
                        Value = 1,
                        DisplayNumber = true,
                        DisplayColor = Color.green
                    });
                }

                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = primaryTarget,
                    KnowledgeType = knowledgeType,
                    PrimaryTarget = secondaryTarget,
                    PrimaryEnumValue = learningKnowledge.PrimaryEnum,
                    SecondaryEnumValue = learningKnowledge.SecondaryEnum,
                    IntValue = learningKnowledge.ValueY
                });

                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = secondaryTarget,
                    KnowledgeType = knowledgeType,
                    PrimaryTarget = primaryTarget,
                    PrimaryEnumValue = learningKnowledge.PrimaryEnum,
                    SecondaryEnumValue = learningKnowledge.SecondaryEnum,
                    IntValue = learningKnowledge.ValueX
                });
                
                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = primaryTarget,
                    KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                    PrimaryTarget = secondaryTarget,
                    SecondaryTarget = primaryTarget,
                    PrimaryEnumValue = new DynamicGameEnum(RelationshipStatType.RESENTMENT),
                });

                KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                    LearningEntity = secondaryTarget,
                    KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                    PrimaryTarget = primaryTarget,
                    SecondaryTarget = secondaryTarget,
                    PrimaryEnumValue = new DynamicGameEnum(RelationshipStatType.RESENTMENT),
                });

                if (data.SourceLearnsKnowledge){
                    KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                        LearningEntity = sourceEntity,
                        KnowledgeType = knowledgeType,
                        PrimaryTarget = secondaryTarget,
                        PrimaryEnumValue = learningKnowledge.PrimaryEnum,
                        SecondaryEnumValue = learningKnowledge.SecondaryEnum,
                        IntValue = learningKnowledge.ValueX
                    });

                    KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                        LearningEntity = sourceEntity,
                        KnowledgeType = knowledgeType,
                        PrimaryTarget = primaryTarget,
                        PrimaryEnumValue = learningKnowledge.PrimaryEnum,
                        SecondaryEnumValue = learningKnowledge.SecondaryEnum,
                        IntValue = learningKnowledge.ValueY
                    });
                }
            }

            if (xHasRel){
                PrimaryRelationships[relXIndex] = relX;
            }

            if (yHasRel){
                SecondaryRelationships[relYIndex] = relY;
            }
        }
    }
}