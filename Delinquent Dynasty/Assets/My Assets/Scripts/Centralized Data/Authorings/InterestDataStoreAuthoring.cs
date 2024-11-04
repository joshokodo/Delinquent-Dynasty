using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class InterestDataStoreAuthoring : MonoBehaviour { }

public class InterestDataStoreBaker : Baker<InterestDataStoreAuthoring> {
    public override void Bake(InterestDataStoreAuthoring authoring){
        var store = new InterestDataStore();
        store.InterestBlobAssets = InitializeInterestBlobAssets();
        AddComponent(store);
    }

    private BlobAssetReference<InterestBlobAssets> InitializeInterestBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<InterestEffectsDataSO>("Scriptable Objects/Interest");

        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var interestBlobBuilder = ref blobBuilder.ConstructRoot<InterestBlobAssets>();

        var baseList = new List<InterestBaseAssetData>();
        var effectsList = new List<PassiveEffectAssetData>();

        for (int i = 0; i < data.Count; i++){
            var baseId = Guid.NewGuid();
            data[i].passiveEffects.ForEach(e => {
                var effs = new FixedList4096Bytes<PassiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                foreach (var f in e.effects){
                    effs.Add(f.ToData());
                }

                effectsList.Add(new PassiveEffectAssetData(){
                    Id = Guid.NewGuid(),
                    ParentId = baseId,
                    Effects = effs,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            baseList.Add(new InterestBaseAssetData(){
                Id = baseId,
                SubjectType = data[i].subjectType,
                EnumValue = data[i].primaryEnumValue.ToData()
            });
        }

        var baseArr = blobBuilder.Allocate(ref interestBlobBuilder.InterestBaseAssets, baseList.Count);
        var effectsArr = blobBuilder.Allocate(ref interestBlobBuilder.PassiveEffectsAssets, effectsList.Count);

        for (var i = 0; i < baseList.Count; i++){
            baseArr[i] = baseList[i];
        }

        for (var i = 0; i < effectsList.Count; i++){
            effectsArr[i] = effectsList[i];
        }

        return blobBuilder.CreateBlobAssetReference<InterestBlobAssets>(Allocator.Persistent);
    }
}