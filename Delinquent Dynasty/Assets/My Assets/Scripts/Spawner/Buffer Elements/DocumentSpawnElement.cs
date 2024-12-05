using Unity.Entities;

public struct DocumentSpawnElement : IBufferElementData{
    public DocumentType DocumentType;
    public Entity DocumentHolder;
    public Entity PrimaryTarget;
    public Entity SecondaryTarget;
}