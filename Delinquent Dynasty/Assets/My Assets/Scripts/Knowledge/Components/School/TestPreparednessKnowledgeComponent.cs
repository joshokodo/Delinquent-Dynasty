using Unity.Entities;

public struct TestPreparednessKnowledgeComponent : IComponentData {
    public Entity SourceTest;
    public int PreparationPoints;
}