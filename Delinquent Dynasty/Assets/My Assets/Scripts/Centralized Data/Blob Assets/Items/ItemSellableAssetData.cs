using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct ItemSellableAssetData {
    public DynamicItemType ItemType;
    public int BaseSellValue;
    public FixedList4096Bytes<StreetValueData> StreetValue;

    public int GetStreetValue(FixedList128Bytes<StreetValueModifierType> modifiers, out FixedString4096Bytes modsText){
        var flat = 0;
        var percent = 0;
        modsText = new FixedString4096Bytes();

        foreach (var mod in StreetValue){
            foreach (var modifier in modifiers){
                if (mod.ModifierType == modifier){
                    // modsText.Append("\t* ");
                    // modsText.Append(mod.Value < 0 ? "-" : "+");
                    if (mod.IsFlatValue){
                        // modsText.Append("$" + mod.Value);
                        flat += mod.Value;
                    }
                    else{
                        // modsText.Append(mod.Value + "%");
                        percent += mod.Value;
                    }
                    // modsText.Append(" (" + mod.ModifierType + ")\n");
                }
            }
        }

        var streetVal = Mathf.Max(BaseSellValue + flat, 0);
        streetVal += Mathf.RoundToInt(streetVal * (percent * 0.01f));
        return Mathf.Max(streetVal, 0);
    }
}