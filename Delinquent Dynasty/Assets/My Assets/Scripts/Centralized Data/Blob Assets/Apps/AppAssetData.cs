using Unity.Collections;

public struct AppAssetData {
    public FixedString32Bytes AppName;
    public FixedList128Bytes<AppType> AppTypes;
}