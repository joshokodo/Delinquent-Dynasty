using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class TrendingPopupSystem : SystemBase {
    private TrendingPopup _popupClone;
    private bool _setPop;
    private bool _isOpen;
    private StringBuilder _stringBuilder = new();
    private EntityQuery _charactersQuery;

    protected override void OnStartRunning(){
        _charactersQuery = EntityManager.CreateEntityQuery(CommonSystemUtils.BuildHasAllQuery<CharacterBio>());
    }

    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<TrendingPopupComponent>()){
                _popupClone = (TrendingPopup) pop.Popup.Clone();
            }

            _setPop = true;
        }

        foreach (var (trendingComponent, trendingElements, entity) in SystemAPI
                     .Query<TrendingComponent, DynamicBuffer<TrendingElement>>().WithEntityAccess()){
            var component = trendingComponent;
            var showUi = component.ShowTrendingUI;

            if (showUi && !_popupClone.isActiveAndEnabled){
                _popupClone.Show();
                UpdateTrending(trendingElements);
            }
            else if (!showUi && _popupClone.isActiveAndEnabled){
                _popupClone.Close();
            }

            if (_popupClone.isActiveAndEnabled && component.UpdateTrendingUI){
                component.UpdateTrendingUI = false;
                EntityManager.SetComponentData(entity, component);
                UpdateTrending(trendingElements);
            }
            else if (_popupClone.isActiveAndEnabled && _popupClone.FilterSelected){
                var isCharacters = !_popupClone.AllSelected && _popupClone.SelectedFilter == GameEnumType.NONE;
                UpdateTrending(trendingElements, _popupClone.AllSelected, isCharacters, _popupClone.SelectedFilter);

                _popupClone.FilterSelected = false;
                _popupClone.AllSelected = false;
                _popupClone.SelectedFilter = GameEnumType.NONE;
            }
        }
    }

    private void UpdateTrending(DynamicBuffer<TrendingElement> trending, bool isAll = true, bool isCharacters = false,
        GameEnumType gameEnumType = GameEnumType.NONE){
        _popupClone.TrendingListView.DataSource.Clear();
        _popupClone.TrendingListView.DataSource.EndUpdate();
        var trendingUtils = new TrendingUtils();
        NativeArray<TrendingElement> trendingArr;
        if (isAll){
            trendingArr = trendingUtils.GetAllSorted(trending);
        }
        else if (isCharacters){
            trendingArr = trendingUtils.GetSorted(trending, true, _charactersQuery.CalculateEntityCount());
        }
        else if (gameEnumType != GameEnumType.NONE){
            trendingArr = trendingUtils.GetSorted(trending, type: gameEnumType);
        }
        else{
            trendingArr = default;
        }

        for (var i = 0; i < trendingArr.Length; i++){
            var trend = trendingArr[i];
            var element = new TrendingElementUI();
            _stringBuilder.Clear();
            _stringBuilder.Append(i + 1 + ". ");
            if (trend.Entity != Entity.Null){
                _stringBuilder.Append("(Character) ");
                _stringBuilder.Append(SystemAPI.GetComponent<CharacterBio>(trend.Entity).FullName);
            }
            else{
                switch (trend.EnumValue.Type){
                    case GameEnumType.CLOTHING_ITEM_CATEGORY:
                        _stringBuilder.Append("(Item) ");
                        _stringBuilder.Append(trend.EnumValue.ClothingItemCategory);
                        break;
                    case GameEnumType.SKILL_TYPE:
                        _stringBuilder.Append("(Skill) ");
                        _stringBuilder.Append(trend.EnumValue.SkillType);
                        break;
                    case GameEnumType.ATTRIBUTE_TYPE:
                        _stringBuilder.Append("(Attribute) ");
                        _stringBuilder.Append(trend.EnumValue.AttributeType);
                        break;
                    case GameEnumType.WELLNESS_TYPE:
                        _stringBuilder.Append("(Wellness) ");
                        _stringBuilder.Append(trend.EnumValue.WellnessType);
                        break;
                    case GameEnumType.GENERAL_INTEREST_TYPE:
                        _stringBuilder.Append("(Interest) ");
                        _stringBuilder.Append(trend.EnumValue.GeneralInterestType);
                        break;
                    case GameEnumType.DEFAULT_TRAIT_CATEGORY:
                        _stringBuilder.Append("(Trait) ");
                        _stringBuilder.Append(trend.EnumValue.DefaultTraitCategory);
                        break;
                    default:
                        _stringBuilder.Append("(N/A) Create case for " + trend.EnumValue.Type);
                        break;
                }
            }

            element.Title = new FixedString512Bytes(_stringBuilder.ToString());
            element.Value = new FixedString128Bytes(trend.TrendingData.TrendingValue.ToString());
            _popupClone.TrendingListView.DataSource.Add(element);
        }

        _popupClone.TrendingListView.DataSource.BeginUpdate();
    }
}