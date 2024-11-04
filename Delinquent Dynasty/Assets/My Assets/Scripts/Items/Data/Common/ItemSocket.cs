using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ItemSocket {
    public FixedString32Bytes socketName;
    public ItemPropertyType socketCategory;
    public DynamicItemType socketedItemType;
    public Entity socketedItemEntity;
}