using System;
using Unity.Collections;
using UnityEngine;

public struct StringUtils {
    public FixedString128Bytes GetEnumString(DynamicGameEnum gameEnum){
        var intValue = gameEnum.IntValue;
        switch (gameEnum.Type){
            case GameEnumType.SKILL_TYPE:
                return new FixedString128Bytes(((SkillType) intValue).ToString());
            case GameEnumType.DEFAULT_TRAIT_CATEGORY:
                return new FixedString128Bytes(((DefaultTraitCategory) intValue).ToString());
            case GameEnumType.GENETIC_TRAIT_CATEGORY:
                return new FixedString128Bytes(((GeneticTraitCategory) intValue).ToString());
            case GameEnumType.PERSONALITY_TRAIT_CATEGORY:
                return new FixedString128Bytes(((PersonalityTraitCategory) intValue).ToString());
            case GameEnumType.PERSONAL_MAINTENANCE_TRAIT_CATEGORY:
                return new FixedString128Bytes(((WellBeingTraitCategory) intValue).ToString());
            case GameEnumType.STATUS_TRAIT_CATEGORY:
                return new FixedString128Bytes(((StatusTraitCategory) intValue).ToString());
            case GameEnumType.ATTRIBUTE_TYPE:
                return new FixedString128Bytes(((AttributeType) intValue).ToString());
            case GameEnumType.GENERAL_INTEREST_TYPE:
                return new FixedString128Bytes(((GeneralInterestType) intValue).ToString());
            case GameEnumType.INTEREST_SUBJECT_TYPE:
                return new FixedString128Bytes(((InterestSubjectType) intValue).ToString());
            case GameEnumType.WELLNESS_TYPE:
                return new FixedString128Bytes(((WellnessType) intValue).ToString());
            case GameEnumType.RELATIONSHIP_STAT_TYPE:
                return new FixedString128Bytes(((RelationshipStatType) intValue).ToString());
            case GameEnumType.RELATIONSHIP_MAIN_TITLE_TYPE:
                return new FixedString128Bytes(((RelationshipMainTitleType) intValue).ToString());
            case GameEnumType.ITEM_PROPERTY_TYPE:
                return new FixedString128Bytes(((ItemPropertyType) intValue).ToString());
            case GameEnumType.CHARACTER_TYPE:
                return new FixedString128Bytes(((CharacterType) intValue).ToString());
            case GameEnumType.LOCOMOTION_STATE:
                return new FixedString128Bytes(((LocomotionState) intValue).ToString());
            case GameEnumType.KNOWLEDGE_TYPE:
                return new FixedString128Bytes(((KnowledgeType) intValue).ToString());
            case GameEnumType.ACTION_CATEGORY:
                return new FixedString128Bytes(((ActionCategory) intValue).ToString());
            case GameEnumType.CLOTHING_ITEM_CATEGORY:
                return new FixedString128Bytes(((ClothingItemCategory) intValue).ToString());
            case GameEnumType.MELEE_WEAPON_ITEM_CATEGORY:
                return new FixedString128Bytes(((MeleeWeaponItemCategory) intValue).ToString());
            case GameEnumType.RANGED_WEAPON_ITEM_CATEGORY:
                return new FixedString128Bytes(((RangedWeaponItemCategory) intValue).ToString());
            case GameEnumType.BOOK_ITEM_CATEGORY:
                return new FixedString128Bytes(((BookItemCategory) intValue).ToString());
            case GameEnumType.SECURITY_ITEM_CATEGORY:
                return new FixedString128Bytes(((SecurityItemCategory) intValue).ToString());
            case GameEnumType.FOOD_ITEM_CATEGORY:
                return new FixedString128Bytes(((FoodItemCategory) intValue).ToString());
            case GameEnumType.TECH_ITEM_CATEGORY:
                return new FixedString128Bytes(((TechItemCategory) intValue).ToString());
            case GameEnumType.MISC_ITEM_CATEGORY:
                return new FixedString128Bytes(((MiscItemCategory) intValue).ToString());
            case GameEnumType.CURRENCY_ITEM_CATEGORY:
                return new FixedString128Bytes(((CurrencyItemCategory) intValue).ToString());
            case GameEnumType.SKILL_BASED_ITEM_ACTION_TYPE:
                return new FixedString128Bytes(((SkillBasedItemActionType) intValue).ToString());
            case GameEnumType.SKILL_BASED_SOCIAL_ACTION_TYPE:
                return new FixedString128Bytes(((SkillBasedSocialActionType) intValue).ToString());
            case GameEnumType.MISC_ACTION_TYPE:
                return new FixedString128Bytes(((MiscActionType) intValue).ToString());
            case GameEnumType.FIGHTING_ACTION_TYPE:
                return new FixedString128Bytes(((FightingActionType) intValue).ToString());
            case GameEnumType.GRAPPLING_ACTION_TYPE:
                return new FixedString128Bytes(((GrapplingActionType) intValue).ToString());
            case GameEnumType.COMMON_ITEM_ACTION_TYPE:
                return new FixedString128Bytes(((CommonItemActionType) intValue).ToString());
            case GameEnumType.COMMON_SOCIAL_ACTION_TYPE:
                return new FixedString128Bytes(((CommonSocialActionType) intValue).ToString());
            case GameEnumType.LOCOMOTION_ACTION_TYPE:
                return new FixedString128Bytes(((LocomotionActionType) intValue).ToString());
            case GameEnumType.ROOM_TYPE:
                return new FixedString128Bytes(((RoomType) intValue).ToString());
        }

        return default;
    }

