using Unity.Entities;

public struct SourceTestKnowledgeComponent : IComponentData {
    public SkillType SkillType;
    public int DifficultyLevel;
    public int TotalQuestionsCount;
    public int TotalPreparationPoints;
    public Entity ClassroomEntity;
    public int ClassPeriod;
}