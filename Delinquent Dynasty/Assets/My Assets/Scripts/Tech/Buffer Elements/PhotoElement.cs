using Unity.Collections;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct PhotoElement : IBufferElementData {
    public ImageData Data;
}