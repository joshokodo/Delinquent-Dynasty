using System;

[Serializable]
public struct NpcGoalDataDTO {
    public NPCGoalType GoalType;
    public DynamicGameEnumDTO EnumType;

    public NpcGoalAssetData ToData(){
        return new NpcGoalAssetData(){
            GoalType = GoalType,
            EnumType = EnumType.ToData()
        };
    }
}