    public static FixedString128Bytes GetActionTypeString(DynamicActionType actionType){
        switch (actionType.Category){
            case ActionCategory.COMMON_ITEMS:
                return new FixedString128Bytes(actionType.CommonItemActionType.ToString());
            case ActionCategory.SKILL_ITEMS:
                return new FixedString128Bytes(actionType.SkillBasedItemActionType.ToString());
            case ActionCategory.SKILL_SOCIAL:
                return new FixedString128Bytes(actionType.SkillBasedSocialActionType.ToString());
            case ActionCategory.COMMON_SOCIAL:
                return new FixedString128Bytes(actionType.CommonSocialActionType.ToString());
            case ActionCategory.FIGHTING:
                return new FixedString128Bytes(actionType.FightingActionType.ToString());
            case ActionCategory.GRAPPLING:
                return new FixedString128Bytes(actionType.GrapplingActionType.ToString());
            case ActionCategory.MISC:
                return new FixedString128Bytes(actionType.MiscActionType.ToString());
            case ActionCategory.LOCOMOTION:
                return new FixedString128Bytes(actionType.LocomotionActionType.ToString());
        }

        return new FixedString128Bytes();
    }

    public static FixedString128Bytes GetTypeString(DynamicGameEnum gameEnum){
        switch (gameEnum.Type){
            case GameEnumType.INTEREST_SUBJECT_TYPE:
                return new FixedString128Bytes(gameEnum.InterestSubjectType.ToString());

            case GameEnumType.GENERAL_INTEREST_TYPE:
                return new FixedString128Bytes(gameEnum.GeneralInterestType.ToString());

            case GameEnumType.SKILL_TYPE:
                return new FixedString128Bytes(gameEnum.SkillType.ToString());

            case GameEnumType.WELLNESS_TYPE:
                return new FixedString128Bytes(gameEnum.WellnessType.ToString());

            case GameEnumType.ATTRIBUTE_TYPE:
                return new FixedString128Bytes(gameEnum.AttributeType.ToString());

            case GameEnumType.STATUS_TRAIT_CATEGORY:
            case GameEnumType.DEFAULT_TRAIT_CATEGORY:
            case GameEnumType.GENETIC_TRAIT_CATEGORY:
            case GameEnumType.PERSONALITY_TRAIT_CATEGORY:
            case GameEnumType.PERSONAL_MAINTENANCE_TRAIT_CATEGORY:
                return GetTraitTypeString(gameEnum.GetTraitType());

            case GameEnumType.BOOK_ITEM_CATEGORY:
            case GameEnumType.FOOD_ITEM_CATEGORY:
            case GameEnumType.MISC_ITEM_CATEGORY:
            case GameEnumType.TECH_ITEM_CATEGORY:
            case GameEnumType.CLOTHING_ITEM_CATEGORY:
            case GameEnumType.CURRENCY_ITEM_CATEGORY:
            case GameEnumType.COMMON_ITEM_ACTION_TYPE:
            case GameEnumType.SECURITY_ITEM_CATEGORY:
            case GameEnumType.MELEE_WEAPON_ITEM_CATEGORY:
            case GameEnumType.RANGED_WEAPON_ITEM_CATEGORY:
            case GameEnumType.PRACTICAL_USE_ITEM_CATEGORY:
                return GetItemTypeString(gameEnum.GetItemType());

            default:
                throw new NotImplementedException("ADD HOE");
        }

        return new FixedString128Bytes();
    }

    public static FixedString128Bytes GetItemTypeString(DynamicItemType itemType){
        switch (itemType.Category){
            case ItemCategory.CLOTHING:
                return new FixedString128Bytes(itemType.ClothingCategory.ToString());
            case ItemCategory.MELEE_WEAPON:
                return new FixedString128Bytes(itemType.MeleeCategory.ToString());
            case ItemCategory.RANGED_WEAPON:
                return new FixedString128Bytes(itemType.RangedCategory.ToString());
            case ItemCategory.BOOK:
                return new FixedString128Bytes(itemType.BookCategory.ToString());
            case ItemCategory.SECURITY:
                return new FixedString128Bytes(itemType.SecurityCategory.ToString());
            case ItemCategory.FOOD:
                return new FixedString128Bytes(itemType.FoodCategory.ToString());
            case ItemCategory.TECH:
                return new FixedString128Bytes(itemType.TechCategory.ToString());
            case ItemCategory.PRACTICAL_USE:
                return new FixedString128Bytes(itemType.PracticalUseCategory.ToString());
            case ItemCategory.MISC:
                return new FixedString128Bytes(itemType.MiscCategory.ToString());
            case ItemCategory.CURRENCY:
                return new FixedString128Bytes(itemType.CurrencyCategory.ToString());
        }

        return default;
    }

