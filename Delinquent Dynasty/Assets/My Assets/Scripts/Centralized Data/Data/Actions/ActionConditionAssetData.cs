using System;
using Unity.Collections;

public struct ActionConditionAssetData {
    public Guid ParentId; // can pair to plan or action elements
    public FixedList4096Bytes<ConditionData> Conditions;
    public bool IsDeleteCondition;
}