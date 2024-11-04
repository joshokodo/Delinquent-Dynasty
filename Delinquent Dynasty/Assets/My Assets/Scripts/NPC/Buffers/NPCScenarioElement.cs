using System;
using UIWidgets.Examples.DataChange;
using Unity.Entities;

public struct NPCScenarioElement : IBufferElementData {
    public DynamicNPCScenarioType ScenarioType;
    public NpcScenarioAssetData Data;
}