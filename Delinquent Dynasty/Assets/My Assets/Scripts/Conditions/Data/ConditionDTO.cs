using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ConditionDTO {
    public DynamicConditionTypeDTO dynamicConditionType;
    public bool useGodVision;
    [FormerlySerializedAs("boolValue")] public bool expectedConditionValue;

    public TargetType primaryTarget;
    public TargetType secondaryTarget;

    [FormerlySerializedAs("numberValue")] public double primaryNumberValue;
    public double secondaryNumberValue;
    public NumericComparisonSign numericComparisonSign;

    [FormerlySerializedAs("aliasValue")] public string primaryStringValue;

    // types
    [FormerlySerializedAs("genericEnumDtoType")]
    public DynamicGameEnumDTO primaryEnumValue;

    [FormerlySerializedAs("secondaryEnumDtoType")]
    public DynamicGameEnumDTO secondaryEnumValue;


    public ConditionData ToConditionData(){
        var data = new ConditionData(){
            DynamicConditionType = dynamicConditionType.ToData(),
            ExpectedConditionValue = expectedConditionValue,
            UseGodVision = useGodVision,
            NumericComparisonSign = numericComparisonSign,
            PrimaryNumberValue = primaryNumberValue,
            SecondaryNumberValue = secondaryNumberValue,
            PrimaryEnumValue = primaryEnumValue.ToData(),
            SecondaryEnumValue = secondaryEnumValue.ToData(),
            PrimaryTarget = primaryTarget,
            SecondaryTarget = secondaryTarget,
            PrimaryStringValue = !string.IsNullOrEmpty(primaryStringValue) ? new FixedString128Bytes(primaryStringValue) : default
        };
        return data;
    }
}