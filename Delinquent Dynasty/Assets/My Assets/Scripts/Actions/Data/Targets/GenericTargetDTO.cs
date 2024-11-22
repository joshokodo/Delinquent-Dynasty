using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Serialization;

[Serializable]
public class GenericTargetDTO {
    public TargetType targetType;
    public int intValue;
    public DynamicGameEnumDTO dynamicEnumDtoType;
    public string stringValue;

    public bool anyCondition;
    public List<ConditionDTO> genericTargetConditions;

    public GenericTargetAssetData ToAssetData(){
        var conds = new FixedList4096Bytes<ConditionData>();
        genericTargetConditions.ForEach(c => conds.Add(c.ToConditionData()));
        return new GenericTargetAssetData(){
            Data = ToData(),
            Conditions = conds
        };
    }

    public TargetData ToData(){
        return new TargetData(){
            TargetType = targetType,
            EnumValue = dynamicEnumDtoType.ToData(),
            CountValue = intValue,
            FindGenericTarget = true,
            AnyCondition = anyCondition,
            TargetString = stringValue == null || stringValue.Length == 0 ? new () : new FixedString32Bytes(stringValue)
        };
    }
}