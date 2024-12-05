using Unity.Collections;
using Unity.Entities;

public struct ClassroomAssignmentComponent : IComponentData {
    public FixedString64Bytes AssignmentTitle;
    public SkillType SkillType;
    public bool IsOpen;
    public GameTimeStamp OpenTime;
    public int DifficultyLevel;
    public int NumberOfProblems;
    public int ProblemTimeInSecs;
}