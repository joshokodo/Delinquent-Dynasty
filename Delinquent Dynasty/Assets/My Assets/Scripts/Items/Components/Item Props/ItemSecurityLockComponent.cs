using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ItemSecurityLockComponent : IComponentData {
    public bool IsLocked;
    public FixedList4096Bytes<Entity> FingerPrintedAllowedCharacters;
}