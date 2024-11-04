using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class TrendingAuthoring : MonoBehaviour { }

public class TrendingBaker : Baker<TrendingAuthoring> {
    public override void Bake(TrendingAuthoring authoring){
        AddComponent<TrendingComponent>();
        var buffer = AddBuffer<TrendingElement>();

        var clothingItemTypes = CommonUtils.GetValues<ClothingItemCategory>();
        foreach (var value in clothingItemTypes){
            if (value == ClothingItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.CLOTHING_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var meleeItemTypes = CommonUtils.GetValues<MeleeWeaponItemCategory>();
        foreach (var value in meleeItemTypes){
            if (value == MeleeWeaponItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.MELEE_WEAPON_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var rangedItemTypes = CommonUtils.GetValues<RangedWeaponItemCategory>();
        foreach (var value in rangedItemTypes){
            if (value == RangedWeaponItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.RANGED_WEAPON_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var bookItemTypes = CommonUtils.GetValues<BookItemCategory>();
        foreach (var value in bookItemTypes){
            if (value == BookItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.BOOK_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var securityItemTypes = CommonUtils.GetValues<SecurityItemCategory>();
        foreach (var value in securityItemTypes){
            if (value == SecurityItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.SECURITY_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var foodItemTypes = CommonUtils.GetValues<FoodItemCategory>();
        foreach (var value in foodItemTypes){
            if (value == FoodItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.FOOD_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var techItemTypes = CommonUtils.GetValues<TechItemCategory>();
        foreach (var value in techItemTypes){
            if (value == TechItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.TECH_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var practicalItemTypes = CommonUtils.GetValues<PracticalUseItemCategory>();
        foreach (var value in practicalItemTypes){
            if (value == PracticalUseItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.PRACTICAL_USE_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var miscItemTypes = CommonUtils.GetValues<MiscItemCategory>();
        foreach (var value in miscItemTypes){
            if (value == MiscItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.MISC_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var currencyItemTypes = CommonUtils.GetValues<CurrencyItemCategory>();
        foreach (var value in currencyItemTypes){
            if (value == CurrencyItemCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.CURRENCY_ITEM_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var skillTypes = CommonUtils.GetValues<SkillType>();
        foreach (var value in skillTypes){
            if (value == SkillType.COMMON){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.SKILL_TYPE,
                    IntValue = (int) value
                },
            });
        }

        var attrTypes = CommonUtils.GetValues<AttributeType>();
        foreach (var value in attrTypes){
            if (value == AttributeType.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.ATTRIBUTE_TYPE,
                    IntValue = (int) value
                },
            });
        }

        var wellnessTypes = CommonUtils.GetValues<WellnessType>();
        foreach (var value in wellnessTypes){
            if (value == WellnessType.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.WELLNESS_TYPE,
                    IntValue = (int) value
                },
            });
        }

        var personalityTraitTypes = CommonUtils.GetValues<PersonalityTraitCategory>();
        foreach (var value in personalityTraitTypes){
            if (value == PersonalityTraitCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.PERSONALITY_TRAIT_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var geneticTraitTypes = CommonUtils.GetValues<GeneticTraitCategory>();
        foreach (var value in geneticTraitTypes){
            if (value == GeneticTraitCategory.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.GENETIC_TRAIT_CATEGORY,
                    IntValue = (int) value
                },
            });
        }

        var generalInterestTypes = CommonUtils.GetValues<GeneralInterestType>();
        foreach (var value in generalInterestTypes){
            if (value == GeneralInterestType.NONE){
                continue;
            }

            buffer.Add(new TrendingElement(){
                EnumValue = new DynamicGameEnum(){
                    Type = GameEnumType.GENERAL_INTEREST_TYPE,
                    IntValue = (int) value
                },
            });
        }
    }
}