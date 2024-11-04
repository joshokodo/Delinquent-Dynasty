using System;
using Unity.Collections;
using Unity.Entities;

public struct NpcBaseResponseAssetData {
    public Guid ResponseId;
    public FixedList4096Bytes<DynamicNPCScenarioType> ScenarioTypes;
    public FixedList4096Bytes<NpcGoalAssetData> GoalTypes;
    public NPCPersonalityType PersonalityType;
    public Severity Severity;
    public int Priority;

    public bool IsMoreUrgent(Severity severity, int priority){
        return (Severity > severity) || (Severity == severity && Priority < priority);
    }


    public bool ScenariosMatch(DynamicBuffer<NPCScenarioElement> scenarios){
        foreach (var scenarioType in ScenarioTypes){
            var foundMatch = false;
            foreach (var scenarioElement in scenarios){
                if (scenarioElement.ScenarioType.Matches(scenarioType)){
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch){
                return false;
            }
        }

        return true;
    }

    public bool GoalsMatch(FixedList4096Bytes<NpcGoalData> goals){
        foreach (var goalAsset in GoalTypes){
            var foundMatch = false;
            foreach (var goal in goals){
                if (goal.GoalType == goalAsset.GoalType && goal.EnumType.Matches(goalAsset.EnumType)){
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch){
                return false;
            }
        }

        return true;
    }

    // public bool PersonalityMatch(FixedList128Bytes<NPCGoalType> goals){
    //     foreach (var goalType in GoalTypes){
    //         var foundMatch = false;
    //         foreach (var goal in goals){
    //             if (goal == goalType){
    //                 foundMatch = true;
    //                 break;
    //             }
    //         }
    //         if (!foundMatch){
    //             return false;
    //         }
    //     }
    //
    //     return true;
    // }
}