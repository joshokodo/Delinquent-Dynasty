using Unity.Collections;
using Unity.Entities;

public struct AppComponent : IComponentData {
    public FixedString32Bytes AppName;
    public int Rating;
    public FixedList128Bytes<AppType> AppTypes;
}