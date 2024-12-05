using Unity.Entities;

[InternalBufferCapacity(0)] // todo might be safe with max student class count if there is no way to dupe or add external student records
public struct ClassroomWorkRecordElement : IBufferElementData {
    public Entity Student;
    public int Percent;
}