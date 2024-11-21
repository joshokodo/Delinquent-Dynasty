using Unity.Entities;
using Unity.Mathematics;

public struct MediaMomentData {
    public Entity Character;
    public float3 Position;
    public DynamicActionType ActionPerformed;
    public Entity ActionTarget;
    public TargetType ActionTargetType;
}