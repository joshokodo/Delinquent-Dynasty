using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ItemStackComponent : IComponentData {
    public int stackCount;
}