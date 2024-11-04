using System;
using Unity.Collections;

public struct NpcResponseConditionAssetData {
    public Guid ParentId;
    public FixedList4096Bytes<ActionConditionAssetData> Conditions;
}