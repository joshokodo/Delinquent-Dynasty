using System;
using TMPro;
using UIWidgets;
using UnityEngine;

public class CharacterBioPopup : Popup {
    public Tabs Tabs;
    public RelationshipsListView RelationshipListView;
    public SkillsListView SkillsListView;
    public InterestListView InterestListView;
    public TextMeshProUGUI InfoText;
    public TextMeshProUGUI NameTitleText;
    public TextMeshProUGUI GenenticTraitsText;
    public TextMeshProUGUI PersonalityTraitsText;
    public TextMeshProUGUI PersonalStatusTraitsText;

// todo make into event
    public bool TriggerUpdate;

    public void TriggerPopupUpdate(){
        TriggerUpdate = true;
    }
}