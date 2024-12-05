using Unity.Entities;

[InternalBufferCapacity(0)]
public struct VideoFrameElement : IBufferElementData {
    public ImageData Data;
}