using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct RelationshipUtils : FunctionalStruct {
    public int AffectRelationshipStat(Entity originCharacter, Entity targetCharacter,
        RelationshipStatType relationshipStatType, int value, DynamicBuffer<RelationshipElement> targetRelationships,
        DynamicBuffer<PassiveEffectSpawnElement> passivesSpawn, CharactersDataStore characterDataStore,
        RelationshipMainTitleType ifCreateNewRelationshipMain = RelationshipMainTitleType.NONE){
        var finalResult = 0;
        if (HasRelationshipWith(targetCharacter, targetRelationships, out RelationshipElement relationshipElement,
                out int index)){
            finalResult = relationshipElement.AffectStat(relationshipStatType, value);
            targetRelationships[index] = relationshipElement;
        }
        else if (ifCreateNewRelationshipMain != RelationshipMainTitleType.NONE){
            relationshipElement.MainTitle = ifCreateNewRelationshipMain;
            relationshipElement.Character = targetCharacter;
            finalResult = relationshipElement.AffectStat(relationshipStatType, value);
            targetRelationships.Add(relationshipElement);
            characterDataStore.RelationshipBlobAssets.Value
                .SetPassiveEffects(originCharacter, targetCharacter, passivesSpawn, ifCreateNewRelationshipMain);
        }

        return finalResult;
    }

    public void AffectRelationshipTitle(Entity originCharacter, Entity targetCharacter,
        DynamicBuffer<PassiveEffectSpawnElement> passivesSpawn, CharactersDataStore characterDataStore,
        RelationshipMainTitleType relationshipMainTitleType, DynamicBuffer<RelationshipElement> targetRelationships){
        if (HasRelationshipWith(targetCharacter, targetRelationships, out RelationshipElement relationshipElement,
                out int index)){
            characterDataStore.RelationshipBlobAssets.Value
                .SetPassiveEffects(originCharacter, targetCharacter, passivesSpawn, relationshipMainTitleType);
            relationshipElement.MainTitle = relationshipMainTitleType;
            targetRelationships[index] = relationshipElement;
        }
    }

    public bool HasRelationshipWith(Entity targetCharacter, DynamicBuffer<RelationshipElement> targetRelationships,
        out RelationshipElement relationshipElement, out int index){
        relationshipElement = default;
        index = -1;
        for (int i = 0; i < targetRelationships.Length; i++){
            if (targetRelationships[i].Character == targetCharacter){
                relationshipElement = targetRelationships[i];
                index = i;
                return true;
            }
        }

        return false;
    }

    public int RelationshipsCount(DynamicBuffer<RelationshipElement> studentAspectRelationships,
        RelationshipMainTitleType type){
        int count = 0;
        foreach (var studentAspectRelationship in studentAspectRelationships){
            if (studentAspectRelationship.MainTitle == type){
                count++;
            }
        }

        return count;
    }

    public int GetInfluenceSuccessChance(ActionBaseAssetData actionData,
        BufferLookup<RelationshipElement> relationshipsLookup, Entity origin, Entity target,
        PassiveEffectsUtils originPassiveUtils, PassiveEffectsUtils targetPassiveUtils){
        
        var originInfluenceOverTarget = targetPassiveUtils.GetNaturalAndBonusInfluence(relationshipsLookup, origin,
            target, actionData.SkillUsed, actionData.ActionType);
        
        originInfluenceOverTarget = Mathf.Max(originInfluenceOverTarget, 0);
        
        var targetInfluenceOverOrigin = originPassiveUtils.GetNaturalAndBonusInfluence(relationshipsLookup, target,
            origin, actionData.SkillUsed, actionData.ActionType, false);
        
        var denominator = originInfluenceOverTarget + targetInfluenceOverOrigin + actionData.DifficultyLevel;

        var chance = 0;

        if (originInfluenceOverTarget > 0){
            chance = denominator <= 0 ? 100 : Mathf.RoundToInt(100 * ((float) originInfluenceOverTarget / denominator));
        }

        return chance;
    }

    public int GetInfluenceSuccessChanceWithStrings(ActionBaseAssetData actionData,
        BufferLookup<RelationshipElement> relationshipsLookup, Entity origin, Entity target,
        PassiveEffectsUtils originPassiveUtils, PassiveEffectsUtils targetPassiveUtils, out int numeratorTotal, out int denominatorTotal,
        out FixedString4096Bytes numeratorBreakDown, out FixedString4096Bytes denominatorBreakDown){
        
        var totalInfluence = originPassiveUtils.GetNaturalAndBonusInfluenceWithString(relationshipsLookup, origin, target,
            actionData.SkillUsed, actionData.ActionType, out numeratorBreakDown);

        numeratorTotal = totalInfluence;
        
        totalInfluence = Mathf.Max(totalInfluence, 0);
        
        var targetInfluenceOverOrigin = targetPassiveUtils.GetNaturalAndBonusInfluence(relationshipsLookup,
            target, origin, actionData.SkillUsed, actionData.ActionType, false);

        
        var totalResistance = totalInfluence + targetInfluenceOverOrigin + actionData.DifficultyLevel;
        denominatorTotal = totalResistance;

        var chance = 0;

        if (totalInfluence > 0){
            chance = totalResistance <= 0 ? 100 : Mathf.RoundToInt(100 * ((float) totalInfluence / totalResistance));
        }
        
        
        if (numeratorTotal <= 0){
            denominatorBreakDown = numeratorTotal.ToString();
        }
        else{
            denominatorBreakDown = "+" + numeratorTotal;
        }

        denominatorBreakDown.Append(" (Total Influence Over Target)\n");
        
        if (targetInfluenceOverOrigin <= 0){
            denominatorBreakDown.Append(targetInfluenceOverOrigin.ToString());
        }
        else{
            denominatorBreakDown.Append("+" + targetInfluenceOverOrigin);
        }

        denominatorBreakDown.Append(" (Target's Influence Over You)\n");

        if (actionData.DifficultyLevel > 0){
            denominatorBreakDown.Append("+" + actionData.DifficultyLevel + " (Difficulty Level)");
        }

        return chance;
    }

    public int GetRelationshipStat(Entity target, DynamicBuffer<RelationshipElement> buffer, RelationshipStatType relationshipStatType){
        foreach (var relationshipElement in buffer){
            if (relationshipElement.Character == target){
                return relationshipElement.GetStat(relationshipStatType);
            }
        }

        return 0;
    }
}