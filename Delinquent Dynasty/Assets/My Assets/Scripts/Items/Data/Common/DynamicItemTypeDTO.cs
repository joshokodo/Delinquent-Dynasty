using System;
using UnityEngine.Serialization;

[Serializable]
public struct DynamicItemTypeDTO {
    public ClothingItemCategory clothingCategory;
    public MeleeWeaponItemCategory meleeCategory;
    public RangedWeaponItemCategory rangedCategory;
    public BookItemCategory bookCategory;
    public SecurityItemCategory securityCategory;
    public FoodItemCategory foodCategory;
    public TechItemCategory techCategory;
    public PracticalUseItemCategory practicalUseCategory;
    public MiscItemCategory miscCategory;
    public CurrencyItemCategory currencyCategory;
    public bool IsNull => ToData().IsNull;

    public DynamicItemType ToData(){
        if (clothingCategory != ClothingItemCategory.NONE){
            return new DynamicItemType(clothingCategory);
        }

        if (meleeCategory != MeleeWeaponItemCategory.NONE){
            return new DynamicItemType(meleeCategory);
        }

        if (rangedCategory != RangedWeaponItemCategory.NONE){
            return new DynamicItemType(rangedCategory);
        }

        if (bookCategory != BookItemCategory.NONE){
            return new DynamicItemType(bookCategory);
        }

        if (securityCategory != SecurityItemCategory.NONE){
            return new DynamicItemType(securityCategory);
        }

        if (foodCategory != FoodItemCategory.NONE){
            return new DynamicItemType(foodCategory);
        }

        if (techCategory != TechItemCategory.NONE){
            return new DynamicItemType(techCategory);
        }

        if (practicalUseCategory != PracticalUseItemCategory.NONE){
            return new DynamicItemType(practicalUseCategory);
        }

        if (miscCategory != MiscItemCategory.NONE){
            return new DynamicItemType(miscCategory);
        }

        if (currencyCategory != CurrencyItemCategory.NONE){
            return new DynamicItemType(currencyCategory);
        }

        return new DynamicItemType();
    }
}