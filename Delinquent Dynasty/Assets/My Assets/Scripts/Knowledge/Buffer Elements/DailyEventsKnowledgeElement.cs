﻿using Unity.Entities;

[InternalBufferCapacity(0)]
public struct DailyEventsKnowledgeElement : IBufferElementData {
    public Entity KnowledgeEntity;
}