using Unity.Collections;

public struct ImageData {
    public FixedList4096Bytes<MediaMomentData> CharactersInView; // todo. find out limit, if too small, find another way
    public EventTimestamp Timestamp;
}