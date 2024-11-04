using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CharactersDataStoreAuthoring : MonoBehaviour { }

public class CharactersDataStoreBaker : Baker<InterestDataStoreAuthoring> {
    public override void Bake(InterestDataStoreAuthoring authoring){
        var store = new CharactersDataStore();
        store.RelationshipBlobAssets = InitializeRelationshipBlobAssets();
        AddComponent(store);
    }

    private BlobAssetReference<RelationshipBlobAssets> InitializeRelationshipBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<RelationshipBaseDataSO>(
            "Scriptable Objects/Characters/Relationships");

        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var relBlobBuilder = ref blobBuilder.ConstructRoot<RelationshipBlobAssets>();

        var baseList = new List<MainRelationshipBaseAssetData>();
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

            baseList.Add(new MainRelationshipBaseAssetData(){
                Id = baseId,
                RelationshipMainTitleType = data[i].relationshipMainTitleType,
                RelationshipDescription = new FixedString512Bytes(data[i].relationshipDescription),
            });
        }

        var subData =
            CommonUtils.GetScriptableObjectData<SubRelationshipBaseDataSO>(
                "Scriptable Objects/Characters/Relationships");

        var subBaseList = new List<SubRelationshipBaseAssetData>();
        var subEffectsList = new List<PassiveEffectAssetData>();

        for (int i = 0; i < subData.Count; i++){
            var baseId = Guid.NewGuid();
            subData[i].passiveEffects.ForEach(e => {
                var effs = new FixedList4096Bytes<PassiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                foreach (var f in e.effects){
                    effs.Add(f.ToData());
                }

                subEffectsList.Add(new PassiveEffectAssetData(){
                    Id = Guid.NewGuid(),
                    ParentId = baseId,
                    Effects = effs,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            subBaseList.Add(new SubRelationshipBaseAssetData(){
                Id = baseId,
                RelationshipSubTitleType = subData[i].relationshipSubTitleType,
                RelationshipDescription = new FixedString512Bytes(subData[i].relationshipDescription),
            });
        }

        var baseArr = blobBuilder.Allocate(ref relBlobBuilder.MainRelationshipBaseAssets, baseList.Count);
        var effectsArr = blobBuilder.Allocate(ref relBlobBuilder.RelationshipPassiveAssets, effectsList.Count);
        var subBaseArr = blobBuilder.Allocate(ref relBlobBuilder.SubRelationshipBaseAssets, subBaseList.Count);
        var subEffectsArr = blobBuilder.Allocate(ref relBlobBuilder.RelationshipPassiveAssets, subEffectsList.Count);

        for (var i = 0; i < baseList.Count; i++){
            baseArr[i] = baseList[i];
        }

        for (var i = 0; i < effectsList.Count; i++){
            effectsArr[i] = effectsList[i];
        }

        for (var i = 0; i < subBaseList.Count; i++){
            subBaseArr[i] = subBaseList[i];
        }

        for (var i = 0; i < subEffectsList.Count; i++){
            subEffectsArr[i] = subEffectsList[i];
        }


        return blobBuilder.CreateBlobAssetReference<RelationshipBlobAssets>(Allocator.Persistent);
    }
}