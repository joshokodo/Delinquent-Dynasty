using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class StorageGO : MonoBehaviour, IPooledObject {
    public string alias;
    public Entity entity;
    public Transform viewPoint;

    [FormerlySerializedAs("userInitItems")]
    public List<ItemSpawnDTO> initItems;


    public void OnActivation(){ }
    public event Action<GameObject> RecallEvent;

    public FixedList4096Bytes<ItemSpawnData> GetInitItemsFixedList(){
        var list = new FixedList4096Bytes<ItemSpawnData>();
        initItems.ForEach(i => list.Add(i.ToData()));
        return list;
    }
}