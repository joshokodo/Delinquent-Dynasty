using System;
using Unity.Collections;
using Unity.Entities;

public struct NpcGoalResponseAssetData {
    public Guid ResponseId;
    public Guid GoalId;
    public NPCGoalType GoalType;
    public FixedList128Bytes<DynamicNPCScenarioType> Scenarios;

    public bool MatchesScenarios(DynamicBuffer<NPCScenarioElement> npcAspectScenarios){
        foreach (var scenarioType in Scenarios){
            var foundMatch = false;
            foreach (var scenarioElement in npcAspectScenarios){
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
}