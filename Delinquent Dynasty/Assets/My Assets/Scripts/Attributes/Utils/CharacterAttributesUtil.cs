using Unity.Collections;
using Unity.Entities;

public struct CharacterAttributesUtil {
    public DynamicBuffer<CharacterAttributeElement> Attributes;

    // TODO: obviously make a scriptable for this or not.
    private const int XpToAttributeLevel2 = 200;
    private const int XpToAttributeLevel3 = 500;
    private const int XpToAttributeLevel4 = 1000;
    private const int XpToAttributeLevel5 = 1500;
    private const int XpToAttributeLevel6 = 2000;
    private const int XpToAttributeLevel7 = 2500;
    private const int XpToAttributeLevel8 = 3000;
    private const int XpToAttributeLevel9 = 3500;
    private const int XpToAttributeLevel10 = 4000;
    private const int XpToAttributeLevel11 = 5000;
    private const int XpToAttributeLevel12 = 6000;
    private const int XpToAttributeLevel13 = 7000;
    private const int XpToAttributeLevel14 = 8000;
    private const int XpToAttributeLevel15 = 9000;
    private const int XpToAttributeLevel16 = 10000;
    private const int XpToAttributeLevel17 = 11000;
    private const int XpToAttributeLevel18 = 12000;
    private const int XpToAttributeLevel19 = 13000;
    private const int XpToAttributeLevel20 = 14000;

    private struct RankingSortData {
        public bool Strength;
        public bool Dexterity;
        public bool Vitality;
        public bool Wisdom;
        public bool Charisma;
        public bool Intelligence;
        public int Value;
    }

    public int GetXpToNextLevelForAttribute(int currentLvl){
        switch (currentLvl){
            case 1:
                return XpToAttributeLevel2;
            case 2:
                return XpToAttributeLevel3;
            case 3:
                return XpToAttributeLevel4;
            case 4:
                return XpToAttributeLevel5;
            case 5:
                return XpToAttributeLevel6;
            case 6:
                return XpToAttributeLevel7;
            case 7:
                return XpToAttributeLevel8;
            case 8:
                return XpToAttributeLevel9;
            case 9:
                return XpToAttributeLevel10;
            case 10:
                return XpToAttributeLevel11;
            case 11:
                return XpToAttributeLevel12;
            case 12:
                return XpToAttributeLevel13;
            case 13:
                return XpToAttributeLevel14;
            case 14:
                return XpToAttributeLevel15;
            case 15:
                return XpToAttributeLevel16;
            case 16:
                return XpToAttributeLevel17;
            case 17:
                return XpToAttributeLevel18;
            case 18:
                return XpToAttributeLevel19;
            case 19:
                return XpToAttributeLevel20;
        }

        return -1;
    }

    private CharacterAttributeElement GetAttribute(AttributeType type, out int index){
        for (var i = 0; i < Attributes.Length; i++){
            var characterAttributeElement = Attributes[i];
            if (characterAttributeElement.AttributeType == type){
                index = i;
                return characterAttributeElement;
            }
        }

        index = -1;
        return default;
    }

    public int GetLevel(AttributeType attributeType){
        foreach (var attributeElement in Attributes){
            if (attributeElement.AttributeType == attributeType){
                return attributeElement.Level;
            }
        }

        return 0;
    }

    public float GetXpRatio(AttributeType type){
        var attribute = GetAttribute(type, out _);
        var nextLvl = GetXpToNextLevelForAttribute(attribute.Level);
        return (float) attribute.CurrentExp / nextLvl;
    }

    public CharacterAttributeElement AddXp(int effectValue, AttributeType attributeType){
        var attribute = GetAttribute(attributeType, out int index);
        var nextLvl = GetXpToNextLevelForAttribute(attribute.Level);
        attribute.CurrentExp += effectValue;
        if (attribute.CurrentExp >= nextLvl){
            attribute.CurrentExp = 0;
            attribute.Level++;
        }

        Attributes[index] = attribute;

        return attribute;
    }

    public int GetRanking(AttributeType attributeType){
        var values = GetAllLevels();

        // TODO: really have no idea how this logic came about. cant really understand but seems to work. either make it better and more readable or just leave
        // TODO: this and remove dupes logic probably can be better
        // var n = values.Length;
        // for (int i = 0; i < n - 1; i++){
        //     for (int j = 0; j < n - i - 1; j++){
        //         if (values[j].Value < values[j + 1].Value){
        //             // swap positions
        //             (values[j], values[j + 1]) = (values[j + 1], values[j]);
        //         }
        //     }
        // }

        var noDupes = new FixedList4096Bytes<RankingSortData>();
        foreach (var next in values){
            var exists = false;
            for (var i = 0; i < noDupes.Length; i++){
                var dup = noDupes[i];
                if (dup.Value == next.Value){
                    exists = true;
                    if (next.Charisma){
                        dup.Charisma = true;
                    }

                    if (next.Strength){
                        dup.Strength = true;
                    }

                    if (next.Intelligence){
                        dup.Intelligence = true;
                    }

                    if (next.Dexterity){
                        dup.Dexterity = true;
                    }

                    if (next.Wisdom){
                        dup.Wisdom = true;
                    }

                    if (next.Vitality){
                        dup.Vitality = true;
                    }

                    break;
                }
            }

            if (!exists){
                noDupes.Add(next);
            }
        }

        for (int i = 0; i < noDupes.Length; i++){
            var isChr = noDupes[i].Charisma && attributeType == AttributeType.CHARISMA;
            var isStr = noDupes[i].Strength && attributeType == AttributeType.STRENGTH;
            var isInt = noDupes[i].Intelligence && attributeType == AttributeType.INTELLIGENCE;
            var isWis = noDupes[i].Wisdom && attributeType == AttributeType.WISDOM;
            var isVit = noDupes[i].Vitality && attributeType == AttributeType.VITALITY;
            var isDex = noDupes[i].Dexterity && attributeType == AttributeType.DEXTERITY;
            if (isChr || isStr || isInt || isWis || isVit || isDex){
                return i + 1;
            }
        }


        return -1;
    }

