using Unity.Entities;

public struct ExpirationComponent : IComponentData {
    public double StartTime;
    public double ExpirationTime;
}