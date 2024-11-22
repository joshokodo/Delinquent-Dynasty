using System;
using Unity.Collections;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct VideoElement : IBufferElementData {
    public Guid GroupId;
    public FixedList4096Bytes<MediaMomentData> CharactersInView; // todo. find out limit, if too small, find another way
    public EventTimestamp Timestamp;
}