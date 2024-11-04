using System.Collections.Generic;
using UIWidgets;
using Unity.Collections;
using Unity.Entities;

//todo: crafting and recipes is stored in skills. delete this and asset unused asset data if able
public struct CraftingBlobAssets {
    public BlobArray<CraftingRecipeAssetData> RecipeAssets;
    public BlobArray<CraftingRecipeActiveEffectAssetData> RecipeEffectsAssets;
    public BlobArray<CraftingRecipePassiveEffectAssetData> RecipePassiveEffectsAssets;

    public CraftingRecipeAssetData GetRecipeData(DynamicGameEnum recipe){
        return GetRecipeData(recipe.GetItemType());
    }

    public CraftingRecipeAssetData GetRecipeData(DynamicItemType recipe){
        for (int i = 0; i < RecipeAssets.Length; i++){
            if (RecipeAssets[i].SuccessfulProduct.Matches(recipe)){
                return RecipeAssets[i];
            }
        }

        return default;
    }

    public ItemBuildInProgressComponent GetWorkInProgressComponent(DynamicItemType itemType, InGameTime inGameTime){
        var data = GetRecipeData(itemType);

        return new ItemBuildInProgressComponent(){
            BuildIterationTime = data.BuildIterationTime,
            ExpireCompleteTime = inGameTime.TotalInGameSeconds + data.ExpireTime,
            SuccessfulProduct = data.SuccessfulProduct,
            FailedProduct = data.FailedProduct,
            ExpiredProduct = data.ExpiredProduct,
        };
    }

    public void SetRecipes(AutocompleteString recipeComboBox, SkillType skillSkillType, int skillLevel,
        Dictionary<DynamicItemType, CraftingRecipeAssetData> recipesMap){
        for (int i = 0; i < RecipeAssets.Length; i++){
            if (RecipeAssets[i].RequiredSkillType == skillSkillType &&
                RecipeAssets[i].RequiredSkillLevel <= skillLevel){
                var val = RecipeAssets[i].SuccessfulProduct;
                if (recipesMap.TryAdd(val, RecipeAssets[i])){
                    recipeComboBox.DataSource.Add(StringUtils.GetItemTypeString(val).Value);
                }
            }
        }
    }

    public FixedString4096Bytes GetCraftedEffects(DynamicItemType itemType, CraftedQualityType qualityType){
        var result = new FixedString4096Bytes();
        var recipeData = GetRecipeData(itemType);
        for (int i = 0; i < RecipeEffectsAssets.Length; i++){
            var effects = RecipeEffectsAssets[i];
            if (effects.EffectAssetData.ParentId == recipeData.Id && effects.QualityType == qualityType){
                result = StringUtils.SetConditionsString(effects.EffectAssetData.Conditions, result);
                result = StringUtils.SetActiveEffectsString(effects.EffectAssetData.Effects, result);
            }
        }

        if (!result.IsEmpty){
            result.Append("\n");
        }

        for (int i = 0; i < RecipePassiveEffectsAssets.Length; i++){
            var effects = RecipePassiveEffectsAssets[i];
            if (effects.PassiveEffectAssetData.ParentId == recipeData.Id && effects.QualityType == qualityType){
                result = StringUtils.SetConditionsString(effects.PassiveEffectAssetData.Conditions, result);
                result = StringUtils.SetPassiveEffectsString(effects.PassiveEffectAssetData.Effects, result);
            }
        }

        return result;
    }

    public void SetCraftedEffects(Entity itemEntity, DynamicItemType itemType, CraftedQualityType qualityType,
        TargetsGroup targetData, DynamicBuffer<ActiveEffectSpawnElement> activeEffectSpawn){
        var recipeData = GetRecipeData(itemType);
        for (int i = 0; i < RecipeEffectsAssets.Length; i++){
            var effects = RecipeEffectsAssets[i];
            if (effects.EffectAssetData.ParentId == recipeData.Id && effects.QualityType == qualityType){
                activeEffectSpawn.Add(new ActiveEffectSpawnElement(){
                    EffectComponentData = new ActiveEffectComponent(){
                        SourceEntity = itemEntity,
                        TargetsData = targetData,
                        Effects = effects.EffectAssetData.Effects,
                        Conditions = effects.EffectAssetData.Conditions,
                        AnyCondition = effects.EffectAssetData.AnyCondition,
                    }
                });
            }
        }
    }

    // public bool IsCraftingMaterial(DynamicItemType itemType, out bool isIngredient, out bool isTool){
    //     isIngredient = false;
    //     isTool = false;
    //     var isMaterial = false;
    //     for (int i = 0; i < RecipeAssets.Length; i++){
    //         var data = RecipeAssets[i];
    //         foreach (var ingredient in data.Ingredients){
    //             if (ingredient.ItemType.Matches(itemType)){
    //                 isIngredient = true;
    //                 isMaterial = true;
    //                 if (isTool){
    //                     break;
    //                 }
    //             }
    //         }
    //         //todo this weird check and then the other is to check if an item is both and ingredient and tool just in case. if not nessasary remove this
    //         if (isTool){
    //             break;
    //         }
    //         
    //         foreach (var tool in data.BuildTools){
    //             if (tool.Matches(itemType)){
    //                 isTool = true;
    //                 isMaterial = true;
    //                 if (isIngredient){
    //                     break;
    //                 }
    //             }
    //         } 
    //         
    //         if (isTool && isIngredient){
    //             break;
    //         }
    //         
    //         foreach (var tool in data.PrepTools){
    //             if (tool.Matches(itemType)){
    //                 isTool = true;
    //                 isMaterial = true;
    //                 if (isIngredient){
    //                     break;
    //                 }
    //             }
    //         } 
    //         
    //     }
    //
    //     return isMaterial;
    // }
}