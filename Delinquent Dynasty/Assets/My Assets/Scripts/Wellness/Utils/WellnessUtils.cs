using Unity.Entities;

public struct WellnessUtils {
    public DynamicBuffer<WellnessElement> Wellness;

    public WellnessElement Focus => GetWellness(WellnessType.FOCUS, out _);
    public WellnessElement Energy => GetWellness(WellnessType.ENERGY, out _);

    public int Affect(WellnessType wellnessType, int wellnessValue, int bonusMax){
        var wellness = GetWellness(wellnessType, out int index);
        wellness.AddValue(wellnessValue, bonusMax);
        Wellness[index] = wellness;
        return wellness.CurrentValue;
    }

    public WellnessElement GetWellness(WellnessType type, out int index){
        for (var i = 0; i < Wellness.Length; i++){
            var wellnessElement = Wellness[i];
            if (wellnessElement.WellnessType == type){
                index = i;
                return wellnessElement;
            }
        }

        index = -1;
        return default;
    }

    public int GetCurrentValue(WellnessType wellnessType){
        return GetWellness(wellnessType, out _).CurrentValue;
    }

    public int GetBaseMax(WellnessType wellnessType){
        return GetWellness(wellnessType, out _).Max;
    }

    public bool MeetsCost(ActionBaseAssetData actData, PassiveEffectsUtils utils){
        var focusCost = utils.GetNaturalAndBonusCost(actData.FocusCost, true, actData.SkillUsed);
        var energyCost = utils.GetNaturalAndBonusCost(actData.EnergyCost, false, actData.SkillUsed);
        return Focus.CurrentValue >= focusCost && Energy.CurrentValue >= energyCost;
    }
}