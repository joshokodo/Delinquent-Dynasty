using System;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct ActiveActionTargetElement : IBufferElementData {
    public Guid ActionId;
    public Guid TargetId;
    public TargetData Data;
}