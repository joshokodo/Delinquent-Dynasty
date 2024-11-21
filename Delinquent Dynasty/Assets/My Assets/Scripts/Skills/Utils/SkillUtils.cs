using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct SkillUtils : FunctionalStruct {
    public SkillDataStore SkillDataStore;

    public bool AddSkill(DynamicBuffer<SkillElement> skills, SkillType skill, int initialLevel, int initialXp){
        if (skill == SkillType.NONE || skill == SkillType.COMMON){
            return false;
        }

        if (HasSkill(skills, skill)){
            return false;
        }

        skills.Add(new SkillElement(){
            SkillType = skill,
            CurrentLevel = initialLevel,
            CurrentXp = initialXp,
        });
        return true;
    }

    public bool HasSkill(DynamicBuffer<SkillElement> skills, SkillType skillType){
        foreach (var skill in skills){
            if (skill.SkillType == skillType){
                return true;
            }
        }

        return false;
    }

    public bool TryGetSkillElement(SkillType type, DynamicBuffer<SkillElement> skills, out SkillElement skill){
        foreach (var skillElement in skills){
            if (skillElement.SkillType == type){
                skill = skillElement;
                return true;
            }
        }

        skill = default;
        return false;
    }

    public int AddXp(int value, SkillType skillType, DynamicBuffer<SkillElement> skills, out bool leveledUp){
        leveledUp = false;
        for (int i = 0; i < skills.Length; i++){
            var skill = skills[i];
            if (skill.SkillType == skillType){
                if (skill.CurrentLevel >= 5){
                    return 5;
                }
                skill.CurrentXp += value;
                var xpToNextLvl = SkillDataStore.SkillBlobAssets.Value.GetNextSkillLevelXp(skill.CurrentLevel);
                if (skill.CurrentXp >= xpToNextLvl){
                    skill.CurrentLevel++;
                    skill.CurrentXp = 0;
                    leveledUp = true;
                }

                skills[i] = skill;
                return skill.CurrentLevel;
            }
        }

        return 0;
    }


    public float GetXpRatio(SkillElement skill){
        return (float) skill.CurrentXp / SkillDataStore.SkillBlobAssets.Value.GetNextSkillLevelXp(skill.CurrentLevel);
    }

    public AttributeType GetSkillPrimaryAttribute(SkillType skill){
        return SkillDataStore.SkillBlobAssets.Value.GetSkillData(skill).PrimaryAttribute;
    }

    public AttributeType GetSkillSecondaryAttribute(SkillType skill){
        return SkillDataStore.SkillBlobAssets.Value.GetSkillData(skill).SecondaryAttribute;
    }
}