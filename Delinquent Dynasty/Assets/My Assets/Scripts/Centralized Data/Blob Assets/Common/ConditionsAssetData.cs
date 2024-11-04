using System;
using Unity.Collections;

public struct ConditionsAssetData {
    public Guid ParentId;
    public FixedList4096Bytes<ConditionData> Conditions;
}