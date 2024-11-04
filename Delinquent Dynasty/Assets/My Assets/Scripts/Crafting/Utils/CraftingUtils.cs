public struct CraftingUtils {
    public CraftedQualityType GetFinalQualityType(int qualityPercentage, int defectPercentage){
        if (defectPercentage >= qualityPercentage){
            return CraftedQualityType.VERY_LOW;
        }

        if (defectPercentage <= 0 && qualityPercentage >= 100){
            return CraftedQualityType.MASTERPIECE;
        }

        var qualityVal = qualityPercentage - defectPercentage;
        if (qualityVal <= 20){
            return CraftedQualityType.VERY_LOW;
        }

        if (qualityVal <= 40){
            return CraftedQualityType.LOW;
        }

        if (qualityVal <= 60){
            return CraftedQualityType.MID;
        }

        if (qualityVal <= 80){
            return CraftedQualityType.HIGH;
        }

        if (qualityVal <= 100){
            return CraftedQualityType.VERY_HIGH;
        }


        return default;
    }
}