    public int GetRanking(AttributeType attributeType, PassiveEffectsUtils passives){
        var values = GetAllLevels(passives);

        // TODO: really have no idea how this logic came about. cant really understand but seems to work. either make it better and more readable or just leave
        // TODO: this and remove dupes logic probably can be better
        var n = values.Length;
        for (int i = 0; i < n - 1; i++){
            for (int j = 0; j < n - i - 1; j++){
                if (values[j].Value < values[j + 1].Value){
                    // swap positions
                    (values[j], values[j + 1]) = (values[j + 1], values[j]);
                }
            }
        }

        var noDupes = new FixedList4096Bytes<RankingSortData>();
        foreach (var next in values){
            var exists = false;
            for (var i = 0; i < noDupes.Length; i++){
                var dup = noDupes[i];
                if (dup.Value == next.Value){
                    exists = true;
                    if (next.Charisma){
                        dup.Charisma = true;
                    }

                    if (next.Strength){
                        dup.Strength = true;
                    }

                    if (next.Intelligence){
                        dup.Intelligence = true;
                    }

                    if (next.Dexterity){
                        dup.Dexterity = true;
                    }

                    if (next.Wisdom){
                        dup.Wisdom = true;
                    }

                    if (next.Vitality){
                        dup.Vitality = true;
                    }

                    break;
                }
            }

            if (!exists){
                noDupes.Add(next);
            }
        }

        for (int i = 0; i < noDupes.Length; i++){
            var isChr = noDupes[i].Charisma && attributeType == AttributeType.CHARISMA;
            var isStr = noDupes[i].Strength && attributeType == AttributeType.STRENGTH;
            var isInt = noDupes[i].Intelligence && attributeType == AttributeType.INTELLIGENCE;
            var isWis = noDupes[i].Wisdom && attributeType == AttributeType.WISDOM;
            var isVit = noDupes[i].Vitality && attributeType == AttributeType.VITALITY;
            var isDex = noDupes[i].Dexterity && attributeType == AttributeType.DEXTERITY;
            if (isChr || isStr || isInt || isWis || isVit || isDex){
                return i + 1;
            }
        }


        return -1;
    }

    private FixedList512Bytes<RankingSortData> GetAllLevels(){
        var values = new FixedList512Bytes<RankingSortData>();

        foreach (var characterAttributeElement in Attributes){
            if (characterAttributeElement.AttributeType == AttributeType.STRENGTH){
                values.Add(new RankingSortData(){Strength = true, Value = characterAttributeElement.Level});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.DEXTERITY){
                values.Add(new RankingSortData(){Dexterity = true, Value = characterAttributeElement.Level});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.INTELLIGENCE){
                values.Add(new RankingSortData(){Intelligence = true, Value = characterAttributeElement.Level});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.VITALITY){
                values.Add(new RankingSortData(){Vitality = true, Value = characterAttributeElement.Level});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.WISDOM){
                values.Add(new RankingSortData(){Wisdom = true, Value = characterAttributeElement.Level});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.CHARISMA){
                values.Add(new RankingSortData(){Charisma = true, Value = characterAttributeElement.Level});
            }
        }

        return values;
    }

    private FixedList512Bytes<RankingSortData> GetAllLevels(PassiveEffectsUtils passives){
        var values = new FixedList512Bytes<RankingSortData>();

        foreach (var characterAttributeElement in passives.CharacterAttributes){
            if (characterAttributeElement.AttributeType == AttributeType.STRENGTH){
                values.Add(new RankingSortData()
                    {Strength = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.STRENGTH)});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.DEXTERITY){
                values.Add(new RankingSortData()
                    {Dexterity = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.DEXTERITY)});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.INTELLIGENCE){
                values.Add(new RankingSortData(){
                    Intelligence = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.INTELLIGENCE)
                });
            }
            else if (characterAttributeElement.AttributeType == AttributeType.VITALITY){
                values.Add(new RankingSortData()
                    {Vitality = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.VITALITY)});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.WISDOM){
                values.Add(new RankingSortData()
                    {Wisdom = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.WISDOM)});
            }
            else if (characterAttributeElement.AttributeType == AttributeType.CHARISMA){
                values.Add(new RankingSortData()
                    {Charisma = true, Value = passives.GetNaturalAndBonusAttributeTotal(AttributeType.CHARISMA)});
            }
        }

        return values;
    }
}