using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AppDataStoreAuthoring : MonoBehaviour { }

public class AppDataStoreBaker : Baker<AppDataStoreAuthoring> {
    public override void Bake(AppDataStoreAuthoring authoring){
        var store = new AppDataStore();
        store.AppBlobAssets = InitializeAppBlobAssets();
        AddComponent(store);
    }

    private BlobAssetReference<AppBlobAssets> InitializeAppBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<AppBaseDataSO>("Scriptable Objects/Apps");

        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var appBlobBuilder = ref blobBuilder.ConstructRoot<AppBlobAssets>();

        var baseList = new List<AppAssetData>();

        for (int i = 0; i < data.Count; i++){
            var types = new FixedList128Bytes<AppType>();
            data[i].AppTypes.ForEach(t => types.Add(t));
            baseList.Add(new AppAssetData(){
                AppName = data[i].appName,
                AppTypes = types
            });
        }

        var baseArr = blobBuilder.Allocate(ref appBlobBuilder.AppAssets, baseList.Count);

        for (var i = 0; i < baseList.Count; i++){
            baseArr[i] = baseList[i];
        }

        return blobBuilder.CreateBlobAssetReference<AppBlobAssets>(Allocator.Persistent);
    }
}