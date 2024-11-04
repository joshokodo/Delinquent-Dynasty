using System;
using Unity.Collections;
using Unity.Entities;

public struct NpcResponsePlanSpawnElement : IBufferElementData {
    public Entity Entity;
    public Guid ResponseId;
}