using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActionDataStoreAuthoring : MonoBehaviour { }

public class ActionDataStoreBaker : Baker<ActionDataStoreAuthoring> {
    public override void Bake(ActionDataStoreAuthoring authoring){
        var store = new ActionDataStore();
        store.ActionsBlobAssets = InitializeActionsBlobAssets();
        AddComponent(store);
        Debug.Log("Cap for 4096 fixed list targets " + new FixedList4096Bytes<ActiveActionTargetElement>().Capacity);
    }

    private BlobAssetReference<ActionsBlobAssets> InitializeActionsBlobAssets(){
        var data = CommonUtils.GetScriptableObjectData<ActionBaseDataSO>("Scriptable Objects/Actions");

        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var actionBlobBuilder = ref blobBuilder.ConstructRoot<ActionsBlobAssets>();

        var baseList = new List<ActionBaseAssetData>();

        var successEffectList = new List<ActionActiveEffectAssetData>();
        var failEffectList = new List<ActionActiveEffectAssetData>();
        var passiveEffectList = new List<ActionPassiveEffectAssetData>();

        for (var i = 0; i < data.Count; i++){
            var next = data[i];
            var baseId = Guid.NewGuid();

            foreach (var nextSuccessActiveEffect in next.successEffects){
                var conds = new FixedList4096Bytes<ConditionData>();
                var effects = new FixedList512Bytes<ActiveEffectData>();

                foreach (var e in nextSuccessActiveEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in nextSuccessActiveEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                successEffectList.Add(new ActionActiveEffectAssetData(){
                    EffectAssetData = new ActiveEffectAssetData(){
                        ParentId = baseId,
                        Effects = effects,
                        AnyCondition = nextSuccessActiveEffect.anyCondition,
                        Conditions = conds,
                    },
                    SkillLevelRequirement = nextSuccessActiveEffect.skillLevelRequirement,
                    SkillLevelComparisonSign = nextSuccessActiveEffect.skillNumericComparison
                });
            }

            foreach (var nextfailActiveEffect in next.failEffects){
                var effects = new FixedList512Bytes<ActiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var e in nextfailActiveEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in nextfailActiveEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                failEffectList.Add(new ActionActiveEffectAssetData(){
                    EffectAssetData = new ActiveEffectAssetData(){
                        ParentId = baseId,
                        Effects = effects,
                        Conditions = conds,
                        AnyCondition = nextfailActiveEffect.anyCondition,
                    },
                    SkillLevelRequirement = nextfailActiveEffect.skillLevelRequirement,
                    SkillLevelComparisonSign = nextfailActiveEffect.skillNumericComparison
                });
            }

            foreach (var nextPassiveActiveEffect in next.passiveEffects){
                var effects = new FixedList4096Bytes<PassiveEffectData>();
                var conds = new FixedList4096Bytes<ConditionData>();

                foreach (var e in nextPassiveActiveEffect.effects){
                    effects.Add(e.ToData());
                }

                foreach (var c in nextPassiveActiveEffect.conditions){
                    conds.Add(c.ToConditionData());
                }

                var title = nextPassiveActiveEffect.passiveTitle == null
                    ? default
                    : new FixedString64Bytes(nextPassiveActiveEffect.passiveTitle);

                passiveEffectList.Add(new ActionPassiveEffectAssetData(){
                    PassiveEffectAssetData = new PassiveEffectAssetData(){
                        ParentId = baseId,
                        Id = Guid.NewGuid(),
                        Effects = effects,
                        Conditions = conds,
                        AnyCondition = nextPassiveActiveEffect.anyCondition,
                    },
                    PassiveTitle = title,
                    SkillLevelRequirement = nextPassiveActiveEffect.skillLevelRequirement,
                    SkillLevelComparisonSign = nextPassiveActiveEffect.skillLevelComparisonSign
                });
            }

            var functions = new FixedList64Bytes<ActionFunctionType>();
            next.functions.ForEach(f => functions.Add(f));
            var targets = new FixedList32Bytes<TargetType>();
            next.targetTypes.ForEach(t => targets.Add(t));

            var prereqs = new FixedList4096Bytes<ConditionData>();
            foreach (var nextPrerequisite in next.prerequisites){
                prereqs.Add(nextPrerequisite.ToConditionData());
            }

            Debug.Log("ACTION PREREQS SIZE " + prereqs.Capacity);

            baseList.Add(new ActionBaseAssetData(){
                Id = baseId,
                ActionType = next.DynamicActionType.ToData(),
                SkillUsed = next.skillUsed,
                UsesLocomotion = next.usesLocomotion,
                UsesInfluence = next.usesInfluence,
                QuickActionCategory = next.quickActionCategory,
                ActionTitleString = new FixedString64Bytes(next.actionTitleString),
                PerformingActionString = new FixedString64Bytes(next.performingActionString),
                Functions = functions,
                Prerequisites = prereqs,
                GenericDescription = new FixedString64Bytes(next.genericDescription),
                TargetTypes = targets,
                FocusCost = next.focusCost,
                EnergyCost = next.energyCost,
                DifficultyLevel = next.difficultyLevel,
                PerformTime = new TimeSpan(next.performHours, next.performMins, next.performSecs).TotalSeconds,
                HideKnowledgeFromTargets = next.hideKnowledgeFromTargets,
                MaxAreaOfEffectRange = next.MaxAreaOfEffectRange
            });
        }

        var baseArr = blobBuilder.Allocate(ref actionBlobBuilder.ActionBaseAssets, baseList.Count);
        var successEffArr =
            blobBuilder.Allocate(ref actionBlobBuilder.SuccessfulEffectsAssets, successEffectList.Count);
        var failEffArr = blobBuilder.Allocate(ref actionBlobBuilder.FailEffectsAssets, failEffectList.Count);
        var passiveArr = blobBuilder.Allocate(ref actionBlobBuilder.PassiveEffectsAssets, passiveEffectList.Count);

        for (var i = 0; i < baseList.Count; i++){
            baseArr[i] = baseList[i];
        }

        for (var i = 0; i < successEffectList.Count; i++){
            successEffArr[i] = successEffectList[i];
        }

        for (var i = 0; i < passiveEffectList.Count; i++){
            passiveArr[i] = passiveEffectList[i];
        }

        for (var i = 0; i < failEffectList.Count; i++){
            failEffArr[i] = failEffectList[i];
        }


        return blobBuilder.CreateBlobAssetReference<ActionsBlobAssets>(Allocator.Persistent);
    }
}