    public static FixedString128Bytes GetTraitTypeString(DynamicTraitType traitType){
        switch (traitType.Category){
            case TraitCategory.DEFAULT:
                return new FixedString128Bytes(traitType.DefaultTraitCategory.ToString());
            case TraitCategory.GENETIC:
                return new FixedString128Bytes(traitType.GeneticTraitCategory.ToString());
            case TraitCategory.PERSONALITY:
                return new FixedString128Bytes(traitType.PersonalityTraitCategory.ToString());
            case TraitCategory.PERSONAL_MAINTENANCE:
                return new FixedString128Bytes(traitType.WellBeingTraitCategory.ToString());
            case TraitCategory.STATUS:
                return new FixedString128Bytes(traitType.StatusTraitCategory.ToString());
        }

        return default;
    }

    public static FixedString4096Bytes SetConditionsString(FixedList4096Bytes<ConditionData> conditions,
        FixedString4096Bytes result){
        for (var i = 0; i < conditions.Length; i++){
            var cond = conditions[i];
            result.Append(i == 0 ? "If " : "& ");
            var targetsSelf = cond.PrimaryTarget == TargetType.SELF;

            switch (cond.ConditionType){
                case ConditionType.X_IS_SKILL_LEVEL:
                    result.Append(cond.PrimaryEnumValue.SkillType + "lvl ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.X_HAS_TRAIT:
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(GetTraitTypeString(cond.PrimaryEnumValue.GetTraitType()) + " ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.FLAT_SUCCESS_CHANCE:
                    result.Append(" " + cond.PrimaryNumberValue + "% chance ");
                    break;

                case ConditionType.ITEM_X_IS_EQUIPPED:
                    result.Append(cond.ExpectedConditionValue ? " target item equipped" : " target item not equipped");
                    break;

                case ConditionType.ITEM_X_IS_ON:
                    result.Append(cond.ExpectedConditionValue ? " target item is on" : "target item is off");
                    break;

                // case ConditionType.X_IS_AT_ROOM_Y:
                //     result.Append(cond.ExpectedConditionValue
                //         ? " you are a student"
                //         : " you are not a student"); // todo: change this when we use character types
                //     break;

                case ConditionType.X_IS_TOTAL_ATTRIBUTE_LEVEL:
                    result.Append(cond.PrimaryEnumValue.AttributeType + " Total Level ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.COMPARE_X_TOTAL_ATTRIBUTE_TO_Y_TOTAL_ATTRIBUTE:
                    result.Append("Total ");
                    result.Append(cond.PrimaryEnumValue.AttributeType + " ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" Others total " + cond.PrimaryEnumValue.AttributeType);
                    break;

                case ConditionType.X_BEATS_SKILL_CHECK:
                    result.Append("Beats skill check for ");
                    result.Append(cond.PrimaryEnumValue.SkillType + " vs " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.X_IS_TOTAL_ATTRIBUTE_LEVEL_RANKING:
                    result.Append("Total ");
                    result.Append(cond.PrimaryEnumValue.AttributeType + " ranking ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.X_RELATIONSHIP_STAT_FOR_Y:
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.PrimaryEnumValue.RelationshipStatType + " for ");
                    result.Append(targetsSelf ? "target " : "you ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.X_RELATIONSHIP_STATS_FOR_Y_COMPARED:
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.PrimaryEnumValue.RelationshipStatType + " for ");
                    result.Append(targetsSelf ? "target " : "you ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.SecondaryEnumValue.RelationshipStatType + " for ");
                    result.Append(targetsSelf ? "target " : "you ");
                    break;
                
                case ConditionType.X_RELATIONSHIP_STAT_COMPARED_TO_Y_STAT:
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.PrimaryEnumValue.RelationshipStatType + " for ");
                    result.Append(targetsSelf ? "target " : "you ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(targetsSelf ? "Target's " : "Your ");
                    result.Append(cond.SecondaryEnumValue.RelationshipStatType + " for ");
                    result.Append(targetsSelf ? "you " : "target ");
                    break;

                case ConditionType.X_WELLNESS_STATS_COMPARED:
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.PrimaryEnumValue.WellnessType + " ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(targetsSelf ? "Your " : "Target's ");
                    result.Append(cond.SecondaryEnumValue.WellnessType + " ");
                    break;

                case ConditionType.X_NATURAL_ATTRIBUTES_STATS_COMPARED:
                    result.Append(targetsSelf ? "Your natural " : "Target's natural ");
                    result.Append(cond.PrimaryEnumValue.AttributeType + " ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(targetsSelf ? "Your natural " : "Target's natural ");
                    result.Append(cond.SecondaryEnumValue.AttributeType + " ");
                    break;

                case ConditionType.X_HAS_INTEREST:
                    result.Append(targetsSelf ? "Your interest in " : "Target's interest in ");
                    result.Append(GetTypeString(cond.SecondaryEnumValue) + " ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;

                case ConditionType.X_HAS_COMMON_INTEREST_WITH_Y:
                    result.Append("You and target's common interest count ");
                    result = SetNumericComparisionString(cond.NumericComparisonSign, result,
                        cond.ExpectedConditionValue);
                    result.Append(" " + cond.PrimaryNumberValue);
                    break;
                default:
                    throw new NotImplementedException("add case for " +
                                                      cond
                                                          .ConditionType); // TODO: just add the rest later when not lazy
            }

            result.Append("\n");
        }

        return result;
    }

    public static FixedString4096Bytes SetActiveEffectsString(FixedList512Bytes<ActiveEffectData> effects,
        FixedString4096Bytes result, int effectsRange = 0){
        for (var i = 0; i < effects.Length; i++){
            var eff = effects[i];
            var primaryTargetIsSelf = eff.EffectPrimaryTarget == TargetType.SELF;
            var primaryTargetIsOther = eff.EffectPrimaryTarget == TargetType.TARGET_CHARACTER;
            var primaryTargetIsTool = eff.EffectPrimaryTarget == TargetType.TARGET_CRAFTING_TOOL;
            var primaryTargetIsAllVisible = eff.EffectPrimaryTarget == TargetType.TARGET_VISIBLE_CHARACTERS_IN_AREA;

            result.Append("\t - ");

            switch (eff.ActiveEffectType){
                case ActiveEffectType.AFFECT_WELLNESS_OF_X:
                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            eff.PrimaryEnumValue.WellnessType.ToString());
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            eff.PrimaryEnumValue.WellnessType.ToString());
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            eff.PrimaryEnumValue.WellnessType.ToString());
                        result.Append("\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_X_RELATIONSHIP_STAT_OF_Y:
                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            eff.PrimaryEnumValue.RelationshipStatType + " for target");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            eff.PrimaryEnumValue.RelationshipStatType + " for you");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            eff.PrimaryEnumValue.RelationshipStatType + " for you");
                        result.Append("\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_SKILL_XP_OF_X:
                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            eff.PrimaryEnumValue.SkillType + " XP");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            eff.PrimaryEnumValue.SkillType + " XP");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            eff.PrimaryEnumValue.SkillType + " XP");
                        result.Append("\n");
                    }

                    break;
                case ActiveEffectType.AFFECT_ATTRIBUTE_XP_OF_X:
                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            eff.PrimaryEnumValue.AttributeType + " XP");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            eff.PrimaryEnumValue.AttributeType + " XP");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            eff.PrimaryEnumValue.AttributeType + " XP");
                        result.Append("\n");
                    }

                    break;
                case ActiveEffectType.X_LEARNS_RANDOM_KNOWLEDGE_ABOUT_Y:
                    if (primaryTargetIsSelf){
                        result.Append("You learn random knowledge about target\n");
                    }
                    else if (primaryTargetIsOther){
                        result.Append("Target learns random knowledge about you\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("All visible targets' learn random knowledge about you within " + effectsRange +
                                      "m\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_TRAIT_INTENSITY_OF_X:
                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            GetTraitTypeString(eff.PrimaryEnumValue.GetTraitType()).Value);
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            GetTraitTypeString(eff.PrimaryEnumValue.GetTraitType()).Value);
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            GetTraitTypeString(eff.PrimaryEnumValue.GetTraitType()).Value);
                        result.Append("\n");
                    }

                    break;

                case ActiveEffectType.X_ADDS_RELATIONSHIP_TITLE_TO_Y:
                    if (primaryTargetIsSelf){
                        result.Append(
                            "Target becomes a " + eff.PrimaryEnumValue.RelationshipMainTitleType + " to you\n");
                    }
                    else if (primaryTargetIsOther){
                        result.Append("You become a " + eff.PrimaryEnumValue.RelationshipMainTitleType +
                                      " to target\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("You become a " + eff.PrimaryEnumValue.RelationshipMainTitleType +
                                      " to all visible targets within " + effectsRange + "Ms\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_BUILD_PROGRESS_OF_X:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target item",
                        " build progress");
                    break;
                case ActiveEffectType.AFFECT_BUILD_QUALITY_OF_X:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target item",
                        " build quality");
                    break;
                case ActiveEffectType.AFFECT_BUILD_DEFECT_OF_X:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target item",
                        " build defect");
                    break;

                case ActiveEffectType.AFFECT_DURABILITY_OF_X:
                    if (primaryTargetIsTool){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "Target tools",
                            " durability");
                    }
                    else{
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "Target item",
                            " durability");
                    }

                    break;

                case ActiveEffectType.X_LEARNS_SKILL:
                    if (primaryTargetIsSelf){
                        result.Append("You learns skill " + eff.PrimaryEnumValue.SkillType + "\n");
                    }
                    else if (primaryTargetIsOther){
                        result.Append("Target learns skill " + eff.PrimaryEnumValue.SkillType + " to target\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("All visible targets within" + effectsRange + "Ms learns skill " +
                                      eff.PrimaryEnumValue.SkillType + "\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_X_INTEREST_IN_SUBJECT:

                    var interestType = "";
                    switch (eff.SecondaryEnumValue.InterestSubjectType){
                        case InterestSubjectType.SKILL:
                            interestType = eff.TertiaryEnumValue.SkillType.ToString();
                            break;
                        case InterestSubjectType.ATTRIBUTE:
                            interestType = eff.TertiaryEnumValue.AttributeType.ToString();
                            break;
                        case InterestSubjectType.WELLNESS:
                            interestType = eff.TertiaryEnumValue.WellnessType.ToString();
                            break;
                        case InterestSubjectType.GENERAL_INTEREST:
                            interestType = eff.TertiaryEnumValue.GeneralInterestType.ToString();
                            break;
                        case InterestSubjectType.ITEM:
                            interestType = eff.TertiaryEnumValue.ClothingItemCategory.ToString();
                            break;
                        default:
                            interestType = "Target Interest";
                            break;
                    }

                    if (primaryTargetIsSelf){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You",
                            " interest in " + interestType);
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, "Target",
                            " interest in " + interestType);
                        result.Append("\n");
                    }
                    else if (primaryTargetIsAllVisible){
                        result.Append("Visible Targets within " + effectsRange + "Ms");
                        result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, suffix:
                            " interest in " + interestType);
                        result.Append("\n");
                    }

                    break;

                case ActiveEffectType.AFFECT_TRENDING_IN_INTEREST:

                    var trendingSubject = "";
                    switch (eff.SecondaryEnumValue.InterestSubjectType){
                        case InterestSubjectType.SKILL:
                            trendingSubject = eff.TertiaryEnumValue.SkillType.ToString();
                            break;
                        case InterestSubjectType.ATTRIBUTE:
                            trendingSubject = eff.TertiaryEnumValue.AttributeType.ToString();
                            break;
                        case InterestSubjectType.WELLNESS:
                            trendingSubject = eff.TertiaryEnumValue.WellnessType.ToString();
                            break;
                        case InterestSubjectType.GENERAL_INTEREST:
                            trendingSubject = eff.TertiaryEnumValue.GeneralInterestType.ToString();
                            break;
                        case InterestSubjectType.ITEM:
                            trendingSubject = eff.TertiaryEnumValue.ClothingItemCategory.ToString();
                            break;
                        default:
                            trendingSubject = "Target Interest";
                            break;
                    }

                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, true, trendingSubject,
                        " in trending ");
                    result.Append("\n");
                    break;

                case ActiveEffectType.X_LEARNS_SPECIFIC_KNOWLEDGE_ABOUT_Y:
                    result.Append(primaryTargetIsSelf ? "You learn " : "They learn");
                    switch (eff.PrimaryEnumValue.KnowledgeType){
                        case KnowledgeType.LAST_KNOWN_RELATIONSHIP_STAT:
                            result.Append("last known " + eff.SecondaryEnumValue.RelationshipStatType +
                                          " of target for you");
                            break;
                        case KnowledgeType.LAST_KNOWN_SKILL:
                            result.Append("last known " + eff.SecondaryEnumValue.SkillType + " of target");
                            break;
                        case KnowledgeType.LAST_KNOWN_WELLNESS:
                            result.Append("last known " + eff.SecondaryEnumValue.WellnessType + " of target");
                            break;
                        case KnowledgeType.LAST_KNOWN_ATTRIBUTE:
                            result.Append("last known " + eff.SecondaryEnumValue.AttributeType + " of target");
                            break;
                        case KnowledgeType.LAST_KNOWN_TRAIT:
                            result.Append("last known " + GetTraitTypeString(eff.SecondaryEnumValue.GetTraitType()) +
                                          " of target");
                            break;
                        case KnowledgeType.LAST_KNOWN_INTEREST:
                            result.Append("last known " + "(finish me)" + " of target");
                            break;
                    }

                    result.Append("\n");
                    break;

                case ActiveEffectType.X_BONDS_WITH_Y:
                    result.Append("You and target try to bond");
                    result.Append("\n");
                    break;

                case ActiveEffectType.X_CONFLICTS_WITH_Y:
                    result.Append("You and target try to conflict");
                    result.Append("\n");
                    break;

                case ActiveEffectType.INCLUSIVELY_AFFECT_RELATIONSHIP_STAT_OF_X_AND_Y:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You and target",
                        eff.PrimaryEnumValue.RelationshipStatType + " for each other (Inclusively)");
                    result.Append("\n");
                    break;

                case ActiveEffectType.INCLUSIVELY_AFFECT_WELLNESS_OF_X_AND_Y:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You and target",
                        eff.PrimaryEnumValue.WellnessType+ " (Inclusively)");
                    result.Append("\n");
                    break;
                
                case ActiveEffectType.EXCLUSIVELY_AFFECT_RELATIONSHIP_STAT_OF_X_AND_Y:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You and target",
                        eff.PrimaryEnumValue.RelationshipStatType + " for each other (Exclusively)");
                    result.Append("\n");
                    break;

                case ActiveEffectType.EXCLUSIVELY_AFFECT_WELLNESS_OF_X_AND_Y:
                    result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You and target",
                        eff.PrimaryEnumValue.WellnessType+ " (Exclusively)");
                    result.Append("\n");
                    break;

                case ActiveEffectType.X_LEARNS_RANDOM_KNOWLEDGE_CATEGORY_ABOUT_Y:
                    if (primaryTargetIsSelf){
                        result.Append("You learn random " + GetTypeString(eff.PrimaryEnumValue) +
                                      " knowledge about target");
                        result.Append("\n");
                    }
                    else if (primaryTargetIsOther){
                        result.Append("Target learns random " + GetTypeString(eff.PrimaryEnumValue) +
                                      " knowledge about you");
                        result.Append("\n");
                    }

                    break;

                default:
                    throw new NotImplementedException("add case for " +
                                                      eff.ActiveEffectType); // TODO: just add the rest later when not lazy. do for all switchs like this
            }
        }

        return result;
    }

    public static FixedString4096Bytes SetPassiveEffectsString(FixedList4096Bytes<PassiveEffectData> effects,
        FixedString4096Bytes result){
        for (var i = 0; i < effects.Length; i++){
            var eff = effects[i];
            result = SetPassiveEffectsString(eff, result);
        }

        return result;
    }

    public static FixedString4096Bytes SetPassiveEffectsString(PassiveEffectData eff, FixedString4096Bytes result){
        result.Append("\t - ");
        if (eff.ApplyToActionTarget){
            result.Append("Apply to Target ");
        }

        switch (eff.PassiveEffectTrigger){
            case PassiveEffectTriggerType.CONSTANT:
                break;
            case PassiveEffectTriggerType.ON_STAT_AFFECTED:
                result = BuildPassiveSubjectString(eff, result, "When ", " is affected, ");
                break;

            case PassiveEffectTriggerType.ON_STAT_DECREASED:
                result = BuildPassiveSubjectString(eff, result, "When ", " is lost, ");

                break;

            case PassiveEffectTriggerType.ON_STAT_INCREASED:
                result = BuildPassiveSubjectString(eff, result, "When ", " is gained, ");

                break;

            case PassiveEffectTriggerType.EFFECTS_OVER_TIME:
                result.Append("Every " + new TimeSpan(0, 0, (int) eff.PassiveEffectTriggerSecondsRate) + ", ");

                break;

            case PassiveEffectTriggerType.ON_AFFECT_OTHERS_STAT:
                result = BuildPassiveSubjectString(eff, result, "When you affect a target's ", ", ");

                break;

            case PassiveEffectTriggerType.ON_INCREASE_OTHERS_STAT:
                result = BuildPassiveSubjectString(eff, result, "When you increase a target's ", ", ");

                break;

            case PassiveEffectTriggerType.ON_DECREASE_OTHERS_STAT:
                result = BuildPassiveSubjectString(eff, result, "When you decrease a target's ", ", ");

                break;

            case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                result.Append("When performing any action, ");

                break;

            case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                result.Append("When performing common action, ");
                break;

            case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                result = BuildPassiveSubjectString(eff, result, "When performing ", " action, ");

                break;

            case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                result.Append("When performing any skill action, ");

                break;

            case PassiveEffectTriggerType.ON_ACTION_TYPE_PERFORM:
                result = BuildPassiveSubjectString(eff, result, "When performing ", " action, ");

                break;

            case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_INCREASES_STAT:
                result = BuildPassiveSubjectString(eff, result, "When target affects your ", ", ");

                break;

            default:
                //todo add remaining
                throw new NotImplementedException("add case for " +
                                                  eff.PassiveEffectTrigger); // TODO: just add the rest later when not lazy
        }

        result = BuildPassiveTypeString(eff, result);
        result.Append("\n");


        return result;
    }

    private static FixedString4096Bytes BuildPassiveTypeString(PassiveEffectData eff, FixedString4096Bytes result){
        switch (eff.PassiveEffectType){
            case PassiveEffectType.AFFECT_MAX_VALUE:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ",
                    " Max ");
                switch (eff.PassiveEffectSubject){
                    case PassiveEffectSubjectType.WELLNESS_STAT:
                        result.Append(eff.SecondaryEnumValue.IsBlank()
                            ? eff.PrimaryEnumValue.WellnessType.ToString()
                            : eff.SecondaryEnumValue.WellnessType.ToString());
                        break;
                    case PassiveEffectSubjectType.ATTRIBUTE:
                        result.Append(eff.SecondaryEnumValue.IsBlank()
                            ? eff.PrimaryEnumValue.AttributeType.ToString()
                            : eff.SecondaryEnumValue.AttributeType.ToString());
                        break;
                }

                break;
            case PassiveEffectType.AFFECT_INCOMING_VALUE:
                result = BuildPassiveAffectString(eff.PrimaryNumberRangeValue, result, " increased by ",
                    " decreased by ");
                break;
            case PassiveEffectType.TRIGGER_AFFECT_WELLNESS_VALUE:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ",
                    eff.SecondaryEnumValue.WellnessType.ToString());
                break;

            case PassiveEffectType.AFFECT_INFLUENCE_VALUE:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ", "Influence");
                break;

            case PassiveEffectType.TRIGGER_AFFECT_ATTRIBUTE_XP_VALUE:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ",
                    eff.SecondaryEnumValue.AttributeType + " XP");
                break;

            case PassiveEffectType.TRIGGER_AFFECT_SKILL_XP_VALUE:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ",
                    eff.SecondaryEnumValue.SkillType + " XP");
                break;

            case PassiveEffectType.TRIGGER_AFFECT_RELATIONSHIP_STAT_OF_SPECIFIC_TARGET:

                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ",
                    (eff.SecondaryEnumValue.IsBlank()
                        ? eff.PrimaryEnumValue.RelationshipStatType
                        : eff.SecondaryEnumValue.RelationshipStatType) + " for target");
                break;

            case PassiveEffectType.TRIGGER_LEARN_SKILL:
                result.Append("Learn ");
                result.Append(eff.SecondaryEnumValue.IsBlank()
                    ? eff.PrimaryEnumValue.SkillType.ToString()
                    : eff.SecondaryEnumValue.SkillType.ToString());
                break;
            
            case PassiveEffectType.TRIGGER_AFFECT_TRAIT_INTENSITY:
                result = BuildNumericChangeString(eff.PrimaryNumberRangeValue, result, false, "You ", GetTraitTypeString(eff.PrimaryEnumValue.GetTraitType()).Value);
                break;

            default:
                throw new NotImplementedException("cant build a passive for " + eff.PassiveEffectType);
        }

        return result;
    }

    private static FixedString4096Bytes BuildNumericChangeString(IntRangeStatData range, FixedString4096Bytes str,
        bool usePlural, string prefix = "", string suffix = ""){
        if (prefix.Length > 0){
            str.Append(prefix);
        }

        if (range.IsSingleValue){
            if (range.Min < 0){
                str.Append(usePlural ? " loses " : " lose ");
                str.Append(Mathf.Abs(range.Min) + " ");
            }
            else if (range.Min >= 0){
                str.Append(usePlural ? " gains " : " gain ");
                str.Append(range.Min + " ");
            }
        }
        else if (range.IsNegative){
            str.Append(usePlural ? " loses " : " lose ");
            str.Append(Mathf.Abs(range.Max) + " to " + Mathf.Abs(range.Min) + " ");
        }
        else if (range.IsPositive){
            str.Append(usePlural ? " gains " : " gain ");
            str.Append(range.Min + " to " + range.Max + " ");
        }
        else{ }

        if (suffix.Length > 0){
            str.Append(suffix);
        }

        return str;
    }

    private static FixedString4096Bytes BuildPassiveAffectString(IntRangeStatData range, FixedString4096Bytes str,
        string positivePrefix = "", string negativePrefix = ""){
        if (range.IsSingleValue){
            if (range.Min < 0){
                if (negativePrefix.Length > 0){
                    str.Append(negativePrefix);
                }

                str.Append(Mathf.Abs(range.Min));
            }
            else if (range.Min >= 0){
                if (positivePrefix.Length > 0){
                    str.Append(positivePrefix);
                }

                str.Append(range.Min + " ");
            }
        }
        else if (range.IsNegative){
            if (negativePrefix.Length > 0){
                str.Append(negativePrefix);
            }

            str.Append(Mathf.Abs(range.Max) + " to " + Mathf.Abs(range.Min));
        }
        else if (range.IsPositive){
            if (positivePrefix.Length > 0){
                str.Append(positivePrefix);
            }

            str.Append(range.Min + " to " + range.Max);
        }


        return str;
    }

    private static FixedString4096Bytes BuildPassiveSubjectString(PassiveEffectData data, FixedString4096Bytes str,
        string prefix = "", string suffix = ""){
        if (prefix.Length > 0){
            str.Append(prefix);
        }

        switch (data.PassiveEffectSubject){
            case PassiveEffectSubjectType.TRAIT:
                str.Append(data.PrimaryEnumValue.DefaultTraitCategory.ToString());
                break;
            case PassiveEffectSubjectType.SKILL_XP_STAT:
                str.Append(data.PrimaryEnumValue.SkillType + " XP");
                break;
            case PassiveEffectSubjectType.WELLNESS_STAT:
                str.Append(data.PrimaryEnumValue.WellnessType.ToString());
                break;
            case PassiveEffectSubjectType.ATTRIBUTE:
                str.Append(data.PrimaryEnumValue.AttributeType.ToString());
                break;
            case PassiveEffectSubjectType.ACTION_CATEGORY:
                str.Append(data.PrimaryEnumValue.ActionCategory.ToString());
                break;
            case PassiveEffectSubjectType.ATTRIBUTE_XP_STAT:
                str.Append(data.PrimaryEnumValue.AttributeType + " XP");
                break;
            case PassiveEffectSubjectType.RELATIONSHIP_STAT:
                str.Append(data.PrimaryEnumValue.RelationshipStatType.ToString());
                break;
            case PassiveEffectSubjectType.ACTION_FOCUS_COST:
                str.Append("focus cost");
                break;
            case PassiveEffectSubjectType.ACTION_ENERGY_COST:
                str.Append("energy cost");
                break;
            case PassiveEffectSubjectType.ACTION_PERFORM_TIME:
                str.Append("perform time");
                break;
            case PassiveEffectSubjectType.ACTION_DIFFICULTY_LEVEL:
                str.Append("difficulty level");
                break;
            case PassiveEffectSubjectType.ACTION_SUCCESS_CHANCE:
                str.Append("success chance");
                break;
            case PassiveEffectSubjectType.ACTION_INFLUENCE_SUCCESS_CHANCE:
                str.Append("influence");
                break;
            case PassiveEffectSubjectType.ACTION_PERFORMED:
                str.Append("on performance completed");
                break;
            default:
                throw new NotImplementedException("Add case for " + data.PassiveEffectSubject);
        }


        if (suffix.Length > 0){
            str.Append(suffix);
        }

        return str;
    }

    private static FixedString4096Bytes SetNumericComparisionString(NumericComparisonSign numericComparisonSign,
        FixedString4096Bytes result, bool boolValue){
        if (numericComparisonSign == NumericComparisonSign.EQUALS){
            result.Append(!boolValue ? "is not" : "is");
        }
        else if (numericComparisonSign == NumericComparisonSign.LESS_THAN){
            result.Append(!boolValue ? "is not less than" : "is less than");
        }
        else if (numericComparisonSign == NumericComparisonSign.LESS_THAN_EQUAL){
            result.Append(!boolValue ? "is not less than/equal to" : "is less than/equal to");
        }
        else if (numericComparisonSign == NumericComparisonSign.GREATER_THAN){
            result.Append(!boolValue ? "is not more than" : "is more than");
        }
        else if (numericComparisonSign == NumericComparisonSign.GREATER_THAN_EQUAL){
            result.Append(!boolValue ? "is not more than/equal to" : "is more than/equal to");
        }

        return result;
    }

    public static FixedString128Bytes GetPassiveSourceString(int val, PassiveEffectComponent passive){
        var displayText = new FixedString128Bytes();
        displayText.Append("\n");
        displayText.Append(val > 0 ? "+" + val : val.ToString());
        displayText.Append(" (");
        switch (passive.SourceType){
            case PassiveEffectSourceType.TRAIT:
                displayText.Append(GetTraitTypeString(passive.SourcePrimaryEnum.GetTraitType()));
                break;
            case PassiveEffectSourceType.INTEREST:
                displayText.Append("Interest in ");
                switch (passive.SourcePrimaryEnum.InterestSubjectType){
                    case InterestSubjectType.ITEM:
                        displayText.Append(
                            GetItemTypeString(passive.SourceSecondaryEnum.GetItemType()));
                        break;
                    case InterestSubjectType.SKILL:
                        displayText.Append(
                            passive.SourceSecondaryEnum.SkillType.ToString());
                        break;
                    case InterestSubjectType.WELLNESS:
                        displayText.Append(
                            passive.SourceSecondaryEnum.WellnessType.ToString());
                        break;
                    case InterestSubjectType.ATTRIBUTE:
                        displayText.Append(
                            passive.SourceSecondaryEnum.AttributeType.ToString());
                        break;
                    case InterestSubjectType.GENERAL_INTEREST:
                        displayText.Append(
                            passive.SourceSecondaryEnum.GeneralInterestType.ToString());
                        break;
                }

                break;
            case PassiveEffectSourceType.EQUIPPED_ITEM:
                displayText.Append("Equipment ");
                displayText.Append(GetItemTypeString(passive.SourcePrimaryEnum.GetItemType()));
                break;
            case PassiveEffectSourceType.ACTION:
                displayText.Append(GetActionTypeString(passive.SourcePrimaryEnum.GetActionType()));
                break;
            case PassiveEffectSourceType.OTHERS_ACTION:
                displayText.Append("Target of ");
                displayText.Append(GetActionTypeString(passive.SourcePrimaryEnum.GetActionType()));
                break;
            default:
                displayText.Append("???");
                break;
        }

        displayText.Append(")");
        return displayText;
    }
}