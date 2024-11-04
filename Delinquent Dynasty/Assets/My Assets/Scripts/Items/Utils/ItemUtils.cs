using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public struct ItemUtils : FunctionalStruct {
    public ItemDataStore ItemDataStore;

    public bool RemoveItemFromInventoryBuffer(Entity itemEntity, DynamicBuffer<ItemElement> items,
        ComponentLookup<ItemStackComponent> stackLookup,
        EntityCommandBuffer ecb, out Entity removedItemEntity, int removeCount = 1){
        removedItemEntity = Entity.Null;
        for (int i = 0; i < items.Length; i++){
            var isStackable = stackLookup.TryGetComponent(itemEntity, out ItemStackComponent stackComponent);
            if (items[i].ItemEntity == itemEntity){
                if (!isStackable || removeCount >= stackComponent.stackCount){
                    removedItemEntity = items[i].ItemEntity;
                    items.RemoveAt(i);
                }
                else{
                    stackComponent.stackCount -= removeCount;
                    ecb.SetComponent(itemEntity, stackComponent);
                }

                return true;
            }
        }

        return false;
    }

    public bool RemoveItemElementFromInventoryBuffer(Entity itemEntity, DynamicBuffer<ItemElement> items,
        out ItemElement itemElement){
        for (int i = 0; i < items.Length; i++){
            if (items[i].ItemEntity == itemEntity){
                itemElement = items[i];
                items.RemoveAt(i);
                return true;
            }
        }

        itemElement = default;
        return false;
    }

    public bool TransformItemFromInventoryBuffer(Entity inventoryEntity, DynamicItemType transformAfterRemove,
        Entity itemEntity, ComponentLookup<ItemStackComponent> stackLookup, DynamicBuffer<ItemElement> items,
        EntityCommandBuffer ecb, DynamicBuffer<ItemSpawnElement> itemSpawnElements, InGameTime inGameTime,
        int removeCount = 1){
        for (int i = 0; i < items.Length; i++){
            var isStackable = stackLookup.TryGetComponent(itemEntity, out ItemStackComponent stackComponent);
            if (items[i].ItemEntity == itemEntity){
                if (!isStackable || removeCount >= stackComponent.stackCount){
                    ecb.AddComponent(itemEntity, new TransformItemComponent(){ItemType = transformAfterRemove});
                }
                else{
                    stackComponent.stackCount -= removeCount;
                    ecb.SetComponent(itemEntity, stackComponent);
                    TriggerCreateItemFor(inventoryEntity, itemSpawnElements, transformAfterRemove, removeCount);
                }

                return true;
            }
        }

        return false;
    }

    public void TriggerCreateItemFor(Entity inventoryEntity, DynamicBuffer<ItemSpawnElement> itemSpawnElements,
        DynamicItemType itemType, int count = 1, bool isWip = false){
        itemSpawnElements.Add(new ItemSpawnElement(){
            Items = new FixedList4096Bytes<ItemSpawnData>(){
                new ItemSpawnData(){
                    Count = count,
                    ItemType = itemType,
                    IsWorkInProgress = isWip
                }
            },
            Owner = inventoryEntity
        });
    }

    // public void TransferCashTo(DynamicBuffer<ItemElement> items, ComponentLookup<ItemStackComponent> stackComponent, ComponentLookup<ItemBaseComponent> itemBaseLookup, DynamicBuffer<ItemElement> inventory, Entity targetInventory, EntityCommandBuffer ecb, DynamicBuffer<ItemSpawnElement> itemSpawnElements, InGameTime inGameTime, int transferAmount){
    //     
    //     // var originCash = stackComponent[cashEntity];
    //     // var transferAllMoney = transferAmount >= originCash.stackCount;
    //     // for (int i = 0; i < items.Length; i++){
    //     //     if (items[i].ItemEntity == cashEntity){
    //     //         
    //     //         var cashExists = false;
    //     //         foreach (var itemElement in inventory){
    //     //             if (itemBaseLookup[itemElement.ItemEntity].ItemType == ItemType.CASH){
    //     //                 var existingCash = stackComponent[itemElement.ItemEntity];
    //     //                 existingCash.stackCount += transferAmount;
    //     //                 ecb.SetComponent(itemElement.ItemEntity, existingCash);
    //     //                 cashExists = true;
    //     //                 if (transferAllMoney){
    //     //                     ecb.AddComponent(cashEntity, new TransformItemTag(){ItemType = ItemType.NONE});
    //     //                     v.RemoveAt(i);
    //     //                 } else {
    //     //                     originCash.stackCount -= transferAmount;
    //     //                     ecb.SetComponent(cashEntity, originCash);
    //     //                 }
    //     //                 break;
    //     //             }
    //     //         }
    //     //
    //     //         if (!cashExists){
    //     //             if (transferAllMoney){
    //     //                 ecb.AppendToBuffer(targetInventory, items[i]);
    //     //                 items.RemoveAt(i);
    //     //             } else {
    //     //                 originCash.stackCount -= transferAmount;
    //     //                 ecb.SetComponent(cashEntity, originCash);
    //     //                 TriggerCreateItemsFor(targetInventory, itemSpawnElements, ItemType.CASH, transferAmount);
    //     //             }
    //     //             break;
    //     //         }
    //     //     }
    //     // }
    // }

    public void TransferCashTo(EntityCommandBuffer ecb, DynamicBuffer<ItemSpawnElement> itemSpawnElements,
        BufferLookup<ItemElement> itemsLookup, ComponentLookup<ItemStackComponent> stackLookup,
        ComponentLookup<ItemBaseComponent> itemBaseLookup, Entity origin, Entity target, int transferAmount){
        var originInv = itemsLookup[origin];
        RemoveCash(origin, Entity.Null, itemsLookup, ecb, originInv, transferAmount, stackLookup, itemBaseLookup);
        TriggerCreateItemFor(target, itemSpawnElements, new DynamicItemType(CurrencyItemCategory.CASH), transferAmount);
    }

    private int RemoveCash(Entity origin, Entity parent, BufferLookup<ItemElement> itemsLookup, EntityCommandBuffer ecb,
        DynamicBuffer<ItemElement> inventory, int remaining, ComponentLookup<ItemStackComponent> stackLookup,
        ComponentLookup<ItemBaseComponent> itemBaseLookup){
        for (var i = 0; i < inventory.Length; i++){
            var itemElement = inventory[i];
            var itemType = itemBaseLookup[itemElement.ItemEntity].ItemType;
            if (remaining <= 0){
                break;
            }

            if (itemType.Matches(new DynamicItemType(CurrencyItemCategory.CASH))){
                var stack = stackLookup[itemElement.ItemEntity];
                if (stack.stackCount <= remaining){
                    remaining -= stack.stackCount;
                    ecb.AddComponent(itemElement.ItemEntity, new TransformItemComponent(){
                        ItemType = default,
                        ActualInventorySource = origin,
                    });
                }
                else if (stack.stackCount > remaining){
                    stack.stackCount -= remaining;
                    ecb.SetComponent(itemElement.ItemEntity, stack);
                    remaining = 0;
                }
            }
            else if (itemsLookup.TryGetBuffer(itemElement.ItemEntity, out DynamicBuffer<ItemElement> innerInventory)){
                remaining = RemoveCash(itemElement.ItemEntity, origin, itemsLookup, ecb, innerInventory, remaining,
                    stackLookup, itemBaseLookup);
            }
        }

        return remaining;
    }

    public bool HasConflictingEquipmentTypes(DynamicItemType a, DynamicItemType b){
        var dataA = ItemDataStore.ItemBlobAssets.Value.GetItemEquippableData(a);
        var dataB = ItemDataStore.ItemBlobAssets.Value.GetItemEquippableData(b);
        return dataA.EquipmentType == dataB.EquipmentType;
    }

    public bool TryGetEquippedRangedWeaponComponent(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemRangedWeaponComponent> rangedWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out ItemRangedWeaponComponent rangedWeaponComponent){
        foreach (var itemElement in inventory){
            if (rangedWeaponLookup.TryGetComponent(itemElement.ItemEntity,
                    out ItemRangedWeaponComponent weaponComponent)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    rangedWeaponComponent = weaponComponent;
                    return true;
                }
            }
        }

        rangedWeaponComponent = default;
        return false;
    }

    public bool TryGetEquippedRangedWeaponEntity(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemRangedWeaponComponent> rangedWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out Entity rangedWeaponEntity){
        foreach (var itemElement in inventory){
            if (rangedWeaponLookup.HasComponent(itemElement.ItemEntity)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    rangedWeaponEntity = itemElement.ItemEntity;
                    return true;
                }
            }
        }

        rangedWeaponEntity = Entity.Null;
        return false;
    }

    public bool TryGetEquippedRangedWeaponComponentAndEntity(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemRangedWeaponComponent> rangedWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out ItemRangedWeaponComponent rangedWeaponComponent,
        out Entity rangedWeaponEntity){
        foreach (var itemElement in inventory){
            if (rangedWeaponLookup.TryGetComponent(itemElement.ItemEntity,
                    out ItemRangedWeaponComponent weaponComponent)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    rangedWeaponComponent = weaponComponent;
                    rangedWeaponEntity = itemElement.ItemEntity;
                    return true;
                }
            }
        }

        rangedWeaponComponent = default;
        rangedWeaponEntity = Entity.Null;
        return false;
    }

    public bool TryGetEquippedMeleeWeaponEntity(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemMeleeWeaponComponent> meleeWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out Entity weaponEntity){
        foreach (var itemElement in inventory){
            if (meleeWeaponLookup.HasComponent(itemElement.ItemEntity)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    weaponEntity = itemElement.ItemEntity;
                    return true;
                }
            }
        }

        weaponEntity = Entity.Null;
        return false;
    }

    public bool TryGetEquippedMeleeWeaponComponentAndEntity(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemMeleeWeaponComponent> meleeWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out ItemMeleeWeaponComponent meleeWeaponComponent,
        out Entity meleeWeaponEntity){
        foreach (var itemElement in inventory){
            if (meleeWeaponLookup.TryGetComponent(itemElement.ItemEntity,
                    out ItemMeleeWeaponComponent weaponComponent)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    meleeWeaponComponent = weaponComponent;
                    meleeWeaponEntity = itemElement.ItemEntity;
                    return true;
                }
            }
        }

        meleeWeaponComponent = default;
        meleeWeaponEntity = Entity.Null;
        return false;
    }

    public bool TryGetEquippedMeleeWeaponComponent(DynamicBuffer<ItemElement> inventory,
        ComponentLookup<ItemRangedWeaponComponent> rangedWeaponLookup,
        ComponentLookup<ItemEquippableComponent> equipmentLookup, out ItemRangedWeaponComponent rangedWeaponComponent){
        foreach (var itemElement in inventory){
            if (rangedWeaponLookup.TryGetComponent(itemElement.ItemEntity,
                    out ItemRangedWeaponComponent weaponComponent)){
                if (equipmentLookup[itemElement.ItemEntity].isEquipped){
                    rangedWeaponComponent = weaponComponent;
                    return true;
                }
            }
        }

        rangedWeaponComponent = default;
        return false;
    }

    public double GetInventoryTotalMoney(Entity e, BufferLookup<ItemElement> itemsLookup,
        ComponentLookup<ItemBaseComponent> baseLookup, ComponentLookup<ItemStackComponent> stackableLookup){
        var result = 0d;
        var inventory = itemsLookup[e];
        foreach (var itemElement in inventory){
            var type = baseLookup[itemElement.ItemEntity].ItemType;
            if (type.Matches(new DynamicItemType(CurrencyItemCategory.CASH)) &&
                stackableLookup.TryGetComponent(itemElement.ItemEntity, out ItemStackComponent stack)){
                result += stack.stackCount;
            }
            // todo make this recursive if nested inventories can have nested inventories
            else if (ItemDataStore.ItemBlobAssets.Value.HasInventory(type) &&
                     itemsLookup.TryGetBuffer(itemElement.ItemEntity, out DynamicBuffer<ItemElement> innerInventory)){
                foreach (var innerItemElement in innerInventory){
                    var innerType = baseLookup[innerItemElement.ItemEntity].ItemType;
                    if (innerType.Matches(new DynamicItemType(CurrencyItemCategory.CASH)) &&
                        stackableLookup.TryGetComponent(innerItemElement.ItemEntity,
                            out ItemStackComponent innerStack)){
                        result += innerStack.stackCount;
                    }
                }
            }
        }

        return result;
    }
}