using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SkillDataStoreAuthoring : MonoBehaviour { }

public class SkillDataStoreBaker : Baker<SkillDataStoreAuthoring> {
    public override void Bake(SkillDataStoreAuthoring authoring){
        var store = new SkillDataStore();
        store.SkillBlobAssets = InitializeSkillsBlobAssets();
        store.SkillsCount = Enum.GetNames(typeof(SkillType)).Length;
        AddComponent(store);
    }

    private BlobAssetReference<SkillBlobAssets> InitializeSkillsBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<SkillDataSO>("Scriptable Objects/Skills");
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var skillBlobBuilder = ref blobBuilder.ConstructRoot<SkillBlobAssets>();

        var baseList = new List<SkillBaseAssetData>();


        for (int i = 0; i < data.Count; i++){
            baseList.Add(new SkillBaseAssetData(){
                SkillType = data[i].skillType,
                IsLearned = data[i].isLearned,
                PrimaryAttribute = data[i].primaryAttribute,
                SecondaryAttribute = data[i].secondaryAttribute,
                IsDynamic = data[i].isDynamic
            });
        }

        var baseArr = blobBuilder.Allocate(ref skillBlobBuilder.SkillAssets, baseList.Count);

        for (var i = 0; i < baseList.Count; i++){
            baseArr[i] = baseList[i];
        }

        return blobBuilder.CreateBlobAssetReference<SkillBlobAssets>(Allocator.Persistent);
    }
}