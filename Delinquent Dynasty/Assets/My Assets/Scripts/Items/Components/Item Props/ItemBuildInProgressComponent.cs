using Unity.Entities;
using UnityEngine;

public struct ItemBuildInProgressComponent : IComponentData {
    public int QualityPercentage;
    public int BuildPercentage;
    public int DefectPercentage;
    public double BuildIterationTime;

    public double ExpireCompleteTime;
    public double ExpireCurrentTime;
    public double ExpireStartTime;

    public DynamicItemType SuccessfulProduct;
    public DynamicItemType FailedProduct;
    public DynamicItemType ExpiredProduct;

    public bool AddDefectPercentage(int val){
        DefectPercentage = Mathf.Clamp(val + DefectPercentage, 0, 100);
        return DefectPercentage >= 100;
    }

    public bool AddBuildPercentage(int val){
        BuildPercentage = Mathf.Clamp(val + BuildPercentage, 0, 100);
        return BuildPercentage >= 100;
    }

    public CraftedQualityType AddQualityPercentage(int val){
        QualityPercentage = Mathf.Clamp(val + QualityPercentage, 0, 100);
        if (NumberUtils.IsBetween(QualityPercentage, 0, 20)){
            return CraftedQualityType.VERY_LOW;
        }
        else if (NumberUtils.IsBetween(QualityPercentage, 21, 40)){
            return CraftedQualityType.LOW;
        }
        else if (NumberUtils.IsBetween(QualityPercentage, 41, 60)){
            return CraftedQualityType.MID;
        }
        else if (NumberUtils.IsBetween(QualityPercentage, 61, 80)){
            return CraftedQualityType.HIGH;
        }
        else if (NumberUtils.IsBetween(QualityPercentage, 81, 100)){
            return CraftedQualityType.VERY_HIGH;
        }

        return CraftedQualityType.NONE;
    }
}