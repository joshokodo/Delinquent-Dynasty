using Unity.Entities;

public struct RequestEntityElement : IBufferElementData{
    public Entity RequestEntity;
    public Entity TargetEntity;
}