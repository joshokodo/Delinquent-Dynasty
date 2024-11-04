using Unity.Entities;

[InternalBufferCapacity(25)]
public struct StudentClassKnowledgeElement : IBufferElementData {
    public Entity ClassroomEntity;
    public Entity StudentEntity;
    public int Period;
    public ClassroomSubject Subject;
}