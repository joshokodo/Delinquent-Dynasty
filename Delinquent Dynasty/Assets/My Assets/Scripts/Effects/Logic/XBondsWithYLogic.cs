using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct XBondsWithYLogic : IApplyActiveEffect {
    public DynamicBuffer<InterestElement> PrimaryInterest;
    public PassiveEffectsUtils PrimaryPassives;

    public DynamicBuffer<InterestElement> SecondaryInterest;
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
                if (pi.SubjectType == si.SubjectType && pi.EnumValue.Matches(si.EnumValue) && pi.InterestValue > 0 &&
                    si.InterestValue > 0){
                    var diff = Mathf.Max(pi.InterestValue, si.InterestValue) -
                               Mathf.Min(pi.InterestValue, si.InterestValue);
                    if (diff <= 10){
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

        foreach (var pa in PrimaryPassives.CharacterAttributes){
            foreach (var sa in SecondaryPassives.CharacterAttributes){
                if (pa.AttributeType == sa.AttributeType){
                    var primAtt = PrimaryPassives.GetNaturalAndBonusAttributeTotal(pa.AttributeType);
                    var secdAtt = SecondaryPassives.GetNaturalAndBonusAttributeTotal(sa.AttributeType);

                    var diff = Mathf.Max(primAtt, secdAtt) -
                               Mathf.Min(primAtt, secdAtt);
                    if (diff <= 1){
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

        foreach (var ps in PrimaryPassives.Skills){
            foreach (var ss in SecondaryPassives.Skills){
                if (ps.SkillType == ss.SkillType){
                    var primSkl = PrimaryPassives.GetNaturalAndBonusSkillLevel(ps.SkillType);
                    var secdSkill = SecondaryPassives.GetNaturalAndBonusSkillLevel(ss.SkillType);

                    var diff = Mathf.Max(primSkl, secdSkill) -
                               Mathf.Min(primSkl, secdSkill);
                    if (diff <= 0){
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
        
        foreach (var pw in PrimaryPassives.CharacterWellness){
            foreach (var sw in SecondaryPassives.CharacterWellness){
                if (pw.WellnessType == sw.WellnessType){
                    var primWel = pw.CurrentValue;
                    var secdWel = sw.CurrentValue;

                    var diff = Mathf.Max(primWel, secdWel) -
                               Mathf.Min(primWel, secdWel);
                    if (diff <= 10){
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
            RelationshipElement relX = new RelationshipElement(){
                Character = secondaryTarget,
                MainTitle = RelationshipMainTitleType.ACQUAINTANCE
            };
            
            var relXIndex = 0;
            var xHasRel = false;
            
            RelationshipElement relY = new RelationshipElement(){
                Character = primaryTarget,
                MainTitle = RelationshipMainTitleType.ACQUAINTANCE
            };
            
            var relYIndex = 0;
            var yHasRel = false;

            for (var i = 0; i < PrimaryPassives.Relationships.Length; i++){
                if (PrimaryPassives.Relationships[i].Character == secondaryTarget){
                    relX = PrimaryPassives.Relationships[i];
                    relXIndex = i;
                    xHasRel = true;
                    break;
                }
            }

            for (var i = 0; i < SecondaryPassives.Relationships.Length; i++){
                if (SecondaryPassives.Relationships[i].Character == primaryTarget){
                    relY = SecondaryPassives.Relationships[i];
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

                relX.AffectStat(RelationshipStatType.RAPPORT,  1);
                relY.AffectStat(RelationshipStatType.RAPPORT, 1);
                
                if (Display){
                    DisplaySpawnElements.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = new DynamicGameEnum(RelationshipStatType.RAPPORT),
                        Value = 1,
                        DisplayNumber = true,
                        DisplayColor = Color.green
                    });
                    DisplaySpawnElements.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = secondaryTarget,
                        DisplayEnum = new DynamicGameEnum(RelationshipStatType.RAPPORT),
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
                PrimaryPassives.Relationships[relXIndex] = relX;
            } else {
                PrimaryPassives.Relationships.Add(relX);
            }
            if (yHasRel){
                SecondaryPassives.Relationships[relYIndex] = relY;
            } else {
                SecondaryPassives.Relationships.Add(relY);
            }
            
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = secondaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                PrimaryTarget = primaryTarget,
                SecondaryTarget = secondaryTarget,
                PrimaryEnumValue = new DynamicGameEnum(RelationshipStatType.RAPPORT),
                IntValue = relX.Rapport
            });
            
            KnowledgeSpawnElements.Add(new KnowledgeSpawnElement(){
                LearningEntity = primaryTarget,
                KnowledgeType = KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT,
                PrimaryTarget = secondaryTarget,
                SecondaryTarget = primaryTarget,
                PrimaryEnumValue = new DynamicGameEnum(RelationshipStatType.RAPPORT),
                IntValue = relY.Rapport
            });

         
        }
    }

}