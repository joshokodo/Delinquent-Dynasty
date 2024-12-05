using Unity.Entities;

public struct AppUserSpawnElement : IBufferElementData {
    public Entity TargetEntity;
    public Entity TargetApp;
}