using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class NPCDataStoreAuthoring : MonoBehaviour { }

public class NPCDataStoreBaker : Baker<NPCDataStoreAuthoring> {
    public override void Bake(NPCDataStoreAuthoring authoring){
        var store = new NPCDataStore();
        store.NpcAssets = InitializeNPCBlobAssets();
        AddComponent(store);
    }

    public BlobAssetReference<NPCBlobAssets> InitializeNPCBlobAssets(){
        //TODO: if fixed list will provide enough room for scenario conditions, remove conditions asset blob and just add fixed list inside scenario asset data

        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var npcBlobBuilder = ref blobBuilder.ConstructRoot<NPCBlobAssets>();
        using MD5 md5 = MD5.Create();
        
        // time based
        var timeBasedscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Time Based");
        var timeBaseScenariosArr =
            blobBuilder.Allocate(ref npcBlobBuilder.TimeBasedScenarioAssets, timeBasedscenarioData.Count);
        var timeBaseTargetsList = SetScenarioList(timeBasedscenarioData, timeBaseScenariosArr, md5);
        var timeBasedTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.TimeBaseScenarioConditionTargetAssets,
            timeBaseTargetsList.Count);
        SetBlobArray(timeBaseTargetsList, timeBasedTargCondArr);

        // loc based
        var locBasedscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Location Based");
        var locBaseScenariosArr =
            blobBuilder.Allocate(ref npcBlobBuilder.LocationBasedScenarioAssets, locBasedscenarioData.Count);
        var locBaseTargCondData = SetScenarioList(locBasedscenarioData, locBaseScenariosArr, md5);
        var locBasedTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.LocationBaseScenarioConditionTargetAssets,
            locBaseTargCondData.Count);
        SetBlobArray(locBaseTargCondData, locBasedTargCondArr);

        // wellness
        var wellnessStatusscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Wellness Status");
        var wellnessStatusScenariosArr = blobBuilder.Allocate(ref npcBlobBuilder.WellnessStatusScenarioAssets,
            wellnessStatusscenarioData.Count);
        var wellnessStatusTargCondData = SetScenarioList(wellnessStatusscenarioData, wellnessStatusScenariosArr, md5);
        var wellnessStatusTargCondArr =
            blobBuilder.Allocate(ref npcBlobBuilder.WellnessStatusScenarioConditionTargetAssets,
                wellnessStatusTargCondData.Count);
        SetBlobArray(wellnessStatusTargCondData, wellnessStatusTargCondArr);

        // academic
        var academicStatusscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Academic Status");
        var academicStatusScenariosArr = blobBuilder.Allocate(ref npcBlobBuilder.AcademicStatusScenarioAssets,
            academicStatusscenarioData.Count);
        var academicStatusTargCondData = SetScenarioList(academicStatusscenarioData, academicStatusScenariosArr, md5);
        var academicStatusTargCondArr =
            blobBuilder.Allocate(ref npcBlobBuilder.AcademicStatusScenarioConditionTargetAssets,
                academicStatusTargCondData.Count);
        SetBlobArray(academicStatusTargCondData, academicStatusTargCondArr);

        // attribute
        var attributeStatusscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Attribute Status");
        var attributeStatusScenariosArr = blobBuilder.Allocate(ref npcBlobBuilder.AttributeStatusScenarioAssets,
            attributeStatusscenarioData.Count);
        var attributeStatusTargCondData = SetScenarioList(attributeStatusscenarioData, attributeStatusScenariosArr, md5);
        var attributeStatusTargCondArr =
            blobBuilder.Allocate(ref npcBlobBuilder.AttributeStatusScenarioConditionTargetAssets,
                attributeStatusTargCondData.Count);
        SetBlobArray(attributeStatusTargCondData, attributeStatusTargCondArr);

        // skills
        var skillStatusscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Skills Status");
        var skillStatusScenariosArr =
            blobBuilder.Allocate(ref npcBlobBuilder.SkillsStatusScenarioAssets, skillStatusscenarioData.Count);
        var skillStatusTargCondData = SetScenarioList(skillStatusscenarioData, skillStatusScenariosArr, md5);
        var skillStatusTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.SkillsStatusScenarioConditionTargetAssets,
            skillStatusTargCondData.Count);
        SetBlobArray(skillStatusTargCondData, skillStatusTargCondArr);

        // interest
        var interestStatusscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Interest Status");
        var interestStatusScenariosArr = blobBuilder.Allocate(ref npcBlobBuilder.InterestStatusScenarioAssets,
            interestStatusscenarioData.Count);
        var interestStatusTargCondData = SetScenarioList(interestStatusscenarioData, interestStatusScenariosArr, md5);
        var interestStatusTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.InterestStatusConditionTargetAssets,
            interestStatusTargCondData.Count);
        SetBlobArray(interestStatusTargCondData, interestStatusTargCondArr);

        // character bio
        var characterBioscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/Character Bio");
        var characterBioScenariosArr =
            blobBuilder.Allocate(ref npcBlobBuilder.CharacterBioScenarioAssets, characterBioscenarioData.Count);
        var characterBioTargCondData = SetScenarioList(characterBioscenarioData, characterBioScenariosArr, md5);
        var characterBioTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.CharacterBioConditionTargetAssets,
            characterBioTargCondData.Count);
        SetBlobArray(characterBioTargCondData, characterBioTargCondArr);

        // External Events
        var externalEventsscenarioData =
            CommonUtils.GetScriptableObjectData<NpcScenarioDataSO>("Scriptable Objects/Npc/Scenarios/External Events");
        var externalEventsScenariosArr = blobBuilder.Allocate(ref npcBlobBuilder.ExternalEventsScenarioAssets,
            externalEventsscenarioData.Count);
        var externalEventsTargCondData = SetScenarioList(externalEventsscenarioData, externalEventsScenariosArr, md5);
        var externalEventsTargCondArr = blobBuilder.Allocate(ref npcBlobBuilder.ExternalEventsConditionTargetAssets,
            externalEventsTargCondData.Count);
        SetBlobArray(externalEventsTargCondData, externalEventsTargCondArr);

        // critical
        var criticalResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/Critical");
        GetResponseList(criticalResponseData, out List<NpcBaseResponseAssetData> criticalBaseData,
            out List<ActionAssetData> criticalRespActData, out List<GenericTargetAssetData> criticalRespTargData,
            out List<ConditionsAssetData> criticalRespCondData);

        var criticalResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.CriticalResponseAssets, criticalBaseData.Count);
        var criticalActResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.CriticalActionAssets, criticalRespActData.Count);
        var criticalTargResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.CriticalActionTargetAssets, criticalRespTargData.Count);
        var CriticalActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.CriticalActionConditionAssets, criticalRespCondData.Count);

        SetBlobArray(criticalBaseData, criticalResponseArr);
        SetBlobArray(criticalRespCondData, CriticalActCondResponseArr);
        SetBlobArray(criticalRespActData, criticalActResponseArr);
        SetBlobArray(criticalRespTargData, criticalTargResponseArr);

        // very high
        var veryHighResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/Very High");

        GetResponseList(veryHighResponseData, out List<NpcBaseResponseAssetData> veryHighBaseData,
            out List<ActionAssetData> veryHighRespActData, out List<GenericTargetAssetData> veryHighRespTargData,
            out List<ConditionsAssetData> veryHighRespCondData);

        var veryHighResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryHighResponseAssets, veryHighBaseData.Count);
        var veryHighActResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryHighActionAssets, veryHighRespActData.Count);
        var veryHighTargResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryHighActionTargetAssets, veryHighRespTargData.Count);
        var veryHighActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryHighActionConditionAssets, veryHighRespCondData.Count);

        SetBlobArray(veryHighBaseData, veryHighResponseArr);
        SetBlobArray(veryHighRespCondData, veryHighActCondResponseArr);
        SetBlobArray(veryHighRespActData, veryHighActResponseArr);
        SetBlobArray(veryHighRespTargData, veryHighTargResponseArr);

        // high
        var highResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/High");

        GetResponseList(highResponseData, out List<NpcBaseResponseAssetData> highBaseData,
            out List<ActionAssetData> highRespActData, out List<GenericTargetAssetData> highRespTargData,
            out List<ConditionsAssetData> highRespCondData);

        var highResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.HighResponseAssets, highBaseData.Count);
        var highActResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.HighActionAssets, highRespActData.Count);
        var highTargResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.HighActionTargetAssets, highRespTargData.Count);
        var highActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.HighActionConditionAssets, highRespCondData.Count);

        SetBlobArray(highBaseData, highResponseArr);
        SetBlobArray(highRespCondData, highActCondResponseArr);
        SetBlobArray(highRespActData, highActResponseArr);
        SetBlobArray(highRespTargData, highTargResponseArr);

        // mid
        var midResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/Mid");

        GetResponseList(midResponseData, out List<NpcBaseResponseAssetData> midBaseData,
            out List<ActionAssetData> midRespActData, out List<GenericTargetAssetData> midRespTargData,
            out List<ConditionsAssetData> midRespCondData);

        var midResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.MidResponseAssets, midBaseData.Count);
        var midActResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.MidActionAssets, midRespActData.Count);
        var midTargResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.MidActionTargetAssets, midRespTargData.Count);
        var midActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.MidActionConditionAssets, midRespCondData.Count);

        SetBlobArray(midBaseData, midResponseArr);
        SetBlobArray(midRespCondData, midActCondResponseArr);
        SetBlobArray(midRespActData, midActResponseArr);
        SetBlobArray(midRespTargData, midTargResponseArr);

        // low
        var lowResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/Low");

        GetResponseList(lowResponseData, out List<NpcBaseResponseAssetData> lowBaseData,
            out List<ActionAssetData> lowRespActData, out List<GenericTargetAssetData> lowRespTargData,
            out List<ConditionsAssetData> lowRespCondData);

        var lowResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.LowResponseAssets, lowBaseData.Count);
        var lowActResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.LowActionAssets, lowRespActData.Count);
        var lowTargResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.LowActionTargetAssets, lowRespTargData.Count);
        var lowActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.LowActionConditionAssets, lowRespCondData.Count);

        // very low
        var veryLowResponseData =
            CommonUtils.GetScriptableObjectData<NpcResponseDataSO>("Scriptable Objects/Npc/Responses/Very Low");

        GetResponseList(veryLowResponseData, out List<NpcBaseResponseAssetData> veryLowBaseData,
            out List<ActionAssetData> veryLowRespActData, out List<GenericTargetAssetData> veryLowRespTargData,
            out List<ConditionsAssetData> veryLowRespCondData);

        var veryLowResponseArr = blobBuilder.Allocate(ref npcBlobBuilder.VeryLowResponseAssets, veryLowBaseData.Count);
        var veryLowActResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryLowActionAssets, veryLowRespActData.Count);
        var veryLowTargResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryLowActionTargetAssets, veryLowRespTargData.Count);
        var veryLowActCondResponseArr =
            blobBuilder.Allocate(ref npcBlobBuilder.VeryLowActionConditionAssets, veryLowRespCondData.Count);

        SetBlobArray(veryLowBaseData, veryLowResponseArr);
        SetBlobArray(veryLowRespCondData, veryLowActCondResponseArr);
        SetBlobArray(veryLowRespActData, veryLowActResponseArr);
        SetBlobArray(veryLowRespTargData, veryLowTargResponseArr);

        return blobBuilder.CreateBlobAssetReference<NPCBlobAssets>(Allocator.Persistent);
    }

    public List<GenericTargetAssetData> SetScenarioList(List<NpcScenarioDataSO> scenarioData,
        BlobBuilderArray<NpcScenarioAssetData> scenariosArr, MD5 md5){
        var targets = new List<GenericTargetAssetData>();
        for (int i = 0; i < scenarioData.Count; i++){
            var data = scenarioData[i];
            var id = Guid.NewGuid();
            var conditions = new FixedList4096Bytes<ConditionData>();
            data.conditions.ForEach(c => conditions.Add(c.ToConditionData()));

            scenariosArr[i] = new NpcScenarioAssetData(){
                Id = id,
                GroupId = string.IsNullOrEmpty(scenarioData[i].groupId) ? Guid.Empty : new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(scenarioData[i].groupId))),
                ScenarioType = scenarioData[i].scenarioType.ToData(),
                AnyCondition = scenarioData[i].anyCondition,
                Conditions = conditions
            };

            foreach (var dataConditionTarget in data.conditionTargets){
                conditions.Clear();
                dataConditionTarget.genericTargetConditions.ForEach(c => conditions.Add(c.ToConditionData()));
                targets.Add(new GenericTargetAssetData(){
                    ParentId = id,
                    Data = dataConditionTarget.ToData(),
                    AnyCondition = dataConditionTarget.anyCondition,
                    Conditions = conditions
                });
            }
        }

        return targets;
    }

    public void GetResponseList(List<NpcResponseDataSO> responseData, out List<NpcBaseResponseAssetData> baseData,
        out List<ActionAssetData> respActData, out List<GenericTargetAssetData> respTargData,
        out List<ConditionsAssetData> respCondData){
        baseData = new List<NpcBaseResponseAssetData>();
        respActData = new List<ActionAssetData>();
        respTargData = new List<GenericTargetAssetData>();
        respCondData = new List<ConditionsAssetData>();

        for (int i = 0; i < responseData.Count; i++){
            var nextResponse = responseData[i];
            var respId = Guid.NewGuid();

            var scenarios = new FixedList4096Bytes<DynamicNPCScenarioType>();
            var goals = new FixedList4096Bytes<NpcGoalAssetData>();
            nextResponse.scenarioTypes.ForEach(s => scenarios.Add(s.ToData()));
            nextResponse.goalTypes.ForEach(g => goals.Add(g.ToData()));

            baseData.Add(new NpcBaseResponseAssetData(){
                Priority = nextResponse.priority,
                Severity = nextResponse.severity,
                ResponseId = respId,
                ScenarioTypes = scenarios,
                GoalTypes = goals,
                PersonalityType = nextResponse.personalityType
            });


            foreach (var act in nextResponse.actions){
                var actId = Guid.NewGuid();
                var conditions = new FixedList4096Bytes<ConditionData>();

                respActData.Add(new ActionAssetData(){
                    ParentId = respId,
                    ActionId = actId,
                    ActionType = act.DynamicActionType.ToData(),
                    Iterations = act.iterations,
                    AnyCondition = act.anyCondition,
                    
                });

                for (var j = 0; j < act.targets.Count; j++){
                    var t = act.targets[j];
                    var targ = t.ToAssetData();
                    targ.ParentId = actId;
                    targ.Id = Guid.NewGuid();
                    respTargData.Add(targ);
                }

                act.actionConditions.ForEach(c => { conditions.Add(c.ToConditionData()); });
                respCondData.Add(new ConditionsAssetData(){
                    Conditions = conditions,
                    ParentId = actId
                });
            }
        }
    }

    public void SetBlobArray<T>(List<T> scenarioCondData, BlobBuilderArray<T> scenariosCondArr) where T : struct{
        for (var i = 0; i < scenarioCondData.Count; i++){
            scenariosCondArr[i] = scenarioCondData[i];
        }
    }
}