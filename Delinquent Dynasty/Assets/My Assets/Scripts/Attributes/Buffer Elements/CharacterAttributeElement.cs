using Unity.Entities;

[InternalBufferCapacity(6)] //TODO: change this as needed. look into maybe making this a fixed list if that will be more performant. did that before but didnt work with multiple effects happening in one frame.
public struct CharacterAttributeElement : IBufferElementData {
    public AttributeType AttributeType;
    public int Level;
    public int CurrentExp;
}