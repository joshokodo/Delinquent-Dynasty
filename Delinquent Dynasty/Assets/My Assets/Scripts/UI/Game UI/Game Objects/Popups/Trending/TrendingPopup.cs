using UIWidgets;

public class TrendingPopup : Popup {
    public TrendingListView TrendingListView;
    public bool FilterSelected;
    public bool AllSelected;
    public GameEnumType SelectedFilter;

    public void AllFilterSelected(){
        FilterSelected = true;
        AllSelected = true;
    }

    public void CharactersFilterSelected(){
        FilterSelected = true;
    }

    public void SkillsFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.SKILL_TYPE;
    }

    public void ItemssFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.CLOTHING_ITEM_CATEGORY;
    }

    public void InterestsFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.GENERAL_INTEREST_TYPE;
    }

    public void AttributesFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.ATTRIBUTE_TYPE;
    }

    public void WellnessFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.WELLNESS_TYPE;
    }

    public void TraitsFilterSelected(){
        FilterSelected = true;
        SelectedFilter = GameEnumType.DEFAULT_TRAIT_CATEGORY;
    }
}