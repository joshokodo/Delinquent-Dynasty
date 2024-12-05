using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class CharacterBioPopupSystem : SystemBase {
    // private PlayerInputs _playerInputs;
    private CharacterBioPopup _popupClone;
    private bool _setPop;
    private bool _isOpen;
    private StringBuilder _stringBuilder1 = new();
    private StringBuilder _stringBuilder2 = new();
    private StringBuilder _stringBuilder3 = new();

    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<CharacterBioPopupComponent>()){
                _popupClone = (CharacterBioPopup) pop.Popup.Clone();
            }

            _setPop = true;
        }

        if (SystemAPI.TryGetSingletonEntity<SelectedCharacter>(out Entity e)){
            var selectedAspect = SystemAPI.GetAspect<SelectedCharacterAspect>(e);
            var showUi = selectedAspect.Selected.ValueRO.ShowCharacterBioUI;

            if (showUi && !_popupClone.isActiveAndEnabled){
                _popupClone.Show();
                UpdateRelationships(e);
            }
            else if (!showUi && _popupClone.isActiveAndEnabled){
                _popupClone.Close();
            }

            if (_popupClone.isActiveAndEnabled && selectedAspect.Selected.ValueRO.UpdateCharacterBioUI){
                selectedAspect.Selected.ValueRW.UpdateCharacterBioUI = false;
                UpdatePopup(e);
            }
            else if (_popupClone.isActiveAndEnabled && _popupClone.TriggerUpdate){
                _popupClone.TriggerUpdate = false;
                UpdatePopup(e);
            }
            else if (_popupClone.isActiveAndEnabled && "Bio".Equals(_popupClone.Tabs.SelectedTab.Name)
                                                    && selectedAspect.Selected.ValueRO.UpdateTraitsUI){
                selectedAspect.Selected.ValueRW.UpdateTraitsUI = false;
                UpdateGeneralBio(e);
            }
            else if (_popupClone.isActiveAndEnabled && "Interest".Equals(_popupClone.Tabs.SelectedTab.Name)
                                                    && selectedAspect.Selected.ValueRO.UpdateInterestUI){
                selectedAspect.Selected.ValueRW.UpdateInterestUI = false;
                UpdateInterest(e);
            }
            else if (_popupClone.isActiveAndEnabled && "Skills".Equals(_popupClone.Tabs.SelectedTab.Name)
                                                    && selectedAspect.Selected.ValueRO.UpdateSkillsUI){
                selectedAspect.Selected.ValueRW.UpdateSkillsUI = false;
                UpdateSkills(e);
            }
            else if (_popupClone.isActiveAndEnabled && "Relationships".Equals(_popupClone.Tabs.SelectedTab.Name)
                                                    && selectedAspect.Selected.ValueRO.UpdateRelationshipsUI){
                selectedAspect.Selected.ValueRW.UpdateRelationshipsUI = false;
                UpdateRelationships(e);
            }
            else if (_popupClone.isActiveAndEnabled && "Academics".Equals(_popupClone.Tabs.SelectedTab.Name)
                                                    && selectedAspect.Selected.ValueRO.updateAcademicsUI){
                selectedAspect.Selected.ValueRW.UpdateTraitsUI = false;
                UpdateAcademics(e);
            }
        }
    }

    private void UpdatePopup(Entity e){
        switch (_popupClone.Tabs.SelectedTab.Name){
            case "Bio":
                UpdateGeneralBio(e);
                break;
            case "Interest":
                UpdateInterest(e);
                break;
            case "Skills":
                UpdateSkills(e);
                break;
            case "Relationships":
                UpdateRelationships(e);
                break;
            case "Academics":
                UpdateAcademics(e);
                break;
        }
    }

    private void UpdateSkills(Entity e){
        _popupClone.SkillsListView.DataSource.Clear();
        _popupClone.SkillsListView.DataSource.EndUpdate();
        var skills = SystemAPI.GetBuffer<SkillElement>(e);
        var traits = SystemAPI.GetBuffer<TraitElement>(e);
        var wellness = SystemAPI.GetBuffer<WellnessElement>(e);
        var passives = SystemAPI.GetBuffer<PassiveEffectElement>(e);
        var characterAttributes = SystemAPI.GetBuffer<CharacterAttributeElement>(e);

        var skillUtils = new SkillUtils(){
            SkillDataStore = SystemAPI.GetSingleton<SkillDataStore>()
        };

        var passivesUtils = new PassiveEffectsUtils(){
            CharacterAttributes = characterAttributes,
            Traits = traits,
            CharacterWellness = wellness,
            PassiveCompLookup = SystemAPI.GetComponentLookup<PassiveEffectComponent>(),
            Passives = passives,
            SkillUtils = skillUtils
        };

        foreach (var skill in skills){
            var element = new SkillElementUI(){
                skillLevel = skill.CurrentLevel,
                SkillType = skill.SkillType,
                PrimaryAttribute = skillUtils.GetSkillPrimaryAttribute(skill.SkillType),
                SecondaryAttribute = skillUtils.GetSkillSecondaryAttribute(skill.SkillType),
                xpRatio = skillUtils.GetXpRatio(skill),

                powerAttribute = skillUtils.GetSkillPrimaryAttribute(skill.SkillType),
                powerValue = passivesUtils.GetSkillPower(skill.SkillType)
            };
            _popupClone.SkillsListView.DataSource.Add(element);
        }

        _popupClone.SkillsListView.DataSource.BeginUpdate();
    }

    private void UpdateInterest(Entity e){
        _popupClone.InterestListView.DataSource.Clear();
        _popupClone.InterestListView.DataSource.EndUpdate();
        var interest = SystemAPI.GetBuffer<InterestElement>(e);
        var dataStore = SystemAPI.GetSingleton<InterestDataStore>();
        foreach (var i in interest){
            var effectsString = dataStore.InterestBlobAssets.Value.GetInterestEffectsString(i.SubjectType, i.EnumValue);
            var element = new InterestElementUI(){
                SubjectType = i.SubjectType,
                InterestType = i.EnumValue,
                Ratio = i.Ratio,
                InterestEffects = effectsString
            };
            _popupClone.InterestListView.DataSource.Add(element);
        }

        _popupClone.InterestListView.DataSource.BeginUpdate();
    }

    private void UpdateGeneralBio(Entity e){
        _stringBuilder1.Clear();
        _stringBuilder2.Clear();
        _stringBuilder3.Clear();

        var traitDataStore = SystemAPI.GetSingleton<TraitDataStore>();
        var traitElements = SystemAPI.GetBuffer<TraitElement>(e);
        var bio = SystemAPI.GetComponent<CharacterBio>(e);

        _popupClone.NameTitleText.text =
            bio.FullName + "\n" + (bio.IsFemale ? "Female" : "Male") + "\n";

        if (SystemAPI.HasComponent<StudentBio>(e)){
            _popupClone.NameTitleText.text += SystemAPI.GetComponent<StudentBio>(e).Seniority;
        }

        _popupClone.InfoText.text = "My story has yet to be written!";

        for (var i = 0; i < traitElements.Length; i++){
            var traitElement = traitElements[i];

            if (traitElement.Intensity == 0){
                continue;
            }

            if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType, TraitCategory.DEFAULT)
                || traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType, TraitCategory.GENETIC)){
                _stringBuilder1.Append(StringUtils.GetTraitTypeString(traitElement.TraitType));
                if (traitElement.Intensity > 1){
                    _stringBuilder1.Append(" " + traitElement.Intensity + " ");
                }
                else{
                    _stringBuilder1.Append(", ");
                }
            }
            else if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType,
                         TraitCategory.PERSONALITY)){
                _stringBuilder2.Append(StringUtils.GetTraitTypeString(traitElement.TraitType));
                if (traitElement.Intensity > 1){
                    _stringBuilder2.Append(" " + traitElement.Intensity + " ");
                }
                else{
                    _stringBuilder2.Append(", ");
                }
            }
            else if (traitDataStore.TraitBlobAssets.Value.IsTraitCategory(traitElement.TraitType, TraitCategory.STATUS)
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

    private void UpdateAcademics(Entity e){
        _popupClone.ClassPeriodListView.DataSource.Clear();
        _popupClone.ClassPeriodListView.DataSource.EndUpdate();
        // todo check we are looking at student
        var academics = SystemAPI.GetComponent<StudentAcademicComponent>(e);

        _stringBuilder1.Clear();
        _stringBuilder1.Append("Locker: ");
        _stringBuilder1.Append(SystemAPI.GetComponent<InteractableLocationComponent>(academics.LockerEntity).Alias);
        _stringBuilder1.Append("\nDorm: ");
        var room = SystemAPI.GetComponent<BedComponent>(academics.BedEntity).RoomEntity;
        _stringBuilder1.Append(SystemAPI.GetComponent<RoomComponent>(room).RoomName);

        _popupClone.AcademicsText.text = _stringBuilder1.ToString();

        var periods = SystemAPI.GetBuffer<ClassroomPeriodElement>(academics.ClassEntity1);
        var item = new ClassPeriodElementUI();
        item.Period = 1;
        item.SkillType = periods[0].Subject;
        item.Grade = "A+ (99% Fix ME)";
        item.AssignmentsTotal = "Assignments: 299/300";
        
        _popupClone.ClassPeriodListView.DataSource.Add(item);
        _popupClone.ClassPeriodListView.DataSource.BeginUpdate();
    }

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
}