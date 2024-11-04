public struct EffectCommonStatData {
    public DynamicGameEnum PrimaryEnum;
    public DynamicGameEnum SecondaryEnum;
    public int ValueX;
    public int ValueY;
    
    public KnowledgeType GetKnowledgeType(){
        if (!SecondaryEnum.IsBlank() && PrimaryEnum.Type == GameEnumType.INTEREST_SUBJECT_TYPE){
            return KnowledgeType.LAST_KNOWN_INTEREST;
        }

        switch (PrimaryEnum.Type){
            case GameEnumType.ATTRIBUTE_TYPE:
                return KnowledgeType.LAST_KNOWN_ATTRIBUTE;
            case GameEnumType.SKILL_TYPE:
                return KnowledgeType.LAST_KNOWN_SKILL;
            case GameEnumType.WELLNESS_TYPE:
                return KnowledgeType.LAST_KNOWN_WELLNESS;
        }

        return default;
    }
}