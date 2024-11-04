using Unity.Entities;
using UnityEngine;

public struct NumberUtils {
    public static bool CheckNumberComparision(NumericComparisonSign numericComparisonSign, int subject,
        int comparisionValue){
        switch (numericComparisonSign){
            case NumericComparisonSign.NONE:
            case NumericComparisonSign.EQUALS:
                return subject == comparisionValue;
            case NumericComparisonSign.LESS_THAN:
                return subject < comparisionValue;
            case NumericComparisonSign.LESS_THAN_EQUAL:
                return subject <= comparisionValue;
            case NumericComparisonSign.GREATER_THAN:
                return subject > comparisionValue;
            case NumericComparisonSign.GREATER_THAN_EQUAL:
                return subject >= comparisionValue;
            case NumericComparisonSign.NOT_EQUALS:
                return subject != comparisionValue;
        }

        return false;
    }

    public static bool IsBetween(int val, int a, int b){
        return val >= a && val <= b;
    }

    public static void SetRelationshipStatDisplay(int number, int totalBonus, bool isLost, bool isGain,
        Entity primaryTarget, ActiveEffectData data, DynamicBuffer<DisplayDamageSpawnElement> displayDamageSpawn){
        if (isLost){
            if (number == 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayBlock = true,
                        DisplayColor = Color.gray
                    });
                }
            }
            else if (number < 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.yellow
                    });
                }
                else if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.magenta
                    });
                }
                else{
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.red
                    });
                }
            }
        }
        else if (isGain){
            if (number == 0){
                if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayBlock = true,
                        DisplayColor = Color.gray
                    });
                }
            }
            else if (number > 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.blue
                    });
                }
                else if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.cyan
                    });
                }
                else{
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.green
                    });
                }
            }
        }
    }

    public static void SetWellnessDisplay(int number, int totalBonus, bool isDamage, bool isRestore,
        Entity primaryTarget, ActiveEffectData data, DynamicBuffer<DisplayDamageSpawnElement> displayDamageSpawn){
        if (isDamage){
            if (number == 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayBlock = true,
                        DisplayColor = Color.gray
                    });
                }
            }
            else if (number < 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = new Color(245, 100, 100)
                    });
                }
                else if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = new Color(89, 1, 1)
                    });
                }
                else{
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.red
                    });
                }
            }
        }
        else if (isRestore){
            if (number == 0){
                if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayBlock = true,
                        DisplayColor = Color.gray
                    });
                }
            }
            else if (number > 0){
                if (totalBonus > 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = new Color(1, 54, 0)
                    });
                }
                else if (totalBonus < 0){
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = new Color(119, 252, 116)
                    });
                }
                else{
                    displayDamageSpawn.Add(new DisplayDamageSpawnElement(){
                        CharacterEntity = primaryTarget,
                        DisplayEnum = data.PrimaryEnumValue,
                        Value = number,
                        DisplayNumber = true,
                        DisplayColor = Color.green
                    });
                }
            }
        }
    }
}