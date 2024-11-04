using Unity.Entities;
using UnityEngine;

public struct IntRangeStatData {
    public int Min;
    public int Max;
    public bool IsSingleValue => Min == Max;
    public bool IsPositive => Min >= 0 && Max > Min;
    public bool IsNegative => Max <= 0 && Min < Max;

    public int GetNextInt(RefRW<RandomComponent> randomComponent){
        return Min == Max ? Max : randomComponent.ValueRW.Random.NextInt(Min, Max + 1);
    }

    public void Add(ActiveEffectData other){
        Min += other.PrimaryNumberRangeValue.Min;
        Max += other.PrimaryNumberRangeValue.Max;
    }

    public void Add(IntRangeStatData other){
        Min += other.Min;
        Max += other.Max;
    }
}