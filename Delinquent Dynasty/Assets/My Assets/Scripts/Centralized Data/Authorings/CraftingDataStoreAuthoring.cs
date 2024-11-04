using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CraftingDataStoreAuthoring : MonoBehaviour { }

public class CraftingDataStoreBaker : Baker<CraftingDataStoreAuthoring> {
    public override void Bake(CraftingDataStoreAuthoring authoring){
        var store = new CraftingDataStore();
        store.CraftingBlobAssets = InitializeCraftingBlobAssets();
        AddComponent(store);
    }

    private BlobAssetReference<CraftingBlobAssets> InitializeCraftingBlobAssets(){
        var recipeData = CommonUtils.GetScriptableObjectData<CraftingRecipeDataSO>("Scriptable Objects/Crafting");
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var craftingBlobBuilder = ref blobBuilder.ConstructRoot<CraftingBlobAssets>();

        var recipeList = new List<CraftingRecipeAssetData>();
        var recipeActiveEffectList = new List<CraftingRecipeActiveEffectAssetData>();
        var recipePassiveEffectList = new List<CraftingRecipePassiveEffectAssetData>();

        foreach (var recipe in recipeData){
            var recipeAssetId = Guid.NewGuid();
            var ingredients = new FixedList512Bytes<CraftingCompoundData>();
            Debug.Log("ingredients 512 fixed list cap " + ingredients.Capacity);
            recipe.ingredients.ForEach(i => ingredients.Add(i.ToData()));

            var prepTools = new FixedList512Bytes<CraftingCompoundData>();
            Debug.Log("prep tools 512 fixed list cap " + prepTools.Capacity);
            recipe.prepTools.ForEach(t => prepTools.Add(t.ToData()));

            var buildTools = new FixedList512Bytes<CraftingCompoundData>();
            Debug.Log("build tools 512 fixed list cap " + buildTools.Capacity);
            recipe.buildTools.ForEach(t => buildTools.Add(t.ToData()));

            recipeList.Add(new CraftingRecipeAssetData(){
                Id = recipeAssetId,
                Ingredients = ingredients,
                PrepTools = prepTools,
                BuildTools = buildTools,
                RequiredLocation = recipe.requiredLocationType,
                ExpireTime = new TimeSpan(recipe.expireTimeHours, recipe.expireTimeMins, recipe.expireTimeSeconds)
                    .TotalSeconds,
                PrepTime = new TimeSpan(recipe.prepTimeHours, recipe.prepTimeMins, recipe.prepTimeSeconds).TotalSeconds,
                BuildIterationTime = new TimeSpan(recipe.buildIterationTimeHours, recipe.buildIterationTimeMins,
                    recipe.buildIterationTimeSeconds).TotalSeconds,
                RequiredSkillLevel = recipe.requiredSkillLevel,
                RequiredSkillType = recipe.requiredSkillType,
                SuccessfulProduct = recipe.successfulProduct.ToData(),
                FailedProduct = recipe.failedProduct.ToData(),
                ExpiredProduct = recipe.expiredProduct.ToData(),
            });


            foreach (var recipeSuccessEffect in recipe.activeEffects){
                var conds = new FixedList4096Bytes<ConditionData>();
                var effects = new FixedList512Bytes<ActiveEffectData>();

                foreach (var e in recipeSuccessEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in recipeSuccessEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                recipeActiveEffectList.Add(new CraftingRecipeActiveEffectAssetData(){
                    QualityType = recipeSuccessEffect.qualityType,
                    EffectAssetData = new ActiveEffectAssetData(){
                        ParentId = recipeAssetId,
                        Effects = effects,
                        Conditions = conds,
                        AnyCondition = recipeSuccessEffect.anyCondition,
                    }
                });
            }


            foreach (var nextPassiveActiveEffect in recipe.passiveEffects){
                var effects = new FixedList4096Bytes<PassiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var e in nextPassiveActiveEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in nextPassiveActiveEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                recipePassiveEffectList.Add(new CraftingRecipePassiveEffectAssetData(){
                    PassiveEffectAssetData = new PassiveEffectAssetData(){
                        ParentId = recipeAssetId,
                        Id = Guid.NewGuid(),
                        Effects = effects,
                        Conditions = conds,
                        AnyCondition = nextPassiveActiveEffect.anyCondition,
                    },
                    QualityType = nextPassiveActiveEffect.qualityType
                });
            }
        }

        var recipeArr = blobBuilder.Allocate(ref craftingBlobBuilder.RecipeAssets, recipeList.Count);
        var activesArr =
            blobBuilder.Allocate(ref craftingBlobBuilder.RecipeEffectsAssets, recipeActiveEffectList.Count);
        var passivesArr = blobBuilder.Allocate(ref craftingBlobBuilder.RecipePassiveEffectsAssets,
            recipePassiveEffectList.Count);

        for (var i = 0; i < recipeList.Count; i++){
            recipeArr[i] = recipeList[i];
        }

        for (var i = 0; i < recipeActiveEffectList.Count; i++){
            activesArr[i] = recipeActiveEffectList[i];
        }

        for (var i = 0; i < recipePassiveEffectList.Count; i++){
            passivesArr[i] = recipePassiveEffectList[i];
        }

        return blobBuilder.CreateBlobAssetReference<CraftingBlobAssets>(Allocator.Persistent);
    }
}