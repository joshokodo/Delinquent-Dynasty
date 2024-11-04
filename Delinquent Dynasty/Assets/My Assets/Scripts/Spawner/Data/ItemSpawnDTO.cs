using System;
using Unity.Collections;

[Serializable]
public class ItemSpawnDTO {
    public DynamicItemTypeDTO itemType;
    public int count;
    public OnSpawnTriggerType onSpawnTrigger;
    public string alias;
    public bool itemTargetBool;

    public ItemSpawnData ToData(){
        return new ItemSpawnData(){
            ItemType = itemType.ToData(),
            Count = count,
            OnSpawn = onSpawnTrigger,
            Alias = new FixedString128Bytes(alias),
            ItemTargetBool = itemTargetBool
        };
    }
}