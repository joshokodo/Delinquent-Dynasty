using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct ItemReadAssetData {
    public Guid Id;
    public int SuccessEffectsCount;
    public int FailEffectsCount;
    public DynamicItemType ItemType;
    public int DifficultyLevel;
    public double PerformTime;
}