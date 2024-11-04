using System;
using Unity.Collections;

public struct NpcScenarioAssetData {
    public Guid Id;
    public Guid GroupId;
    public DynamicNPCScenarioType ScenarioType;
    public bool AnyCondition;
    public FixedList4096Bytes<ConditionData> Conditions;
}