using System;
using Unity.Collections;
using Unity.Entities;

public struct GenericTargetAssetData {
    public Guid Id;
    public Guid ParentId;
    public TargetData Data;
    public bool AnyCondition;
    public FixedList4096Bytes<ConditionData> Conditions;

    public TargetElement ToTargetBufferElement(){
        return new TargetElement(){
            TargetId = Id,
            ParentId = ParentId,
            Data = Data,
        };
    }

    public DynamicBuffer<ConditionElement> GetConditionsElements(){
        var conds = new DynamicBuffer<ConditionElement>();
        foreach (var condData in Conditions){
            conds.Add(new ConditionElement(){
                ConditionData = condData,
                ParentId = Id
            });
        }

        return conds;
    }
}