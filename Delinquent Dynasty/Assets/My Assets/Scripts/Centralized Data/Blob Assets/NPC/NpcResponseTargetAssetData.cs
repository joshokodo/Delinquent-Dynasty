using System;
using Unity.Collections;

public struct NpcResponseTargetAssetData {
    public Guid ParentId;
    public FixedList4096Bytes<TargetAssetData> Targets;
}