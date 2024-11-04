using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(0)]
public struct ActionElement : IBufferElementData {
    public DynamicActionType ActionType;

    public Guid SourceId;
    public Guid ActionId;
    public Guid ParentActionId;

    public bool IsEnabled;
    public bool IsActive;

    public bool AnyCondition;
    public bool MeetsConditions;
    public bool MeetsPrerequisites;
    public bool MeetsCost;

    public bool DeleteOnCompletion;
    public bool DeleteOnDeactivation;
    public bool TriggerDelete;

    public bool HasCompletedAllIterations;

    public bool WaitToCheckChildActions;


    public int TotalIterations;
    public int CurrentIteration;
}