using Unity.Entities;

[InternalBufferCapacity(0)]
public struct TextMessageElement : IBufferElementData {
    public Entity OriginPhone;
    public DynamicActionType ActionType;
    public int SkillLevel;
    public bool IsSuccessful;
    public bool HasRead;
}