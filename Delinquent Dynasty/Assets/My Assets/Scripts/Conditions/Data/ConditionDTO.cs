using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ConditionDTO {
    public ConditionType conditionType;
    public bool useGodVision;
    [FormerlySerializedAs("boolValue")] public bool expectedConditionValue;

    public TargetType primaryTarget;
    public TargetType secondaryTarget;

    [FormerlySerializedAs("numberValue")] public double primaryNumberValue;
    public double secondaryNumberValue;
    public NumericComparisonSign numericComparisonSign;

    public string aliasValue;
    public YesNoChoiceType checkHasAlias;

    // types
    [FormerlySerializedAs("genericEnumDtoType")]
    public DynamicGameEnumDTO primaryEnumValue;

    [FormerlySerializedAs("secondaryEnumDtoType")]
    public DynamicGameEnumDTO secondaryEnumValue;


    public ConditionData ToConditionData(){
        var data = new ConditionData(){
            ConditionType = conditionType,
            ExpectedConditionValue = expectedConditionValue,
            UseGodVision = useGodVision,
            NumericComparisonSign = numericComparisonSign,
            PrimaryNumberValue = primaryNumberValue,
            SecondaryNumberValue = secondaryNumberValue,
            PrimaryEnumValue = primaryEnumValue.ToData(),
            SecondaryEnumValue = secondaryEnumValue.ToData(),
            CheckHasAlias = checkHasAlias,
            PrimaryTarget = primaryTarget,
            SecondaryTarget = secondaryTarget,
            AliasValue = !string.IsNullOrEmpty(aliasValue) ? new FixedString128Bytes(aliasValue) : default
        };
        return data;
    }
}