using Unity.Entities;

[InternalBufferCapacity(0)]
public struct StudentClassKnowledgeElement : IBufferElementData {
    public Entity ClassroomEntity;
    public Entity StudentEntity;
    public int Period;
    public ClassroomSubject Subject;
}