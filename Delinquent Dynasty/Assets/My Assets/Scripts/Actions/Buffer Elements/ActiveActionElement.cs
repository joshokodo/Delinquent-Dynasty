using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(10)] //todo find out actually max actions possible at once. 
public struct ActiveActionElement : IBufferElementData {
    public Guid ActionId;
    public Guid PerformanceId;
    public DynamicActionType ActionType;
    public bool HasCompletedIteration;
    public bool HasStarted;
    public double FinishTime;
    public double StartTime;
    public double CurrentPerformTime;
    public double LastEffectIterationTime;
}