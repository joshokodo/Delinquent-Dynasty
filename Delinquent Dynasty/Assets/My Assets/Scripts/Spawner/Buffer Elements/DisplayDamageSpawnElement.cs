using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct DisplayDamageSpawnElement : IBufferElementData {
    public Entity CharacterEntity;
    public DynamicGameEnum DisplayEnum;
    public bool DisplayMiss;
    public bool DisplayBlock;
    public int Value;
    public bool DisplayNumber;
    public Color DisplayColor;
}