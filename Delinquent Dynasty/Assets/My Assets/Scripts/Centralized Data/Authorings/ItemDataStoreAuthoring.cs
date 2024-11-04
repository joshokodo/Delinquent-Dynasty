using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ItemDataStoreAuthoring : MonoBehaviour { }

public class ItemDataStoreBaker : Baker<ItemDataStoreAuthoring> {
    public override void Bake(ItemDataStoreAuthoring authoring){
        var store = new ItemDataStore();
        store.ItemBlobAssets = InitializeItemsBlobAssets();
        AddComponent(store);
    }

    private BlobAssetReference<ItemBlobAssets> InitializeItemsBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<ItemDataSO>("Scriptable Objects/Items");
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var itemBlobBuilder = ref blobBuilder.ConstructRoot<ItemBlobAssets>();

        var baseArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemBaseAssets, data.Count);

        for (int i = 0; i < data.Count; i++){
            baseArr[i] = new ItemBaseAssetData(){
                ItemType = data[i].itemType.ToData(),
                ItemDescription = new FixedString64Bytes(data[i].description),
                Weight = data[i].weight,
                Rarity = data[i].rarityType,
                ItemPropertyTypes = data[i].GetPropertyTypes()
            };
        }

        var consumeEffectList = new List<ActiveEffectAssetData>();

        var readSuccessEffectList = new List<ActiveEffectAssetData>();
        var readFailEffectList = new List<ActiveEffectAssetData>();

        var equippableEffectsList = new List<PassiveEffectAssetData>();
        var equippableEffectsOverTimeList = new List<ActiveEffectAssetData>();

        var completableEffectList = new List<ActiveEffectAssetData>();

        var rangeWeaponSuccessEffectList = new List<ActiveEffectAssetData>();
        var rangeWeaponFailEffectList = new List<ActiveEffectAssetData>();

        var ammoEffectList = new List<ActiveEffectAssetData>();

        var meleeWeaponSuccessEffectList = new List<ActiveEffectAssetData>();
        var meleeWeaponFailEffectList = new List<ActiveEffectAssetData>();

        var filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.CONSUME));
        var consumeArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemConsumeAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();

            var prop = (ConsumeItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.CONSUME);
            prop.successEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();
                Debug.Log("Active effect data fixed list 4096 cap is " + effects.Capacity + "  condition cap 4096 " +
                          conds.Capacity + " targ 4096 cap " + (new FixedList4096Bytes<TargetData>().Capacity));

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                consumeEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });


            consumeArr[i] = new ItemConsumeAssetData(){
                Id = id,
                ItemType = filteredData[i].itemType.ToData(),
                PerformTime = new TimeSpan(prop.consumeHrs, prop.consumeMins, prop.consumeSecs).TotalSeconds,
                ItemAfterConsume = prop.itemAfterConsume.ToData()
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.DECAY));
        var decayArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemDecayAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (DecayItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.DECAY);
            decayArr[i] = new ItemDecayAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                decayDays = prop.decayDays,
                decayHrs = prop.decayHrs,
                decayMins = prop.decayMins,
                decaySecs = prop.decaySecs,
                itemAfterDecay = prop.ItemAfterDecay
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.CELL_PHONE));
        var cellArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemCellPhoneAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (CellPhoneItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.CELL_PHONE);
            cellArr[i] = new ItemCellPhoneAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                BatteryLife = prop.batteryLife,
                DecreaseBatteryRate = new TimeSpan(prop.decreaseBatteryRateHrs, prop.decreaseBatteryRateMins,
                    prop.decreaseBatteryRateSecs).TotalSeconds,
                ProcessingPower = prop.processingPower
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.INVENTORY));
        var inventoryArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemInventoryAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (InventoryItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.INVENTORY);
            var allowedToCarry = new FixedList64Bytes<ItemPropertyType>();
            foreach (var cat in prop.allowedToCarry){
                allowedToCarry.Add(cat);
            }

            inventoryArr[i] = new ItemInventoryAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                CarryCapacity = prop.carryCapacity,
                AllowedToCarry = allowedToCarry
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.SECURITY_LOCK));
        var securityLockArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemSecurityLockAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (SecurityLockItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.SECURITY_LOCK);
            securityLockArr[i] = new ItemSecurityLockAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                SecurityLockType = prop.securityLockType
            };
        }

        filteredData = data.FindAll(d =>
            d.properties.Exists(p => p.PropertyType == ItemPropertyType.CRAFTING_INGREDIENT));
        var ingredientsArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemCraftingIngredientAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (IngredientItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.CRAFTING_INGREDIENT);
            var ingredients = new FixedList64Bytes<CraftingIngredientType>();
            prop.IngredientTypes.ForEach(ing => ingredients.Add(ing));
            ingredientsArr[i] = new ItemCraftingIngredientAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                IngredientTypes = ingredients,
                WasteProduced = prop.WasteProduced.ToData()
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.CRAFTING_TOOL));
        var toolArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemCraftingToolAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (ToolItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.CRAFTING_TOOL);
            var tools = new FixedList64Bytes<CraftingToolType>();
            prop.ToolTypes.ForEach(t => tools.Add(t));
            toolArr[i] = new ItemCraftingToolAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                ToolTypes = tools
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.SECURITY_KEY));
        var securityKeyArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemSecurityKeyAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (SecurityKeyItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.SECURITY_KEY);
            securityKeyArr[i] = new ItemSecurityKeyAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.READ));
        var readArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemReadAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();
            var prop = (ReadItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.READ);

            prop.successEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                readSuccessEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            prop.failEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }


                readFailEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });


            readArr[i] = new ItemReadAssetData(){
                Id = id,
                SuccessEffectsCount = prop.successEffects.Count,
                FailEffectsCount = prop.failEffects.Count,
                ItemType = filteredData[i].itemType.ToData(),
                DifficultyLevel = prop.difficultyLevel,
                PerformTime = new TimeSpan(prop.performHours, prop.performMins, prop.performSecs).TotalSeconds
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.LOCK_PICK));
        var lockPickArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemLockPickAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (LockPickItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.LOCK_PICK);
            lockPickArr[i] = new ItemLockPickAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                difficultyLevelReduction = prop.difficultyLevelReduction,
                performTime = new TimeSpan(prop.performHours, prop.performMins, prop.performSecs).TotalSeconds
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.STACKABLE));
        var stackArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemStackAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (StackableItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.STACKABLE);
            stackArr[i] = new ItemStackAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                StackLimit = prop.stackLimit
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.SELLABLE));
        var sellArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemSellableAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (SellableItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.SELLABLE);
            var streetVal = new FixedList4096Bytes<StreetValueData>();
            prop.streetValue.ForEach(sv => streetVal.Add(sv));
            sellArr[i] = new ItemSellableAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                BaseSellValue = prop.sellValue,
                StreetValue = streetVal
            };
        }


        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.EQUIPPABLE));
        var equipArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemEquippableAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();
            var prop = (ItemEquippablePropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.EQUIPPABLE);

            prop.effects.ForEach(e => {
                var effects = new FixedList4096Bytes<PassiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                equippableEffectsList.Add(new PassiveEffectAssetData(){
                    ParentId = id,
                    Id = Guid.NewGuid(),
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            foreach (var nextEffect in prop.effectsOverTime){
                var conds = new FixedList4096Bytes<ConditionData>();
                var effects = new FixedList512Bytes<ActiveEffectData>();

                foreach (var e in nextEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in nextEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                equippableEffectsOverTimeList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = nextEffect.anyCondition,
                });
            }

            equipArr[i] = new ItemEquippableAssetData(){
                Id = id,
                EffectsCount = prop.effects.Count,
                ItemType = filteredData[i].itemType.ToData(),
                EquipmentType = prop.EquipmentType,
                IsFemale = prop.isFemale,
                MeshIndex = prop.meshIndex,
                PrimaryMaterialIndex = prop.primaryMaterialIndex,
                SecondaryMaterialIndex = prop.secondaryMaterialIndex,
                TertiaryMaterialIndex = prop.tertiaryMaterialIndex,
                EquipTime = new TimeSpan(prop.equipHours, prop.equipMins, prop.equipSecs).TotalSeconds,
                UnEquipTime = new TimeSpan(prop.unequipHours, prop.unequipMins, prop.unequipSecs).TotalSeconds,
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.COMPLETABLE));
        var completeArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemCompletableAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();
            var prop = (ItemCompletablePropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.COMPLETABLE);

            prop.CompletionEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                completableEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            completeArr[i] = new ItemCompletableAssetData(){
                Id = id,
                EffectsCount = prop.CompletionEffects.Count,
                ItemType = filteredData[i].itemType.ToData(),
                CompleteValue = prop.CompleteValue
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.RANGED_WEAPON));
        var rangedWeaponArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemRangedWeaponsAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();
            var prop = (RangedWeaponItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.RANGED_WEAPON);

            prop.SuccessEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();
                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                rangeWeaponSuccessEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            prop.FailEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                rangeWeaponFailEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            rangedWeaponArr[i] = new ItemRangedWeaponAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                Id = id,
                SuccessEffectsCount = prop.SuccessEffects.Count,
                FailEffectsCount = prop.FailEffects.Count,
                DifficultyLevel = prop.DifficultyLevel,
                AmmoLoadLimit = prop.AmmoLoadLimit,
                FireRate = new TimeSpan(prop.FireRateHours, prop.FireRateMins, prop.FireRateSecs).TotalSeconds,
                ReloadTime = new TimeSpan(prop.ReloadHours, prop.ReloadMins, prop.ReloadSecs).TotalSeconds,
                WeaponType = prop.WeaponType
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.RANGED_AMMO));
        var rangedAmmoArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemRangedAmmoAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();
            var prop = (RangedAmmoItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.RANGED_AMMO);

            prop.Effects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();
                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                ammoEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            rangedAmmoArr[i] = new ItemRangedAmmoAssetData(){
                Id = id,
                ItemType = filteredData[i].itemType.ToData(),
                WeaponType = prop.RangedWeaponType
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.MELEE_WEAPON));
        var meleeArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemMeleeWeaponsAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var id = Guid.NewGuid();

            var prop = (MeleeWeaponItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.MELEE_WEAPON);
            prop.SuccessEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();
                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                meleeWeaponSuccessEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            prop.FailEffects.ForEach(e => {
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var ae in e.effects){
                    effects.Add(ae.ToData());
                }

                foreach (var c in e.conditions){
                    conds.Add(c.ToConditionData());
                }

                meleeWeaponFailEffectList.Add(new ActiveEffectAssetData(){
                    ParentId = id,
                    Effects = effects,
                    Conditions = conds,
                    AnyCondition = e.anyCondition,
                });
            });

            meleeArr[i] = new ItemMeleeWeaponAssetData(){
                Id = id,
                SuccessEffectsCount = prop.SuccessEffects.Count,
                FailEffectsCount = prop.FailEffects.Count,
                ItemType = filteredData[i].itemType.ToData(),
                WeaponType = prop.weaponType,
                DifficultyLevel = prop.difficultyLevel,
                AttackTime = new TimeSpan(prop.AttackTimeHours, prop.AttackTimeMins, prop.AttackTimeSecs).TotalSeconds,
                BaseAwareness = prop.baseAwareness,
                BaseDetection = prop.baseDetection
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.DURABILITY));
        var durArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemDurabilityAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (DurabilityItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.DURABILITY);

            durArr[i] = new ItemDurabilityAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                MaxDurability = prop.MaxDurability
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.USAGE));
        var usageArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemUsageAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (UsageItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.USAGE);

            usageArr[i] = new ItemUsageAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                UsageCount = prop.usageCount,
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.NOTEBOOK));
        var notebookArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemNotebookAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            var prop = (NotebookItemPropertyDataSO) filteredData[i].properties
                .Find(p => p.PropertyType == ItemPropertyType.NOTEBOOK);

            notebookArr[i] = new ItemNotebookAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
                NumberOfPages = prop.pageCount
            };
        }

        filteredData = data.FindAll(d => d.properties.Exists(p => p.PropertyType == ItemPropertyType.WRITING_UTENSIL));
        var writingArr = blobBuilder.Allocate(ref itemBlobBuilder.ItemWritingUtensilAssets, filteredData.Count);
        for (int i = 0; i < filteredData.Count; i++){
            // var prop = (WritingUtensilItemPropertyDataSO)filteredData[i].properties.Find(p => p.PropertyType == ItemPropertyType.WRITING_UTENSIL);

            writingArr[i] = new ItemWritingUtensilAssetData(){
                ItemType = filteredData[i].itemType.ToData(),
            };
        }


        var consumeEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.ConsumeEffectsAssets, consumeEffectList.Count);

        var readSuccessEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.ReadSuccessActiveEffectsAssets,
            readSuccessEffectList.Count);
        var readFailEffectArr =
            blobBuilder.Allocate(ref itemBlobBuilder.ReadFailActiveEffectsAssets, readFailEffectList.Count);

        var equippableEffectsArr = blobBuilder.Allocate(ref itemBlobBuilder.EquippablePassiveEffectsAssets,
            equippableEffectsList.Count);
        var equippableEffectsOverTimeArr = blobBuilder.Allocate(ref itemBlobBuilder.EquippableEffectsOverTimeAssets,
            equippableEffectsOverTimeList.Count);

        var completableEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.CompletableSuccessActiveEffectsAssets,
            completableEffectList.Count);

        var rangeWeaponSuccessEffectArr =
            blobBuilder.Allocate(ref itemBlobBuilder.RangedWeaponSuccessActiveEffectsAssets,
                rangeWeaponSuccessEffectList.Count);
        var rangeWeaponFailEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.RangedWeaponFailActiveEffectsAssets,
            rangeWeaponFailEffectList.Count);

        var ammoEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.AmmoActiveEffectsAssets, ammoEffectList.Count);

        var meleeWeaponSuccessEffectArr =
            blobBuilder.Allocate(ref itemBlobBuilder.MeleeWeaponSuccessActiveEffectsAssets,
                meleeWeaponSuccessEffectList.Count);
        var meleeWeaponFailEffectArr = blobBuilder.Allocate(ref itemBlobBuilder.MeleeWeaponFailActiveEffectsAssets,
            meleeWeaponFailEffectList.Count);


        for (var i = 0; i < consumeEffectList.Count; i++){
            consumeEffectArr[i] = consumeEffectList[i];
        }

        for (var i = 0; i < readSuccessEffectList.Count; i++){
            readSuccessEffectArr[i] = readSuccessEffectList[i];
        }

        for (var i = 0; i < readFailEffectList.Count; i++){
            readFailEffectArr[i] = readFailEffectList[i];
        }

        for (var i = 0; i < equippableEffectsList.Count; i++){
            equippableEffectsArr[i] = equippableEffectsList[i];
        }

        for (var i = 0; i < equippableEffectsOverTimeList.Count; i++){
            equippableEffectsOverTimeArr[i] = equippableEffectsOverTimeList[i];
        }

        for (var i = 0; i < completableEffectList.Count; i++){
            completableEffectArr[i] = completableEffectList[i];
        }

        for (var i = 0; i < rangeWeaponSuccessEffectList.Count; i++){
            rangeWeaponSuccessEffectArr[i] = rangeWeaponSuccessEffectList[i];
        }

        for (var i = 0; i < rangeWeaponFailEffectList.Count; i++){
            rangeWeaponFailEffectArr[i] = rangeWeaponFailEffectList[i];
        }

        for (var i = 0; i < ammoEffectList.Count; i++){
            ammoEffectArr[i] = ammoEffectList[i];
        }

        for (var i = 0; i < meleeWeaponSuccessEffectList.Count; i++){
            meleeWeaponSuccessEffectArr[i] = meleeWeaponSuccessEffectList[i];
        }

        for (var i = 0; i < meleeWeaponFailEffectList.Count; i++){
            meleeWeaponFailEffectArr[i] = meleeWeaponFailEffectList[i];
        }

        return blobBuilder.CreateBlobAssetReference<ItemBlobAssets>(Allocator.Persistent);
    }
}