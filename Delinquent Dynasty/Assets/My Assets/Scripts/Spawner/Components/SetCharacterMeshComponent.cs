using Unity.Collections;
using Unity.Entities;

public struct SetCharacterMeshComponent : IComponentData {
    public FixedString64Bytes headKey;
    public FixedString64Bytes torsoKey;
    public FixedString64Bytes legsKey;
    public FixedString64Bytes feetKey;
}