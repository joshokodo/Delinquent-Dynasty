using System;
using Unity.Collections;
using UnityEngine;

public struct ItemRangedWeaponAssetData {
    public Guid Id;
    public int SuccessEffectsCount;
    public int FailEffectsCount;
    public DynamicItemType ItemType;
    public WeaponType WeaponType;
    public int AmmoLoadLimit;
    public int DifficultyLevel;
    public double ReloadTime;
    public double FireRate;
}