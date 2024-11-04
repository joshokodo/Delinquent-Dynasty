using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct PassiveEffectsUtils {
    public DynamicBuffer<CharacterAttributeElement> CharacterAttributes;
    public DynamicBuffer<WellnessElement> CharacterWellness;
    public DynamicBuffer<TraitElement> Traits;
    public DynamicBuffer<PassiveEffectElement> Passives;
    public DynamicBuffer<SkillElement> Skills;

    public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;

    public SkillUtils SkillUtils;
    public TraitUtils TraitUtils;

    public bool CanBuildRecipe(DynamicItemType itemType, CraftingDataStore craftingDataStore,
        out CraftingRecipeAssetData data){
        data = craftingDataStore.CraftingBlobAssets.Value.GetRecipeData(itemType);
        if (data.SuccessfulProduct.IsNull){
            return false;
        }

        var skillLevel = GetNaturalAndBonusSkillLevel(data.RequiredSkillType);
        return skillLevel > 0 && skillLevel >= data.RequiredSkillLevel;
    }

    public int GetNaturalAndBonusAttributeTotal(AttributeType type, float baseAttributePercent = 1f){
        var attributesUtil = new CharacterAttributesUtil(){
            Attributes = CharacterAttributes
        };
        var finalLevel = attributesUtil.GetLevel(type);

        finalLevel = Mathf.CeilToInt(finalLevel * baseAttributePercent);
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ATTRIBUTE){
                continue;
            }

            if (passive.Data.PassiveEffectTrigger == PassiveEffectTriggerType.CONSTANT &&
                passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_MAX_VALUE &&
                passive.Data.PrimaryEnumValue.AttributeType == type){
                finalLevel += GetFlatBonus(passive);
            }
        }

        return finalLevel;
    }

    public int GetNaturalAndBonusAttributeTotal(AttributeType type, out FixedString4096Bytes displayText,
        float baseAttributePercent = 1f){
        var attributesUtil = new CharacterAttributesUtil(){
            Attributes = CharacterAttributes
        };
        var finalLevel = attributesUtil.GetLevel(type);
        displayText = new FixedString4096Bytes(finalLevel + " (Natural Level)");

        finalLevel = Mathf.CeilToInt(finalLevel * baseAttributePercent);
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ATTRIBUTE){
                continue;
            }

            if (passive.Data.PassiveEffectTrigger == PassiveEffectTriggerType.CONSTANT &&
                passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_MAX_VALUE &&
                passive.Data.PrimaryEnumValue.AttributeType == type){
                var val = GetFlatBonus(passive);
                finalLevel += val;
                displayText.Append(StringUtils.GetPassiveSourceString(val, passive));
            }
        }

        return finalLevel;
    }

    public int OnAffectAttributeXp(ActiveEffectData data, int number, Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ATTRIBUTE_XP_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.AttributeType == data.PrimaryEnumValue.AttributeType){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_STAT_AFFECTED:
                    case PassiveEffectTriggerType.ON_STAT_INCREASED:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                }
            }
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int OnAffectSkillXp(ActiveEffectData data, int number, Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.SKILL_XP_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.SkillType == data.PrimaryEnumValue.SkillType){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_STAT_AFFECTED:
                    case PassiveEffectTriggerType.ON_STAT_INCREASED:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                }
            }
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int OnAffectOtherWellness(ActiveEffectData data, WellnessType type, int number, Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.WELLNESS_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.WellnessType == type){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_AFFECT_OTHERS_STAT:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                    case PassiveEffectTriggerType.ON_INCREASE_OTHERS_STAT:
                        if (isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                    case PassiveEffectTriggerType.ON_DECREASE_OTHERS_STAT:
                        if (!isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_AFFECT_SPECIFIC_TARGETS_STAT:
                        if (passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_INCREASE_SPECIFIC_TARGETS_STAT:
                        if (isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_DECREASE_SPECIFIC_TARGETS_STAT:
                        if (!isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                }
            }
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int GetNaturalAndBonusSkillPower(SkillType skillType){
        if (skillType == SkillType.COMMON){
            return 0;
        }

        return GetSkillPower(skillType);
    }

    public int GetNaturalAndBonusSkillLevel(SkillType skillType){
        if (skillType == SkillType.COMMON){
            return 0;
        }

        foreach (var skillElement in Skills){
            if (skillElement.SkillType == skillType){
                return skillElement.CurrentLevel;
            }
        }

        return 0;
    }

    public int GetNaturalAndBonusSkillLevel(SkillElement skillElement){
        return skillElement.CurrentLevel;
    }


    public float GetSkillSuccessChance(SkillType skillType, int difficultyLevel){
        if (skillType == SkillType.COMMON){
            return 100f;
        }

        if (SkillUtils.TryGetSkillElement(skillType, Skills, out SkillElement skill)){
            var skillPower = GetSkillPower(skill.SkillType);
            var bonus = GetBonusSuccessChance(skillType);
            var bonusSuccess = (int) bonus;
            var newDifficulty = GetNaturalAndBonusDifficultyLevel(difficultyLevel, skillType);
            var successChance = newDifficulty <= 0
                ? 100f
                : 100 * Mathf.Clamp((float) skillPower / newDifficulty, 0f, 1f);
            successChance = Mathf.Round(successChance);
            successChance = Mathf.Clamp(successChance + bonus, 0, 100);
            return Mathf.Round(successChance);
        }

        return 0f;
    }

    public float GetSkillSuccessChanceWithString(SkillType skillType, int difficultyLevel,
        DynamicBuffer<SkillElement> skills, out int baseSuccessChance, out int skillPower, out int newDifficulty,
        out int bonusSuccess, out FixedString4096Bytes bonusSuccessBreakdown,
        out FixedString4096Bytes skillPowerBreakdown, out FixedString4096Bytes difficultyBreakdown){
        if (SkillUtils.TryGetSkillElement(skillType, skills, out SkillElement skill)){
            skillPower = GetSkillPowerWithString(skill, out skillPowerBreakdown);
            var bonus = GetBonusSuccessChanceWithString(skillType, out bonusSuccessBreakdown);
            bonusSuccess = (int) bonus;
            newDifficulty =
                GetNaturalAndBonusDifficultyLevelWithString(difficultyLevel, skillType, out difficultyBreakdown);
            var successChance = newDifficulty <= 0
                ? 100f
                : 100 * Mathf.Clamp((float) skillPower / newDifficulty, 0f, 1f);
            successChance = Mathf.Round(successChance);
            baseSuccessChance = (int) successChance;
            successChance = Mathf.Clamp(successChance + bonus, 0, 100);
            return Mathf.Round(successChance);
        }

        skillPowerBreakdown = new();
        difficultyBreakdown = new();
        bonusSuccessBreakdown = new();
        newDifficulty = difficultyLevel;
        bonusSuccess = 0;
        skillPower = 0;
        baseSuccessChance = 0;
        return 0f;
    }

    public int GetSkillPower(SkillType skill){
        var skillPower = GetSkillPower(skill, out _, out _, out SkillBaseAssetData data);

        if (data.SecondaryAttribute == AttributeType.NONE) return skillPower;

        return skillPower;
    }

    public int GetSkillPowerWithString(SkillElement skill, out FixedString4096Bytes displayText){
        var skillPower =
            GetSkillPower(skill.SkillType, out int primary, out int secondary, out SkillBaseAssetData data);
        displayText = new FixedString4096Bytes((skillPower >= 0 ? "+" : "") + primary + " (Primary Skill Attribute " +
                                               data.PrimaryAttribute + ")");

        if (data.SecondaryAttribute == AttributeType.NONE) return skillPower;

        displayText.Append("\n");
        displayText.Append((skillPower >= 0 ? "+" : "") + secondary + " (Secondary Skill Attribute " +
                           data.SecondaryAttribute + ")");
        return skillPower;
    }

    public int GetSkillPower(SkillType skill, out int primarySkillPower, out int secondarySkillPower,
        out SkillBaseAssetData data){
        data = SkillUtils.SkillDataStore.SkillBlobAssets.Value.GetSkillData(skill);
        var baseAttributePercentage = 1f;
        secondarySkillPower = 0;

        primarySkillPower = GetNaturalAndBonusAttributeTotal(data.PrimaryAttribute, baseAttributePercentage);
        secondarySkillPower = GetNaturalAndBonusAttributeTotal(data.SecondaryAttribute, baseAttributePercentage);
        var finalValue = data.IsDynamic
            ? Mathf.Max(primarySkillPower, secondarySkillPower)
            : primarySkillPower + secondarySkillPower;

        return Mathf.Max(finalValue, 0);
    }

    public int OnWellnessAffected(ActiveEffectData data, int number, Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn, RefRW<RandomComponent> random, out int totalBonus){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.WELLNESS_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.WellnessType == data.PrimaryEnumValue.WellnessType){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_STAT_AFFECTED:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                    case PassiveEffectTriggerType.ON_STAT_INCREASED:
                        if (isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                    case PassiveEffectTriggerType.ON_STAT_DECREASED:
                        if (!isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_AFFECTS_STAT:
                        if (passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_DECREASES_STAT:
                        if (!isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_INCREASES_STAT:
                        if (isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                }
            }
        }

        totalBonus = bonus;
        if (percentage > 0){
            totalBonus += Mathf.CeilToInt(totalBonus * (percentage * 0.01f));
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int OnAffectOtherRelationshipStat(ActiveEffectData data, RelationshipStatType type, int number,
        Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.RELATIONSHIP_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.RelationshipStatType == type){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_AFFECT_OTHERS_STAT:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                    case PassiveEffectTriggerType.ON_INCREASE_OTHERS_STAT:
                        if (isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                    case PassiveEffectTriggerType.ON_DECREASE_OTHERS_STAT:
                        if (!isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_AFFECT_SPECIFIC_TARGETS_STAT:
                        if (passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_INCREASE_SPECIFIC_TARGETS_STAT:
                        if (isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_DECREASE_SPECIFIC_TARGETS_STAT:
                        if (!isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                }
            }
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int OnRelationshipStatAffected(ActiveEffectData data, int number, Entity origin, Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn, out int totalBonus){
        var percentage = 0;
        var bonus = 0;
        var isPositive = number >= 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.RELATIONSHIP_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.RelationshipStatType == data.PrimaryEnumValue.RelationshipStatType){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.ON_STAT_AFFECTED:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                            bonus += GetFlatBonus(passive);
                            percentage = GetPercentBonus(passive, percentage);
                        }
                        else{
                            TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                activeEffectsSpawn, origin);
                        }

                        break;
                    case PassiveEffectTriggerType.ON_STAT_INCREASED:
                        if (isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                    case PassiveEffectTriggerType.ON_STAT_DECREASED:
                        if (!isPositive){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_AFFECTS_STAT:
                        if (passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_DECREASES_STAT:
                        if (!isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;

                    case PassiveEffectTriggerType.ON_SPECIFIC_TARGETS_INCREASES_STAT:
                        if (isPositive && passive.SecondaryTarget == target && passive.PrimaryTarget == origin){
                            if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                                bonus += GetFlatBonus(passive);
                                percentage = GetPercentBonus(passive, percentage);
                            }
                            else{
                                TriggerPassiveActiveEffect(data.IsPassiveActiveTriggered, target, passive,
                                    activeEffectsSpawn, origin);
                            }
                        }

                        break;
                }
            }
        }

        totalBonus = bonus;
        if (percentage > 0){
            totalBonus += Mathf.CeilToInt(totalBonus * (percentage * 0.01f));
        }

        number += bonus;
        number += Mathf.CeilToInt(number * (percentage * 0.01f));
        if (isPositive && number < 0 || (!isPositive && number > 0)){
            number = 0;
        }

        return number;
    }

    public int GetNaturalAndBonusDifficultyLevel(int difficultyLevel, SkillType skillType){
        var percentage = 0;
        var bonus = 0;

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            var nextText = new FixedString128Bytes();
            if (!passive.IsValid || passive.Data.PassiveEffectSubject !=
                PassiveEffectSubjectType.ACTION_DIFFICULTY_LEVEL){
                continue;
            }

            // todo: IMPORTANT! find all switches in this file that use on all action perform and add case for ON_ACTION_TYPE_PERFORM or any others that make sense
            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

            }

            if (!nextText.IsEmpty){ }
        }

        difficultyLevel += bonus;
        difficultyLevel += Mathf.CeilToInt(difficultyLevel * (percentage * 0.01f));
        if (difficultyLevel < 0){
            difficultyLevel = 0;
        }

        return difficultyLevel;
    }

    public int GetNaturalAndBonusDifficultyLevelWithString(int difficultyLevel, SkillType skillType,
        out FixedString4096Bytes displayText){
        var percentage = 0;
        var bonus = 0;
        displayText = new FixedString4096Bytes("Difficult Level: " + difficultyLevel);

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            var nextText = new FixedString128Bytes();
            if (!passive.IsValid || passive.Data.PassiveEffectSubject !=
                PassiveEffectSubjectType.ACTION_DIFFICULTY_LEVEL){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out nextText);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out nextText);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out nextText);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
            }

            if (!nextText.IsEmpty){
                displayText.Append("\n" + nextText.Value);
                displayText.Append(nextText.Value);
            }
        }

        difficultyLevel += bonus;
        difficultyLevel += Mathf.CeilToInt(difficultyLevel * (percentage * 0.01f));
        if (difficultyLevel < 0){
            difficultyLevel = 0;
        }

        return difficultyLevel;
    }

    public int GetNaturalAndBonusCost(int cost, bool isFocus, SkillType skillType){
        var percentage = 0;
        var bonus = 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || (isFocus &&
                                     passive.Data.PassiveEffectSubject !=
                                     PassiveEffectSubjectType.ACTION_FOCUS_COST)
                                 || (!isFocus && passive.Data.PassiveEffectSubject !=
                                     PassiveEffectSubjectType.ACTION_ENERGY_COST)){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType
                        && skillType != SkillType.COMMON
                        && passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
            }
        }

        cost += bonus;
        cost += Mathf.CeilToInt(cost * (percentage * 0.01f));
        if (cost > 0){
            cost = 0;
        }

        return cost;
    }

    public float GetBonusSuccessChance(SkillType skillType){
        var bonus = 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid ||
                passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_SUCCESS_CHANCE){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
            }
        }

        return bonus;
    }

    public float GetBonusSuccessChanceWithString(SkillType skillType, out FixedString4096Bytes displayText){
        var bonus = 0;
        displayText = new();
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid ||
                passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_SUCCESS_CHANCE){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        displayText.Append(val.Value + "\n");
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        displayText.Append(val.Value + "\n");
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        displayText.Append(val.Value + "\n");
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        displayText.Append(val.Value + "\n");
                    }

                    break;
            }
        }

        return bonus;
    }

    public int GetNaturalAndBonusInfluence(BufferLookup<RelationshipElement> relationshipsLookup, Entity origin,
        Entity target, SkillType skillType = SkillType.COMMON, DynamicActionType dynamicActionType = default, bool checkOnTriggerEffects = true){
        var influence = 0;
        var bonus = 0;
        var targRel = relationshipsLookup[target];

        foreach (var relationshipElement in targRel){
            if (relationshipElement.Character == origin){
                influence = relationshipElement.TotalInfluence;
                break;
            }
        }

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.CONSTANT:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (checkOnTriggerEffects &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == passive.Data.PrimaryEnumValue.SkillType &&
                        skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ACTION_TYPE_PERFORM:
                    if (checkOnTriggerEffects && dynamicActionType.Matches(passive.Data.PrimaryActionType) &&
                        dynamicActionType.Category != ActionCategory.NONE &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
            }
        }

        return influence + bonus;
    }
    
    public int GetNaturalAndBonusInfluence(RelationshipElement relationshipElement, SkillType skillType = SkillType.COMMON, DynamicActionType dynamicActionType = default, bool checkOnTriggerEffects = true){
        var influence = 0;
        var bonus = 0;
        influence = relationshipElement.TotalInfluence;

        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.CONSTANT:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (checkOnTriggerEffects &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == passive.Data.PrimaryEnumValue.SkillType &&
                        skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ACTION_TYPE_PERFORM:
                    if (checkOnTriggerEffects && dynamicActionType.Matches(passive.Data.PrimaryActionType) &&
                        dynamicActionType.Category != ActionCategory.NONE &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonus(passive);
                    }

                    break;
            }
        }

        return influence + bonus;
    }

    public int GetNaturalAndBonusInfluenceWithString(BufferLookup<RelationshipElement> relationshipsLookup,
        Entity origin, Entity target, SkillType skillType, DynamicActionType dynamicActionType, out int baseInfluence,
        out FixedString4096Bytes displayText, bool checkOnTriggerEffects = true){
        var influence = 0;
        var bonus = 0;
        var targRel = relationshipsLookup[target];
        var origRel = relationshipsLookup[origin];
        displayText = new();

        foreach (var relationshipElement in targRel){
            if (relationshipElement.Character == origin){
                influence = relationshipElement.TotalInfluence;
                break;
            }
        }

        foreach (var relationshipElement in origRel){
            if (relationshipElement.Character == target){
                influence += relationshipElement.Resentment + relationshipElement.Entitlement;
                break;
            }
        }

        baseInfluence = influence;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];
            if (!passive.IsValid){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.CONSTANT:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                        
                    }

                    break;
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (checkOnTriggerEffects &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == passive.Data.PrimaryEnumValue.SkillType &&
                        skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (checkOnTriggerEffects && skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                    }

                    break;

                case PassiveEffectTriggerType.ON_ACTION_TYPE_PERFORM:
                    if (checkOnTriggerEffects && dynamicActionType.Matches(passive.Data.PrimaryActionType) &&
                        dynamicActionType.Category != ActionCategory.NONE &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INFLUENCE_VALUE){
                        bonus += GetFlatBonusWithString(passive, out FixedString128Bytes val);
                        if (!passive.PassiveTitle.IsEmpty){
                            displayText.Append(val.Value + " " + passive.PassiveTitle + "\n");
                        } else {
                            displayText.Append(StringUtils.GetPassiveSourceString(passive.NextIntValue, passive));
                        }
                    }

                    break;
            }
        }

        return influence + bonus;
    }
    
    public void TriggerOnActionPerformEffects(Entity target,
        DynamicBuffer<ActiveEffectSpawnElement> activeEffectsSpawn, SkillType skillType, DynamicActionType actionType, Entity source = default){
      
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid ||
                passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_PERFORMED){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    TriggerPassiveActiveEffect(false, target, passive, activeEffectsSpawn, source);

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON){
                        TriggerPassiveActiveEffect(false, target, passive, activeEffectsSpawn, source);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON){
                        TriggerPassiveActiveEffect(false, target, passive, activeEffectsSpawn, source);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON){
                        TriggerPassiveActiveEffect(false, target, passive, activeEffectsSpawn, source);
                    }

                    break;
                
                case PassiveEffectTriggerType.ON_ACTION_TYPE_PERFORM:
                    if (actionType.Matches(passive.Data.PrimaryActionType) &&
                        actionType.Category != ActionCategory.NONE){
                        TriggerPassiveActiveEffect(false, target, passive, activeEffectsSpawn, source);
                    }

                    break;
            }
        }

    }

    public double GetNaturalAndBonusPerformTime(double performTime, SkillType skillType){
        if (performTime == 0){
            return 0;
        }

        var percentage = 0;
        var bonus = 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid ||
                passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_PERFORM_TIME){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.CONSTANT:
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
            }
        }

        performTime += bonus;
        performTime += Mathf.CeilToInt((float) performTime * (percentage * 0.01f));
        if (performTime < 0){
            performTime = 1;
        }

        return performTime;
    }

    public double GetNaturalAndBonusPerformTimeWithString(double performTime, SkillType skillType,
        out FixedString4096Bytes displayText){
        displayText = new FixedString4096Bytes(TimeUtils.GetGameTimeSpanString(performTime).Value + "\n");
        if (performTime == 0){
            return 0;
        }

        var percentage = 0;
        var bonus = 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid ||
                passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_PERFORM_TIME){
                continue;
            }

            switch (passive.Data.PassiveEffectTrigger){
                case PassiveEffectTriggerType.CONSTANT:
                case PassiveEffectTriggerType.ON_ALL_ACTION_PERFORM:
                    if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        displayText.Append((bonusVal < 0 ? "-" : "+") +
                                           TimeUtils.GetGameTimeSpanString(bonusVal).Value + "\n");
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
                case PassiveEffectTriggerType.ON_SKILL_ACTION_PERFORM:
                    if (skillType == passive.Data.PrimaryEnumValue.SkillType && skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        displayText.Append((bonusVal < 0 ? "-" : "+") +
                                           TimeUtils.GetGameTimeSpanString(bonusVal).Value + "\n");
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_ALL_SKILL_ACTIONS_PERFORM:
                    if (skillType != SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        displayText.Append((bonusVal < 0 ? "-" : "+") +
                                           TimeUtils.GetGameTimeSpanString(bonusVal).Value + "\n");
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;

                case PassiveEffectTriggerType.ON_NON_SKILL_ACTION_PERFORM:
                    if (skillType == SkillType.COMMON &&
                        passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_INCOMING_VALUE){
                        var bonusVal = GetFlatBonus(passive);
                        displayText.Append((bonusVal < 0 ? "-" : "+") +
                                           TimeUtils.GetGameTimeSpanString(bonusVal).Value + "\n");
                        bonus += bonusVal;
                        percentage = GetPercentBonus(passive, percentage);
                    }

                    break;
            }
        }

        performTime += bonus;
        performTime += Mathf.CeilToInt((float) performTime * (percentage * 0.01f));
        if (performTime < 0){
            performTime = 1;
        }

        return performTime;
    }

    public int GetBonusWellnessMaxTotal(WellnessType wellnessType){
        var bonus = 0;
        for (var i = 0; i < Passives.Length; i++){
            var passive = PassiveCompLookup[Passives[i].EffectEntity];

            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.WELLNESS_STAT){
                continue;
            }

            if (passive.Data.PrimaryEnumValue.WellnessType == wellnessType){
                switch (passive.Data.PassiveEffectTrigger){
                    case PassiveEffectTriggerType.CONSTANT:
                        if (passive.Data.PassiveEffectType == PassiveEffectType.AFFECT_MAX_VALUE){
                            bonus += GetFlatBonus(passive);
                        }

                        break;
                }
            }
        }

        return bonus;
    }

    // todo this is redundant, if we are keeping all passive intensity in intensity buffer and nothing specail is happening. remove this method and have them call traitutils directly
    public int GetTotalTraitIntensity(DynamicTraitType traitType){
        return TraitUtils.GetTotalIntensity(Traits, traitType);
    }

    public bool OnCheckActionForcedDisabled(DynamicActionType type, ActionDataStore store){
        return OnCheckActionForcedDisabled(store.ActionsBlobAssets.Value.GetActionBaseData(type).ActionType.Category);
    }

    public bool OnCheckActionForcedDisabled(ActionCategory actionCategory){
        foreach (var passiveEffectElement in Passives){
            var passive = PassiveCompLookup[passiveEffectElement.EffectEntity];
            if (!passive.IsValid || passive.Data.PassiveEffectSubject != PassiveEffectSubjectType.ACTION_CATEGORY){
                continue;
            }

            if (passive.Data.PassiveEffectType == PassiveEffectType.DISABLE
                && passive.Data.PrimaryEnumValue.ActionCategory == actionCategory){
                return true;
            }
        }

        return false;
    }

    public bool HasPassive(Guid dataId){
        var found = false;
        foreach (var passiveEffectElement in Passives){
            var data = PassiveCompLookup[passiveEffectElement.EffectEntity];
            if (data.PassiveAssetId == dataId){
                found = true;
                break;
            }
        }

        return found;
    }

    public bool HasPassiveByAction(Guid actId){
        var found = false;
        foreach (var passiveEffectElement in Passives){
            var data = PassiveCompLookup[passiveEffectElement.EffectEntity];
            if (data.OriginActionId == actId){
                found = true;
                break;
            }
        }

        return found;
    }

    public void TriggerPassiveActiveEffect(bool isPassiveActiveTriggered, Entity effectTarget,
        PassiveEffectComponent passiveEffectComponent,
        DynamicBuffer<ActiveEffectSpawnElement> spawn, Entity effectOrigin = default){
        if (isPassiveActiveTriggered) return;

        var targets = new TargetsGroup();
        var secondaryEnumType = passiveEffectComponent.Data.SecondaryEnumValue.Type;
        switch (passiveEffectComponent.Data.PassiveEffectType){
            case PassiveEffectType.TRIGGER_AFFECT_WELLNESS_VALUE:

                var affectWellnessType = secondaryEnumType == GameEnumType.WELLNESS_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;


                targets.SetSingleTarget(TargetType.SELF, effectTarget);
                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = effectOrigin,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_WELLNESS_OF_X,
                                PrimaryEnumValue = affectWellnessType,
                                EffectPrimaryTarget = TargetType.SELF,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        },
                    }
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_TRAIT_INTENSITY:
                var affectTraitType = secondaryEnumType == GameEnumType.STATUS_TRAIT_CATEGORY ||
                                      secondaryEnumType == GameEnumType.PERSONAL_MAINTENANCE_TRAIT_CATEGORY
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;
                
                targets.SetSingleTarget(TargetType.SELF, effectTarget);
                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = effectOrigin,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_TRAIT_INTENSITY_OF_X,
                                PrimaryEnumValue = affectTraitType,
                                EffectPrimaryTarget = TargetType.SELF,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        }
                    }
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_WELLNESS_VALUE_OF_ORIGIN:
                var affectOriginWellnessType = secondaryEnumType == GameEnumType.WELLNESS_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;
                targets.SetSingleTarget(TargetType.TARGET_CHARACTER, effectOrigin);

                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = effectOrigin,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_WELLNESS_OF_X,
                                PrimaryEnumValue = affectOriginWellnessType,
                                EffectPrimaryTarget = TargetType.TARGET_CHARACTER,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        }
                    },
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_ATTRIBUTE_XP_VALUE:

                var affectAttributeType = secondaryEnumType == GameEnumType.ATTRIBUTE_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;


                targets.SetSingleTarget(TargetType.SELF, effectTarget);
                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = effectOrigin,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_ATTRIBUTE_XP_OF_X,
                                PrimaryEnumValue = affectAttributeType,
                                EffectPrimaryTarget = TargetType.SELF,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        }
                    }
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_SKILL_XP_VALUE:

                var affectSkillType = secondaryEnumType == GameEnumType.SKILL_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;


                targets.SetSingleTarget(TargetType.SELF, effectTarget);
                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = effectOrigin,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_SKILL_XP_OF_X,
                                PrimaryEnumValue = affectSkillType,
                                EffectPrimaryTarget = TargetType.SELF,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        }
                    }
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_RELATIONSHIP_STAT_OF_SPECIFIC_TARGET:
                var affectRelType = secondaryEnumType == GameEnumType.RELATIONSHIP_STAT_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;

                targets.AddTarget(passiveEffectComponent.PrimaryTargetType, passiveEffectComponent.PrimaryTarget);
                targets.AddTarget(passiveEffectComponent.SecondaryTargetType, passiveEffectComponent.SecondaryTarget);

                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = passiveEffectComponent.PrimaryTarget,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_X_RELATIONSHIP_STAT_OF_Y,
                                PrimaryEnumValue = affectRelType,
                                EffectPrimaryTarget = TargetType.SELF,
                                EffectSecondaryTarget = TargetType.TARGET_CHARACTER,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        },
                    }
                });
                break;

            case PassiveEffectType.TRIGGER_AFFECT_RELATIONSHIP_STAT_OF_ORIGIN:
            case PassiveEffectType.TRIGGER_AFFECT_ORIGIN_RELATIONSHIP_STAT_OF_YOU:
                var relType = secondaryEnumType == GameEnumType.RELATIONSHIP_STAT_TYPE
                    ? passiveEffectComponent.Data.SecondaryEnumValue
                    : passiveEffectComponent.Data.PrimaryEnumValue;

                var affectOrigin = passiveEffectComponent.Data.PassiveEffectType ==
                                   PassiveEffectType.TRIGGER_AFFECT_RELATIONSHIP_STAT_OF_ORIGIN;
                targets.AddTarget(TargetType.SELF, affectOrigin ? effectTarget : effectOrigin);
                targets.AddTarget(TargetType.TARGET_CHARACTER, affectOrigin ? effectOrigin : effectTarget);

                spawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        TargetsData = targets,
                        SourceEntity = affectOrigin ? effectOrigin : effectTarget,
                        Effects = new FixedList512Bytes<ActiveEffectData>(){
                            new(){
                                ActiveEffectType = ActiveEffectType.AFFECT_X_RELATIONSHIP_STAT_OF_Y,
                                PrimaryEnumValue = relType,
                                EffectPrimaryTarget = TargetType.SELF,
                                EffectSecondaryTarget = TargetType.TARGET_CHARACTER,
                                PrimaryNumberRangeValue = new IntRangeStatData(){
                                    Max = passiveEffectComponent.NextIntValue,
                                    Min = passiveEffectComponent.NextIntValue
                                },
                                DisplayToWorld = passiveEffectComponent.Data.DisplayTriggeredEffect,
                                SourceLearnsKnowledge = passiveEffectComponent.Data.SourcesLearnEffect,
                                IsPassiveActiveTriggered = true,
                            }
                        },
                    }
                });
                break;
        }
    }

    private int GetFlatBonus(PassiveEffectComponent passive, DynamicTraitType traitType = default){
        var bonus = 0;
        var wellnessUtils = new WellnessUtils(){
            Wellness = CharacterWellness
        };

        switch (passive.Data.BonusNumericType){
            case NumberScaleType.FLAT:
                bonus += passive.NextIntValue;
                break;

            case NumberScaleType.SCALE_BY_TOTAL_ATTRIBUTE_FLAT:
                var attributeType = passive.Data.BonusEnumValue.AttributeType;
                var attributeTotalLvl = GetNaturalAndBonusAttributeTotal(
                    attributeType);
                var scale = (attributeTotalLvl * passive.NextIntValue);
                bonus += scale;
                break;

            case NumberScaleType.SCALE_BY_TRAIT_INTENSITY_FLAT:
                if (passive.Data.BonusEnumValue.GetTraitType().Matches(traitType) && !traitType.IsNull){
                    throw new Exception("Cannot have trait intensity scale from same trait type");
                }

                var intensity =
                    GetTotalTraitIntensity(passive.Data.BonusEnumValue.GetTraitType());
                var intensityScaleVal = (passive.NextIntValue * (intensity < 0 ? 0 : intensity));
                bonus += intensityScaleVal;
                break;

            case NumberScaleType.SCALE_BY_CURRENT_WELLNESS_FLAT:
                var wellnessScaleVal = (passive.NextIntValue *
                                        wellnessUtils.GetCurrentValue(passive.Data.BonusEnumValue.WellnessType));
                bonus += wellnessScaleVal;
                break;

            case NumberScaleType.SCALE_BY_WELLNESS_DEFICIT_FLAT:
                var max = GetBonusWellnessMaxTotal(passive.Data.BonusEnumValue.WellnessType);
                var current = wellnessUtils.GetCurrentValue(passive.Data.BonusEnumValue.WellnessType);
                var wellnessDeficitValue = (passive.NextIntValue * (max - current));
                bonus += wellnessDeficitValue;
                break;

            case NumberScaleType.SCALE_BY_SKILL_POWER:
                var skillPower = GetSkillPower(passive.Data.BonusEnumValue.SkillType, out _,
                    out _, out _);
                bonus += (passive.NextIntValue * skillPower);
                break;
        }

        return bonus;
    }

    private int GetFlatBonusWithString(PassiveEffectComponent passive, out FixedString128Bytes text,
        DynamicTraitType traitType = default){
        var bonus = 0;
        var wellnessUtils = new WellnessUtils(){
            Wellness = CharacterWellness
        };
        text = new FixedString128Bytes();
        switch (passive.Data.BonusNumericType){
            case NumberScaleType.FLAT:
                bonus += passive.NextIntValue;
                text = new FixedString128Bytes((passive.NextIntValue >= 0 ? "+" : "") + passive.NextIntValue);
                break;

            case NumberScaleType.SCALE_BY_TOTAL_ATTRIBUTE_FLAT:
                var attributeType = passive.Data.BonusEnumValue.AttributeType;
                var attributeTotalLvl = GetNaturalAndBonusAttributeTotal(
                    attributeType);
                var scale = (attributeTotalLvl * passive.NextIntValue);
                bonus += scale;
                text = new FixedString128Bytes((scale >= 0 ? "(+" : "(") + (scale) + ") " + passive.NextIntValue + "x" +
                                               attributeType);
                break;

            case NumberScaleType.SCALE_BY_TRAIT_INTENSITY_FLAT:
                if (passive.Data.BonusEnumValue.GetTraitType().Matches(traitType) && !traitType.IsNull){
                    throw new Exception("Cannot have trait intensity scale from same trait type");
                }

                var intensity =
                    GetTotalTraitIntensity(passive.Data.BonusEnumValue.GetTraitType());
                var intensityScaleVal = (passive.NextIntValue * (intensity < 0 ? 0 : intensity));
                bonus += intensityScaleVal;
                text = new FixedString128Bytes((intensityScaleVal >= 0 ? "(+" : "(") + intensityScaleVal + ") " +
                                               passive.NextIntValue + "x" +
                                               passive.Data.BonusEnumValue.DefaultTraitCategory + " INTENSITY");
                break;

            case NumberScaleType.SCALE_BY_CURRENT_WELLNESS_FLAT:
                var wellnessScaleVal = (passive.NextIntValue *
                                        wellnessUtils.GetCurrentValue(passive.Data.BonusEnumValue.WellnessType));
                bonus += wellnessScaleVal;
                text = new FixedString128Bytes((wellnessScaleVal >= 0 ? "(+" : "(") + wellnessScaleVal + ") " +
                                               passive.NextIntValue + "x" + passive.Data.BonusEnumValue.WellnessType);
                break;

            case NumberScaleType.SCALE_BY_WELLNESS_DEFICIT_FLAT:
                var max = GetBonusWellnessMaxTotal(passive.Data.BonusEnumValue.WellnessType);
                var current = wellnessUtils.GetCurrentValue(passive.Data.BonusEnumValue.WellnessType);
                var wellnessDeficitValue = (passive.NextIntValue * (max - current));
                bonus += wellnessDeficitValue;
                text = new FixedString128Bytes((wellnessDeficitValue >= 0 ? "(+" : "(") + wellnessDeficitValue + ") " +
                                               passive.NextIntValue + "x" + passive.Data.BonusEnumValue.WellnessType +
                                               " DEFICIT (" + (max - current) + ")");
                break;

            case NumberScaleType.SCALE_BY_SKILL_POWER:
                var skillPower = GetSkillPower(passive.Data.BonusEnumValue.SkillType, out _, out _, out _);
                bonus += (passive.NextIntValue * skillPower);
                text = new FixedString128Bytes((skillPower >= 0 ? "(+" : "(") + skillPower + ") " +
                                               passive.NextIntValue + "x" + passive.Data.BonusEnumValue.SkillType +
                                               " SKILL POWER");
                break;
        }

        return bonus;
    }

    private int GetPercentBonus(PassiveEffectComponent passive, int percentage){
        var wellnessUtils = new WellnessUtils(){
            Wellness = CharacterWellness
        };
        switch (passive.Data.BonusNumericType){
            case NumberScaleType.PERCENTAGE:
                percentage += passive.NextIntValue;
                break;

            case NumberScaleType.SCALE_BY_ATTRIBUTE_FLAT_PERCENTAGE:
                var attributeType = passive.Data.BonusEnumValue.AttributeType;
                var attributeTotalLvl = GetNaturalAndBonusAttributeTotal(attributeType);
                percentage += (attributeTotalLvl * passive.NextIntValue);
                break;

            case NumberScaleType.SCALE_BY_TRAIT_INTENSITY_PERCENTAGE:
                var intensity =
                    GetTotalTraitIntensity(passive.Data.BonusEnumValue.GetTraitType());
                percentage += (passive.NextIntValue * intensity);
                break;

            case NumberScaleType.SCALE_BY_CURRENT_WELLNESS_PERCENTAGE:
                percentage += (passive.NextIntValue *
                               wellnessUtils.GetCurrentValue(passive.Data.BonusEnumValue.WellnessType));
                break;
        }

        return percentage;
    }
}