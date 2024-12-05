using System;
using Unity.Collections;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct VideoElement : IBufferElementData {
    public Guid GroupId;
    public Entity ImagesEntity;
}