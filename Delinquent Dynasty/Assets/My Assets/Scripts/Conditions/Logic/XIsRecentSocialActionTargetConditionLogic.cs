using Unity.Entities;
using UnityEngine;

public struct XIsRecentSocialActionTargetConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);

        if (target == Entity.Null || !utils.CharacterBioLookup.HasComponent(target)){
            return !conditionData.ExpectedConditionValue;
        }

        var visibility = utils.VisibilityLookup[target];
        var knowledge = utils.DailyEventsLookup[target];
        var isTargetOfAction = false;

        foreach (var visibleCharacterElement in visibility){
            var behaviorEntity = utils.CharacterBehaviorLookup[visibleCharacterElement.VisibleCharacter].BehaviorEntity;
            var actions = utils.ActiveActionsLookup[behaviorEntity];

            foreach (var act in actions){
                if (act.ActionType.IsSocialAction){
                    var targets = utils.ActiveActionTargetsLookup[behaviorEntity];
                    foreach (var targ in targets){
                        if (targ.ActionId == act.ActionId){
                            if (targ.Data.TargetType == TargetType.TARGET_CHARACTER &&
                                targ.Data.TargetEntity == target){
                                isTargetOfAction = true;
                                break;
                            }


                            if (targ.Data.TargetType == TargetType.TARGET_VISIBLE_CHARACTERS_IN_AREA){
                                var data =
                                    utils.ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(act.ActionType);
                                isTargetOfAction = visibleCharacterElement.Distance <=
                                                   data.MaxAreaOfEffectRange;
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            if (!isTargetOfAction && conditionData.PrimaryNumberValue > 0){
                foreach (var knowledgeElement in knowledge){
                    if (!utils.EventElementsLookup.TryGetBuffer(knowledgeElement.KnowledgeEntity,
                            out DynamicBuffer<EventKnowledgeElement> events)) continue;

                    foreach (var eventKnowledgeElement in events){
                        if (eventKnowledgeElement.PerformingEntity == visibleCharacterElement.VisibleCharacter
                            && eventKnowledgeElement.ActionType.IsSocialAction
                            && eventKnowledgeElement.ContainsTarget(target)
                            && eventKnowledgeElement.ActionTimestamp.MatchesSeasonAndYear(utils.InGameTime)){
                            var difference = utils.InGameTime.TotalInGameSeconds -
                                             eventKnowledgeElement.ActionTimestamp.TotalInGameSeconds;
                            if (NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign,
                                    (int) difference, (int) conditionData.PrimaryNumberValue)){
                                isTargetOfAction = true;
                                break;
                            }
                        }
                    }

                    if (isTargetOfAction){
                        break;
                    }
                }
            }

            if (isTargetOfAction){
                break;
            }
        }

        return isTargetOfAction == conditionData.ExpectedConditionValue;
    }
}