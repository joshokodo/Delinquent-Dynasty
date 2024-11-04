using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct InventoryElementUI {
    public FixedString64Bytes inventoryName;
    public bool isLockable;
    public bool isLocked;
    public bool isSelling;
    public Entity lockEntity;
}