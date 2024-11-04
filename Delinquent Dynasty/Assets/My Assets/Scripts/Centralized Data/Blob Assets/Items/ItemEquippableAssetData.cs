using System;
using Unity.Collections;

public struct ItemEquippableAssetData {
    public Guid Id;
    public int EffectsCount;
    public DynamicItemType ItemType;
    public double EquipTime;
    public double UnEquipTime;
    public EquipmentType EquipmentType;
    public bool IsFemale;
    public int MeshIndex;
    public int PrimaryMaterialIndex;
    public int SecondaryMaterialIndex;
    public int TertiaryMaterialIndex;
}