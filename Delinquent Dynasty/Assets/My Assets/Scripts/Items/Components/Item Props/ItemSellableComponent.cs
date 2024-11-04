using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ItemSellableComponent : IComponentData {
    public int SellValue;
    public bool ForSell;
    public FixedList128Bytes<StreetValueModifierType> StreetValueModifiers;

    public bool HasStreetValueModifier(StreetValueModifierType type){
        foreach (var streetValueModifierType in StreetValueModifiers){
            if (streetValueModifierType == type){
                return true;
            }
        }

        return false;
    }
}