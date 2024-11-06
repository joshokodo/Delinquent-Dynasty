using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(10)] //TODO: change this as needed. look into maybe making this a fixed list if that will be more performant. did that before but didnt work with multiple effects happening in one frame as they override each other.
public struct WellnessElement : IBufferElementData {
    public WellnessType WellnessType;
    public int CurrentValue;
    public int BaseMax => 200;

    public void AddValue(int val, int bonusMax){
        var max = BaseMax + bonusMax;
        CurrentValue = Mathf.Clamp(CurrentValue + val, 0, max);
    }
}