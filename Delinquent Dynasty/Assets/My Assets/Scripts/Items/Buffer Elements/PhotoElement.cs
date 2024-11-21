using Unity.Collections;
using Unity.Entities;

public struct PhotoElement : IBufferElementData {
    public FixedList4096Bytes<MediaMomentData> CharactersInView; // todo. find out limit, if too small, find another way
    public EventTimestamp Timestamp;
}