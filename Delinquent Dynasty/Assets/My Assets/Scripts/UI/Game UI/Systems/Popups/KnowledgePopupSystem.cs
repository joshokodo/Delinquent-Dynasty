using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class KnowledgePopupSystem : SystemBase {
    private KnowledgePopup _popupClone;
    private bool _setPop;
    private bool _isOpen;
    private StringBuilder _mainTextStringBuilder;
    private StringBuilder _headerTextStringBuilder;

    private StringBuilder _stringBuilder1 = new();
    private StringBuilder _stringBuilder2 = new();
    private StringBuilder _stringBuilder3 = new();

    private bool _triggerUpdate;
    private bool _triggerDetailsSelected;
    private bool _triggerExitDetails;

    protected override void OnStartRunning(){
        base.OnStartRunning();
        _mainTextStringBuilder = new StringBuilder();
        _headerTextStringBuilder = new StringBuilder();
    }

    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<KnowledgePopupComponent>()){
                _popupClone = (KnowledgePopup) pop.Popup.Clone();
                _popupClone.triggerUpdate += TriggerUpdateKnowledge;
                _popupClone.DetailsPanelSelected += TriggerDetailsSelected;
                _popupClone.ExitDetailsPanelTriggered += TriggerDetailsClosed;
                _popupClone.DetailsPanelTabSelected += TriggerDetailsTabSelected;
            }

            _setPop = true;
        }
        
        if (SystemAPI.TryGetSingletonEntity<SelectedCharacter>(out Entity e)){
            var selectedAspect = SystemAPI.GetAspect<SelectedCharacterAspect>(e);
   
            
            var showUi = selectedAspect.Selected.ValueRW.ShowKnowledgeUI;
            if (showUi && !_popupClone.isActiveAndEnabled){
                var dailyEvents = SystemAPI.GetBuffer<DailyEventsKnowledgeElement>(e);
                var characters = SystemAPI.GetBuffer<CharacterKnowledgeElement>(e);
                var rooms = SystemAPI.GetBuffer<RoomKnowledgeElement>(e);
                var access = SystemAPI.GetBuffer<AccessKnowledgeElement>(e);
                _popupClone.Show();
                UpdateKnowledge(dailyEvents, characters, rooms, access);
            }
            else if (!showUi && _popupClone.isActiveAndEnabled){
                _popupClone.Close();
            }

            if (_popupClone.isActiveAndEnabled){
                if (_triggerDetailsSelected){
                    RelationshipDetailsSelected();
                }
                else if (selectedAspect.Selected.ValueRW.UpdateKnowledgeUI || _triggerUpdate || _triggerExitDetails){
                    selectedAspect.Selected.ValueRW.UpdateKnowledgeUI = false;

                    if (_triggerExitDetails){
                        _popupClone.CharactersListGameObject.SetActive(true);
                        _popupClone.CharacterDetailsGameObject.SetActive(false);
                    }
                    var dailyEvents = SystemAPI.GetBuffer<DailyEventsKnowledgeElement>(e);
                    var characters = SystemAPI.GetBuffer<CharacterKnowledgeElement>(e);
                    var rooms = SystemAPI.GetBuffer<RoomKnowledgeElement>(e);
                    var access = SystemAPI.GetBuffer<AccessKnowledgeElement>(e);
                    UpdateKnowledge(dailyEvents, characters, rooms, access);
                }
            }

            if (_triggerUpdate){
                _triggerUpdate = false;
            }

            if (_triggerDetailsSelected){
                _triggerDetailsSelected = false;
            }

            if (_triggerExitDetails){
                _triggerExitDetails = false;
            }
        }
    }

    private void TriggerDetailsTabSelected(){
        RelationshipDetailsSelected();
    }

    private void TriggerDetailsClosed(){
        _triggerExitDetails = true;
    }

    private void TriggerDetailsSelected(){
        _triggerDetailsSelected = true;
    }

    private void TriggerUpdateKnowledge(){
        _triggerUpdate = true;
    }

    private void RelationshipDetailsSelected(){
        if (_popupClone.CharactersListGameObject.activeSelf){
            _popupClone.CharactersListGameObject.SetActive(false);
            _popupClone.CharacterDetailsGameObject.SetActive(true);
        }

        UpdateCharacterDetailsPanel(SystemAPI.GetSingletonEntity<SelectedCharacter>(),
            _popupClone.CharacterDetailsEntity);
    }


    private void UpdateKnowledge(DynamicBuffer<DailyEventsKnowledgeElement> dailyEvents,
        DynamicBuffer<CharacterKnowledgeElement> characters, DynamicBuffer<RoomKnowledgeElement> rooms,
        DynamicBuffer<AccessKnowledgeElement> access){
        switch (_popupClone.Tabs.SelectedTab.Name){
            case "Events":
                _popupClone.eventsKnowledgeListView.DataSource.Clear();
                _popupClone.eventsKnowledgeListView.DataSource.EndUpdate();
                foreach (var k in dailyEvents){
                    if (SystemAPI.GetComponent<DailyEventsKnowledgeComponent>(k.KnowledgeEntity)
                        .MatchesTime(SystemAPI.GetSingleton<InGameTime>())){
                        var buffer = SystemAPI.GetBuffer<EventKnowledgeElement>(k.KnowledgeEntity);
                        foreach (var eventKnowledgeElement in buffer){
                            _popupClone.eventsKnowledgeListView.DataSource.Add(
                                BuildEventUiElement(eventKnowledgeElement));
                        }

                        break;
                    }
                }

                _popupClone.eventsKnowledgeListView.DataSource.BeginUpdate();
                break;

            case "Characters":

                _popupClone.charactersKnowledgeListView.DataSource.Clear();
                _popupClone.charactersKnowledgeListView.DataSource.EndUpdate();
                foreach (var k in characters){
                    _popupClone.charactersKnowledgeListView.DataSource.Add(BuildCharactersUiElement(k));
                }

                _popupClone.charactersKnowledgeListView.DataSource.BeginUpdate();
                break;
            case "Access":
                _popupClone.accessKnowledgeListView.DataSource.Clear();
                _popupClone.accessKnowledgeListView.DataSource.EndUpdate();
                foreach (var k in access){
                    if (SystemAPI.HasBuffer<PhoneNumberKnowledgeElement>(k.KnowledgeEntity)){
                        _popupClone.accessKnowledgeListView.DataSource.Add(BuildAccessPhoneNumberUiElement(k));
                    }

                    if (SystemAPI.HasBuffer<SecurityCodeAccessKnowledgeElement>(k.KnowledgeEntity)){
                        _popupClone.accessKnowledgeListView.DataSource.Add(BuildAccessSecurityUiElement(k));
                    }
                }

                _popupClone.accessKnowledgeListView.DataSource.BeginUpdate();
                break;
        }
    }

    private CharactersKnowledgeElementUI BuildCharactersUiElement(CharacterKnowledgeElement knowledgeElement){
        _mainTextStringBuilder.Clear();
        _headerTextStringBuilder.Clear();

        CharactersKnowledgeElementUI result = new CharactersKnowledgeElementUI();

        var attrBuffer = SystemAPI.GetBuffer<LastKnownAttributeElement>(knowledgeElement.KnowledgeEntity);
        var wellnessBuffer = SystemAPI.GetBuffer<LastKnownWellnessElement>(knowledgeElement.KnowledgeEntity);
        var comp = SystemAPI.GetComponent<CharacterKnowledgeComponent>(knowledgeElement.KnowledgeEntity);
        result.Title = SystemAPI.GetComponent<CharacterBio>(comp.CharacterEntity).FullName;
        result.Character = comp.CharacterEntity;

        foreach (var attribute in CommonUtils.GetValues<AttributeType>()){
            if (attribute == AttributeType.NONE) continue;
            var found = false;
            foreach (var at in attrBuffer){
                if (at.AttributeType == attribute){
                    found = true;
                    switch (attribute){
                        case AttributeType.STRENGTH:
                            result.Strength = at.Value.ToString();
                            break;
                        case AttributeType.VITALITY:
                            result.Vitality = at.Value.ToString();
                            break;
                        case AttributeType.DEXTERITY:
                            result.Dexterity = at.Value.ToString();
                            break;
                        case AttributeType.WISDOM:
                            result.Wisdom = at.Value.ToString();
                            break;
                        case AttributeType.INTELLIGENCE:
                            result.Intelligence = at.Value.ToString();
                            break;
                        case AttributeType.CHARISMA:
                            result.Charisma = at.Value.ToString();
                            break;
                    }

                    break;
                }
            }

            if (!found){
                switch (attribute){
                    case AttributeType.STRENGTH:
                        result.Strength = "??";
                        break;
                    case AttributeType.VITALITY:
                        result.Vitality = "??";
                        break;
                    case AttributeType.DEXTERITY:
                        result.Dexterity = "??";
                        break;
                    case AttributeType.WISDOM:
                        result.Wisdom = "??";
                        break;
                    case AttributeType.INTELLIGENCE:
                        result.Intelligence = "??";
                        break;
                    case AttributeType.CHARISMA:
                        result.Charisma = "??";
                        break;
                }
            }
        }

        foreach (var wellness in CommonUtils.GetValues<WellnessType>()){
            if (wellness == WellnessType.NONE) continue;
            var found = false;
            foreach (var wt in wellnessBuffer){
                if (wt.WellnessType == wellness){
                    found = true;
                    switch (wellness){
                        case WellnessType.HEALTH:
                            result.PhysicalHealth = wt.Value.ToString();
                            break;
                        case WellnessType.SLEEP:
                            result.Sleep = wt.Value.ToString();
                            break;
                        case WellnessType.FOCUS:
                            result.Focus = wt.Value.ToString();
                            break;
                        case WellnessType.HAPPINESS:
                            result.Happiness = wt.Value.ToString();
                            break;
                        case WellnessType.HYGIENE:
                            result.Hygiene = wt.Value.ToString();
                            break;
                        case WellnessType.NOURISHMENT:
                            result.Nourishment = wt.Value.ToString();
                            break;
                        case WellnessType.ENERGY:
                            result.Energy = wt.Value.ToString();
                            break;
                    }

                    break;
                }
            }

            if (!found){
                switch (wellness){
                    case WellnessType.HEALTH:
                        result.PhysicalHealth = "??";
                        break;
                    case WellnessType.SLEEP:
                        result.Sleep = "??";
                        break;
                    case WellnessType.FOCUS:
                        result.Focus = "??";
                        break;
                    case WellnessType.HAPPINESS:
                        result.Happiness = "??";
                        break;
                    case WellnessType.HYGIENE:
                        result.Hygiene = "??";
                        break;
                    case WellnessType.NOURISHMENT:
                        result.Nourishment = "??";
                        break;
                    case WellnessType.ENERGY:
                        result.Energy = "??";
                        break;
                }
            }
        }

        return result;
    }

    private BasicKnowledgeElementUI BuildEventUiElement(EventKnowledgeElement eventKnowledge){
        _mainTextStringBuilder.Clear();
        _headerTextStringBuilder.Clear();

        _headerTextStringBuilder.Append("Source: ");


        if (SystemAPI.HasComponent<CharacterBio>(eventKnowledge.ActionTimestamp.Source)){
            _headerTextStringBuilder.Append(SystemAPI.GetComponent<CharacterBio>(eventKnowledge.ActionTimestamp.Source)
                .FullName);
        }
        else{
            _headerTextStringBuilder.Append("???");
        }

        _headerTextStringBuilder
            .Append(" | ")
            .Append(TimeUtils.GetGameTimeString(eventKnowledge.ActionTimestamp.TotalInGameSeconds))
            .Append(" ")
            .Append(TimeUtils.GetGameDateString(eventKnowledge.ActionTimestamp.Day,
                eventKnowledge.ActionTimestamp.SeasonState,
                eventKnowledge.ActionTimestamp.Year));

        var performer = SystemAPI.GetComponent<CharacterBio>(eventKnowledge.PerformingEntity).FullName;
        var targets = string.Empty;
        if (eventKnowledge.TargetCharacters.Length == 1){
            targets = " on " + SystemAPI.GetComponent<CharacterBio>(eventKnowledge.TargetCharacters[0])
                .FullName;
        }
        else if (eventKnowledge.TargetCharacters.Length > 1){
            targets = " on " + eventKnowledge.TargetCharacters.Length + " characters";
        }

        _mainTextStringBuilder.Append(performer + " performed " +
                                      StringUtils.GetActionTypeString(eventKnowledge.ActionType) +
                                      (eventKnowledge.IsSuccessful ? " Successfully " : " Unsuccessfully ") + targets +
                                      " at " + TimeUtils.GetGameTimeSpanString(eventKnowledge.ActionTimestamp
                                          .TotalInGameSeconds));


        return new BasicKnowledgeElementUI(){
            MainText = new FixedString512Bytes(_mainTextStringBuilder.Replace("_", " ").ToString()),
            HeaderText = new FixedString512Bytes(_headerTextStringBuilder.Replace("_", " ").ToString()),
        };
    }

    private BasicKnowledgeElementUI BuildRoomUiElement(RoomKnowledgeElement roomKnowledge){
        _mainTextStringBuilder.Clear();
        _headerTextStringBuilder.Clear();
        var room = SystemAPI.GetComponent<RoomKnowledgeComponent>(roomKnowledge.KnowledgeEntity);
        var roomComp = SystemAPI.GetComponent<RoomComponent>(room.RoomEntity);


        return new BasicKnowledgeElementUI(){
            MainText = new FixedString512Bytes(_mainTextStringBuilder.Replace("_", " ").ToString()),
            HeaderText = new FixedString512Bytes(_headerTextStringBuilder.Replace("_", " ").ToString()),
        };
    }

    private BasicKnowledgeElementUI BuildAccessPhoneNumberUiElement(AccessKnowledgeElement accessKnowledge){
        _mainTextStringBuilder.Clear();
        _headerTextStringBuilder.Clear();

        var accessComp = SystemAPI.GetComponent<AccessKnowledgeComponent>(accessKnowledge.KnowledgeEntity);
        var phoneNums = SystemAPI.GetBuffer<PhoneNumberKnowledgeElement>(accessKnowledge.KnowledgeEntity);
        var itemType = SystemAPI.GetComponent<ItemBaseComponent>(accessComp.AccessTargetEntity).ItemType;
        var phoneOwner = SystemAPI.GetComponent<ItemCellPhoneComponent>(accessComp.AccessTargetEntity).Owner;
        var ownerName = SystemAPI.GetComponent<CharacterBio>(phoneOwner).FullName;
        _headerTextStringBuilder.Append("Phone Number For " + StringUtils.GetItemTypeString(itemType) + " (" +
                                        ownerName + ")");

        if (!phoneNums.IsEmpty){
            var latest = phoneNums[^1];
            var kSource = latest.Timestamp.Source;
            if (kSource != Entity.Null && SystemAPI.GetComponentLookup<CharacterBio>().TryGetComponent(kSource, out CharacterBio bio)){
                _mainTextStringBuilder.Append( " Latest By " + bio.FullName + " ");
            }
            else{
                _mainTextStringBuilder.Append( " Latest By Unknown ");
            }

            _mainTextStringBuilder.Append(TimeUtils.GetGameDateString(latest.Timestamp));
        }

        return new BasicKnowledgeElementUI(){
            MainText = new FixedString512Bytes(_mainTextStringBuilder.Replace("_", " ").ToString()),
            HeaderText = new FixedString512Bytes(_headerTextStringBuilder.Replace("_", " ").ToString()),
        };
    }


    private BasicKnowledgeElementUI BuildAccessSecurityUiElement(AccessKnowledgeElement accessKnowledge){
        _mainTextStringBuilder.Clear();
        _headerTextStringBuilder.Clear();

        var accessComp = SystemAPI.GetComponent<AccessKnowledgeComponent>(accessKnowledge.KnowledgeEntity);
        var securityAccess = SystemAPI.GetBuffer<SecurityCodeAccessKnowledgeElement>(accessKnowledge.KnowledgeEntity);

        _headerTextStringBuilder.Append("Security Access For ");
        if (SystemAPI.HasComponent<LockerComponent>(accessComp.AccessTargetEntity)){
            _headerTextStringBuilder.Append("Locker");
        }
        else if (SystemAPI.HasComponent<DoorTag>(accessComp.AccessTargetEntity)){
            _headerTextStringBuilder.Append("Door");
        }
        else if (SystemAPI.HasComponent<VendingMachineTag>(accessComp.AccessTargetEntity)){
            _headerTextStringBuilder.Append("Vending Machine");
        }
        else if (SystemAPI.HasComponent<FoodBarComponent>(accessComp.AccessTargetEntity)){
            _headerTextStringBuilder.Append("Food Bar");
        }

        if (!securityAccess.IsEmpty){
            var latest = securityAccess[^1];
            var kSource = latest.Timestamp.Source;
            if (kSource != Entity.Null && SystemAPI.GetComponentLookup<CharacterBio>().TryGetComponent(kSource, out CharacterBio bio)){
                _mainTextStringBuilder.Append( " Latest By " + bio.FullName + " ");
            }
            else{
                _mainTextStringBuilder.Append( " Latest By Unknown ");
            }

            _mainTextStringBuilder.Append(TimeUtils.GetGameDateString(latest.Timestamp));
        }

        return new BasicKnowledgeElementUI(){
            MainText = new FixedString512Bytes(_mainTextStringBuilder.Replace("_", " ").ToString()),
            HeaderText = new FixedString512Bytes(_headerTextStringBuilder.Replace("_", " ").ToString()),
        };
    }


    private void UpdateSkills(Entity target, DynamicBuffer<CharacterKnowledgeElement> characterKnowledgeElements){
        _popupClone.SkillsListView.DataSource.Clear();
        _popupClone.SkillsListView.DataSource.EndUpdate();

        var skillUtils = new SkillUtils(){
            SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>()
        };

        foreach (var knowledgeElement in characterKnowledgeElements){
            var comp = SystemAPI.GetComponent<CharacterKnowledgeComponent>(knowledgeElement.KnowledgeEntity);
            if (comp.CharacterEntity == target){
                var skillsBuffer = SystemAPI.GetBuffer<LastKnownSkillElement>(knowledgeElement.KnowledgeEntity);
                foreach (var lastKnownSkillElement in skillsBuffer){
                    var element = new SkillElementUI(){
                        skillLevel = lastKnownSkillElement.Value,
                        SkillType = lastKnownSkillElement.SkillType,
                        PrimaryAttribute = skillUtils.GetSkillPrimaryAttribute(lastKnownSkillElement.SkillType),
                        SecondaryAttribute = skillUtils.GetSkillSecondaryAttribute(lastKnownSkillElement.SkillType),
                    };
                    _popupClone.SkillsListView.DataSource.Add(element);
                }

                break;
            }
        }

        _popupClone.SkillsListView.DataSource.BeginUpdate();
    }

    private void UpdateInterest(Entity target, DynamicBuffer<CharacterKnowledgeElement> knowledgeElements){
        _popupClone.InterestListView.DataSource.Clear();
        _popupClone.InterestListView.DataSource.EndUpdate();
        var dataStore = SystemAPI.GetSingleton<InterestDataStore>();

        foreach (var knowledgeElement in knowledgeElements){
            var comp = SystemAPI.GetComponent<CharacterKnowledgeComponent>(knowledgeElement.KnowledgeEntity);
            if (comp.CharacterEntity == target){
                var interestBuffer = SystemAPI.GetBuffer<LastKnownInterestElement>(knowledgeElement.KnowledgeEntity);
                foreach (var i in interestBuffer){
                    var effectsString =
                        dataStore.InterestBlobAssets.Value.GetInterestEffectsString(i.SubjectType, i.PrimaryEnumValue);
                    var element = new InterestElementUI(){
                        SubjectType = i.SubjectType,
                        InterestType = i.PrimaryEnumValue,
                        Ratio = new InterestElement(){InterestValue = i.Value}.Ratio, // todo dont like. fix this
                        InterestEffects = effectsString
                    };
                    _popupClone.InterestListView.DataSource.Add(element);
                }
            }
        }

        _popupClone.InterestListView.DataSource.BeginUpdate();
    }

    private void UpdateGeneralBio(Entity target, DynamicBuffer<CharacterKnowledgeElement> knowledgeElements){
        _stringBuilder1.Clear();
        _stringBuilder2.Clear();
        _stringBuilder3.Clear();

        var traitDataStore = SystemAPI.GetSingleton<TraitDataStore>();
        var traitElements = SystemAPI.GetBuffer<TraitElement>(target);
        var bio = SystemAPI.GetComponent<CharacterBio>(target);

        _popupClone.NameTitleText.text =
            bio.FullName + "\n" + (bio.IsFemale ? "Female" : "Male") + "\n";

        if (SystemAPI.HasComponent<StudentBio>(target)){
            _popupClone.NameTitleText.text += SystemAPI.GetComponent<StudentBio>(target).Seniority;
        }

        _popupClone.InfoText.text = "My story has yet to be written!";

        foreach (var characterKnowledgeElement in knowledgeElements){
            var comp = SystemAPI.GetComponent<CharacterKnowledgeComponent>(characterKnowledgeElement.KnowledgeEntity);
            if (comp.CharacterEntity == target){
                var traits = SystemAPI.GetBuffer<LastKnownTraitElement>(characterKnowledgeElement.KnowledgeEntity);
                foreach (var trait in traits){
                    if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(trait.TraitType, TraitCategory.DEFAULT)
                        || traitDataStore.TraitBlobAssets.Value.IsTraitCategory(trait.TraitType,
                            TraitCategory.GENETIC)){
                        _stringBuilder1.Append(StringUtils.GetTraitTypeString(trait.TraitType));
                        if (trait.Value > 1){
                            _stringBuilder1.Append(" " + trait.Value + " ");
                        }
                        else{
                            _stringBuilder1.Append(", ");
                        }
                    }
                    else if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(trait.TraitType,
                                 TraitCategory.PERSONALITY)){
                        _stringBuilder2.Append(StringUtils.GetTraitTypeString(trait.TraitType));
                        if (trait.Value > 1){
                            _stringBuilder2.Append(" " + trait.Value + " ");
                        }
                        else{
                            _stringBuilder2.Append(", ");
                        }
                    }
                }

                break;
            }
        }

        for (var i = 0; i < traitElements.Length; i++){
            var traitElement = traitElements[i];

            if (traitElement.Intensity == 0){
                continue;
            }

            if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType, TraitCategory.STATUS)
                || traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType,
                    TraitCategory.PERSONAL_MAINTENANCE)){
                _stringBuilder3.Append(StringUtils.GetTraitTypeString(traitElement.TraitType));
                if (traitElement.Intensity > 1){
                    _stringBuilder3.Append(" " + traitElement.Intensity + " ");
                }
                else{
                    _stringBuilder3.Append(", ");
                }
            }
        }

        _popupClone.GenenticTraitsText.text = _stringBuilder1.Replace("_", " ").ToString();
        _popupClone.PersonalityTraitsText.text = _stringBuilder2.Replace("_", " ").ToString();
        _popupClone.PersonalStatusTraitsText.text = _stringBuilder3.Replace("_", " ").ToString();
    }

    private void UpdateAcademics(Entity e){ }

    private void UpdateRelationships(Entity e){
        _popupClone.RelationshipListView.DataSource.Clear();
        _popupClone.RelationshipListView.DataSource.EndUpdate();

        var characterBioLookup = SystemAPI.GetComponentLookup<CharacterBio>();
        var relationshipsLookup = SystemAPI.GetBufferLookup<RelationshipElement>();
        var relationships = relationshipsLookup[e];

        foreach (var relationship in relationships){
            _stringBuilder1.Clear();
            var element = new RelationshipElementUI();
            var theyHaveAnyRelationship = relationshipsLookup.TryGetBuffer(relationship.Character,
                out DynamicBuffer<RelationshipElement> theirRelationships);
            var theirRelationshipForYou = new RelationshipElement();

            if (theyHaveAnyRelationship){
                foreach (var relationshipElement in theirRelationships){
                    if (relationshipElement.Character == e){
                        theirRelationshipForYou = relationshipElement;
                        break;
                    }
                }
            }

            _stringBuilder1.Append("(" + relationship.MainTitle + " ");
            for (var i = 0; i < relationship.SubTitles.Length; i++){
                var relationshipTitleType = relationship.SubTitles[i];
                _stringBuilder1.Append(relationshipTitleType);
                if (i != relationship.SubTitles.Length - 1){
                    _stringBuilder1.Append(", ");
                }
            }

            _stringBuilder1.Append(") ");
            _stringBuilder1.Append(characterBioLookup[relationship.Character].FullName);

            element.TitleText = new FixedString512Bytes(_stringBuilder1.ToString());

            _stringBuilder1.Clear();
            _stringBuilder1.Append("Y: " + relationship.Admiration);
            element.YourAdmirationText = new FixedString128Bytes(_stringBuilder1.ToString());

            _stringBuilder1.Clear();
            _stringBuilder1.Append("Y: " + relationship.Attraction);
            element.YourAttractionText = new FixedString128Bytes(_stringBuilder1.ToString());

            _stringBuilder1.Clear();
            _stringBuilder1.Append("Y: " + relationship.Fear);
            element.YourFearText = new FixedString128Bytes(_stringBuilder1.ToString());

            _stringBuilder1.Clear();
            _stringBuilder1.Append("Y: " + relationship.Entitlement);
            element.YourEntitlmentText = new FixedString128Bytes(_stringBuilder1.ToString());

            if (theyHaveAnyRelationship && theirRelationshipForYou.Character == e){
                _stringBuilder1.Clear();
                _stringBuilder1.Append("T: " + theirRelationshipForYou.Admiration);
                element.TheirAdmirationText = new FixedString128Bytes(_stringBuilder1.ToString());

                _stringBuilder1.Clear();
                _stringBuilder1.Append("T: " + theirRelationshipForYou.Attraction);
                element.TheirAttractionText = new FixedString128Bytes(_stringBuilder1.ToString());

                _stringBuilder1.Clear();
                _stringBuilder1.Append("T: " + theirRelationshipForYou.Fear);
                element.TheirFearText = new FixedString128Bytes(_stringBuilder1.ToString());

                _stringBuilder1.Clear();
                _stringBuilder1.Append("T: " + theirRelationshipForYou.Entitlement);
                element.TheirEntitlementText = new FixedString128Bytes(_stringBuilder1.ToString());
            }
            else{
                element.TheirAdmirationText = new FixedString128Bytes("T: ???");
                element.TheirAttractionText = new FixedString128Bytes("T: ???");
                element.TheirFearText = new FixedString128Bytes("T: ???");
                element.TheirEntitlementText = new FixedString128Bytes("T: ???");
            }


            _popupClone.RelationshipListView.DataSource.Add(element);
        }

        _popupClone.RelationshipListView.DataSource.BeginUpdate();
    }

    private void UpdateCharacterDetailsPanel(Entity e, Entity target){
        var characterKnowledge = SystemAPI.GetBuffer<CharacterKnowledgeElement>(e);
        switch (_popupClone.CharacterDetailsTabs.SelectedTab.Name){
            case "Bio":
                UpdateGeneralBio(target, characterKnowledge);
                break;
            case "Interest":
                UpdateInterest(target, characterKnowledge);
                break;
            case "Skills":
                UpdateSkills(target, characterKnowledge);
                break;
            case "Relationships":
                // UpdateRelationships(target);
                break;
            case "Academics":
                UpdateAcademics(target);
                break;
        }
    }
}