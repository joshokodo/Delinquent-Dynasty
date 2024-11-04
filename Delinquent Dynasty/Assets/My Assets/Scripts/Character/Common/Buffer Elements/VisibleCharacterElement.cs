using Unity.Entities;

[InternalBufferCapacity(0)]
public struct VisibleCharacterElement : IBufferElementData {
    public Entity VisibleCharacter;
    public bool IsHidden;
    public float Distance;
}