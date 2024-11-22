using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TargetDTO {
    public TargetType targetType;
    public int intValue;

    [FormerlySerializedAs("genericEnumDtoType")]
    public DynamicGameEnumDTO dynamicEnumDtoType;

    public string stringValue;


    public TargetAssetData ToAssetData(){
        return new TargetAssetData(){
            Data = ToData()
        };
    }

    public TargetData ToData(){
        return new TargetData(){
            TargetType = targetType,
            EnumValue = dynamicEnumDtoType.ToData(),
            CountValue = intValue,
            TargetString = stringValue
        };
    }
}