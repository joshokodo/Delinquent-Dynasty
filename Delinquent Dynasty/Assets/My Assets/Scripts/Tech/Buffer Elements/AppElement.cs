using Unity.Collections;
using Unity.Entities;

public struct AppElement : IBufferElementData {
    public FixedString32Bytes AppName;
}