using System;

[Serializable]
public struct DynamicConditionTypeDTO {
    public AcademicConditionType academicConditionType;
    public ActivityConditionType activityConditionType;
    public AttributeConditionType attributeConditionType;
    public CharacterConditionType characterConditionType;
    public InteractableConditionType interactableConditionType;
    public InterestConditionType interestConditionType;
    public InventoryConditionType inventoryConditionType;
    public ItemConditionType itemConditionType;
    public LocomotionConditionType locomotionConditionType;
    public MiscConditionType miscConditionType;
    public RelationshipConditionType relationshipConditionType;
    public SkillConditionType skillConditionType;
    public TimeConditionType timeConditionType;
    public TraitConditionType traitConditionType;
    public WellnessConditionType wellnessConditionType;

    public DynamicConditionType ToData(){
        var val = GetEnumIntValue(out ConditionCategoryType type);
        return new DynamicConditionType(){
            Value = val,
            Category = type
        };
    }

    private int GetEnumIntValue(out ConditionCategoryType enumType){
        if (academicConditionType != AcademicConditionType.NONE){
            enumType = ConditionCategoryType.ACADEMIC;
            return (int) academicConditionType;
        }
        if (activityConditionType != ActivityConditionType.NONE){
            enumType = ConditionCategoryType.ACTIVITY;
            return (int) activityConditionType;
        }
        if (attributeConditionType != AttributeConditionType.NONE){
            enumType = ConditionCategoryType.ATTRIBUTES;
            return (int) attributeConditionType;
        }
        if (characterConditionType != CharacterConditionType.NONE){
            enumType = ConditionCategoryType.CHARACTER;
            return (int) characterConditionType;
        }
        if (interactableConditionType != InteractableConditionType.NONE){
            enumType = ConditionCategoryType.INTERACTABLE;
            return (int) interactableConditionType;
        }
        if (itemConditionType != ItemConditionType.NONE){
            enumType = ConditionCategoryType.ITEMS;
            return (int) itemConditionType;
        }
        if (interestConditionType != InterestConditionType.NONE){
            enumType = ConditionCategoryType.INTEREST;
            return (int) interestConditionType;
        }
        if (inventoryConditionType != InventoryConditionType.NONE){
            enumType = ConditionCategoryType.INVENTORY;
            return (int) inventoryConditionType;
        }
        if (locomotionConditionType != LocomotionConditionType.NONE){
            enumType = ConditionCategoryType.LOCOMOTION;
            return (int) locomotionConditionType;
        }
        if (miscConditionType != MiscConditionType.NONE){
            enumType = ConditionCategoryType.MISC;
            return (int) miscConditionType;
        }
        if (relationshipConditionType != RelationshipConditionType.NONE){
            enumType = ConditionCategoryType.RELATIONSHIP;
            return (int) relationshipConditionType;
        }
        if (skillConditionType != SkillConditionType.NONE){
            enumType = ConditionCategoryType.SKILL;
            return (int) skillConditionType;
        }
        if (timeConditionType != TimeConditionType.NONE){
            enumType = ConditionCategoryType.TIME;
            return (int) timeConditionType;
        }
        if (traitConditionType != TraitConditionType.NONE){
            enumType = ConditionCategoryType.TRAITS;
            return (int) traitConditionType;
        }
        if (wellnessConditionType != WellnessConditionType.NONE){
            enumType = ConditionCategoryType.WELLNESS;
            return (int) wellnessConditionType;
        }

        enumType = default;
        return 0;
    }
}