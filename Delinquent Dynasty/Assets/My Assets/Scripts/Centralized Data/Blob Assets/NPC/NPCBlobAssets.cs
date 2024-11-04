using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct NPCBlobAssets {
    public BlobArray<NpcScenarioAssetData> TimeBasedScenarioAssets;
    public BlobArray<GenericTargetAssetData> TimeBaseScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> LocationBasedScenarioAssets;
    public BlobArray<GenericTargetAssetData> LocationBaseScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> WellnessStatusScenarioAssets;
    public BlobArray<GenericTargetAssetData> WellnessStatusScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> AcademicStatusScenarioAssets;
    public BlobArray<GenericTargetAssetData> AcademicStatusScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> AttributeStatusScenarioAssets;
    public BlobArray<GenericTargetAssetData> AttributeStatusScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> SkillsStatusScenarioAssets;
    public BlobArray<GenericTargetAssetData> SkillsStatusScenarioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> InterestStatusScenarioAssets;
    public BlobArray<GenericTargetAssetData> InterestStatusConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> CharacterBioScenarioAssets;
    public BlobArray<GenericTargetAssetData> CharacterBioConditionTargetAssets;

    public BlobArray<NpcScenarioAssetData> ExternalEventsScenarioAssets;
    public BlobArray<GenericTargetAssetData> ExternalEventsConditionTargetAssets;

    public BlobArray<NpcBaseResponseAssetData> CriticalResponseAssets;
    public BlobArray<ActionAssetData> CriticalActionAssets;
    public BlobArray<GenericTargetAssetData> CriticalActionTargetAssets;
    public BlobArray<ConditionsAssetData> CriticalActionConditionAssets;

    public BlobArray<NpcBaseResponseAssetData> VeryHighResponseAssets;
    public BlobArray<ActionAssetData> VeryHighActionAssets;
    public BlobArray<GenericTargetAssetData> VeryHighActionTargetAssets;
    public BlobArray<ConditionsAssetData> VeryHighActionConditionAssets;

    public BlobArray<NpcBaseResponseAssetData> HighResponseAssets;
    public BlobArray<ActionAssetData> HighActionAssets;
    public BlobArray<GenericTargetAssetData> HighActionTargetAssets;
    public BlobArray<ConditionsAssetData> HighActionConditionAssets;

    public BlobArray<NpcBaseResponseAssetData> MidResponseAssets;
    public BlobArray<ActionAssetData> MidActionAssets;
    public BlobArray<GenericTargetAssetData> MidActionTargetAssets;
    public BlobArray<ConditionsAssetData> MidActionConditionAssets;

    public BlobArray<NpcBaseResponseAssetData> LowResponseAssets;
    public BlobArray<ActionAssetData> LowActionAssets;
    public BlobArray<GenericTargetAssetData> LowActionTargetAssets;
    public BlobArray<ConditionsAssetData> LowActionConditionAssets;

    public BlobArray<NpcBaseResponseAssetData> VeryLowResponseAssets;
    public BlobArray<ActionAssetData> VeryLowActionAssets;
    public BlobArray<GenericTargetAssetData> VeryLowActionTargetAssets;
    public BlobArray<ConditionsAssetData> VeryLowActionConditionAssets;

    public NativeArray<GenericTargetAssetData> GetScenarioGenericTargetsArray(NPCScenarioCategory category,
        Allocator allocator){
     
        switch (category){
            case NPCScenarioCategory.TIME_BASED:
                var a = new NativeArray<GenericTargetAssetData>(TimeBaseScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < TimeBaseScenarioConditionTargetAssets.Length; i++){
                    var data = TimeBaseScenarioConditionTargetAssets[i];
                    a[i] = data;
                }

                return a;
            case NPCScenarioCategory.LOCATION_BASED:
                var b = new NativeArray<GenericTargetAssetData>(LocationBaseScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < LocationBaseScenarioConditionTargetAssets.Length; i++){
                    var data = LocationBaseScenarioConditionTargetAssets[i];
                    b[i] = data;
                }

                return b;
            case NPCScenarioCategory.SKILLS_STATUS:
                var c = new NativeArray<GenericTargetAssetData>(SkillsStatusScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < SkillsStatusScenarioConditionTargetAssets.Length; i++){
                    var data = SkillsStatusScenarioConditionTargetAssets[i];
                    c[i] = data;
                }

                return c;
            case NPCScenarioCategory.ATTRIBUTE_STATUS:
                var d = new NativeArray<GenericTargetAssetData>(AttributeStatusScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < AttributeStatusScenarioConditionTargetAssets.Length; i++){
                    var data = AttributeStatusScenarioConditionTargetAssets[i];
                    d[i] = data;
                }

                return d;
            case NPCScenarioCategory.WELLNESS_STATUS:
                var e = new NativeArray<GenericTargetAssetData>(WellnessStatusScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < WellnessStatusScenarioConditionTargetAssets.Length; i++){
                    var data = WellnessStatusScenarioConditionTargetAssets[i];
                    e[i] = data;
                }

                return e;
            case NPCScenarioCategory.INTEREST_STATUS:
                var f = new NativeArray<GenericTargetAssetData>(InterestStatusConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < InterestStatusConditionTargetAssets.Length; i++){
                    var data = InterestStatusConditionTargetAssets[i];
                    f[i] = data;
                }

                return f;
            case NPCScenarioCategory.ACADEMIC_STATUS:
                var g = new NativeArray<GenericTargetAssetData>(AcademicStatusScenarioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < AcademicStatusScenarioConditionTargetAssets.Length; i++){
                    var data = AcademicStatusScenarioConditionTargetAssets[i];
                    g[i] = data;
                }

                return g;
            case NPCScenarioCategory.CHARACTER_BIO:
                var h = new NativeArray<GenericTargetAssetData>(CharacterBioConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < CharacterBioConditionTargetAssets.Length; i++){
                    var data = CharacterBioConditionTargetAssets[i];
                    h[i] = data;
                }

                return h;
            case NPCScenarioCategory.EXTERNAL_EVENTS:
                var j = new NativeArray<GenericTargetAssetData>(ExternalEventsConditionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < ExternalEventsConditionTargetAssets.Length; i++){
                    var data = ExternalEventsConditionTargetAssets[i];
                    j[i] = data;
                }

                return j;
        }

        return default;
    }

    public NativeArray<NpcScenarioAssetData> GetScenarioArray(NPCScenarioCategory category, Allocator allocator){
        switch (category){
            case NPCScenarioCategory.TIME_BASED:
                var a = new NativeArray<NpcScenarioAssetData>(TimeBasedScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < TimeBasedScenarioAssets.Length; i++){
                    var data = TimeBasedScenarioAssets[i];
                    a[i] = data;
                }

                return a;
            case NPCScenarioCategory.LOCATION_BASED:
                var b = new NativeArray<NpcScenarioAssetData>(LocationBasedScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < LocationBasedScenarioAssets.Length; i++){
                    var data = LocationBasedScenarioAssets[i];
                    b[i] = data;
                }

                return b;
            case NPCScenarioCategory.SKILLS_STATUS:
                var c = new NativeArray<NpcScenarioAssetData>(SkillsStatusScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < SkillsStatusScenarioAssets.Length; i++){
                    var data = SkillsStatusScenarioAssets[i];
                    c[i] = data;
                }

                return c;
            case NPCScenarioCategory.ATTRIBUTE_STATUS:
                var d = new NativeArray<NpcScenarioAssetData>(AttributeStatusScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < AttributeStatusScenarioAssets.Length; i++){
                    var data = AttributeStatusScenarioAssets[i];
                    d[i] = data;
                }

                return d;
            case NPCScenarioCategory.WELLNESS_STATUS:
                var e = new NativeArray<NpcScenarioAssetData>(WellnessStatusScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < WellnessStatusScenarioAssets.Length; i++){
                    var data = WellnessStatusScenarioAssets[i];
                    e[i] = data;
                }

                return e;
            case NPCScenarioCategory.INTEREST_STATUS:
                var f = new NativeArray<NpcScenarioAssetData>(InterestStatusScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < InterestStatusScenarioAssets.Length; i++){
                    var data = InterestStatusScenarioAssets[i];
                    f[i] = data;
                }

                return f;
            case NPCScenarioCategory.ACADEMIC_STATUS:
                var g = new NativeArray<NpcScenarioAssetData>(AcademicStatusScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < AcademicStatusScenarioAssets.Length; i++){
                    var data = AcademicStatusScenarioAssets[i];
                    g[i] = data;
                }

                return g;
            case NPCScenarioCategory.CHARACTER_BIO:
                var h = new NativeArray<NpcScenarioAssetData>(CharacterBioScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < CharacterBioScenarioAssets.Length; i++){
                    var data = CharacterBioScenarioAssets[i];
                    h[i] = data;
                }

                return h;
            case NPCScenarioCategory.EXTERNAL_EVENTS:
                var j = new NativeArray<NpcScenarioAssetData>(ExternalEventsScenarioAssets.Length,
                    allocator);
                for (int i = 0; i < ExternalEventsScenarioAssets.Length; i++){
                    var data = ExternalEventsScenarioAssets[i];
                    j[i] = data;
                }

                return j;
        }

        return default;
    }

    public NativeArray<ActionAssetData> GetResponseActionsArray(Severity category, Allocator allocator){
        switch (category){
            case Severity.CRITICAL:
                var a = new NativeArray<ActionAssetData>(CriticalActionAssets.Length,
                    allocator);
                for (int i = 0; i < CriticalActionAssets.Length; i++){
                    var data = CriticalActionAssets[i];
                    a[i] = data;
                }

                return a;
            case Severity.VERY_HIGH:
                var b = new NativeArray<ActionAssetData>(VeryHighActionAssets.Length,
                    allocator);
                for (int i = 0; i < VeryHighActionAssets.Length; i++){
                    var data = VeryHighActionAssets[i];
                    b[i] = data;
                }

                return b;
            case Severity.HIGH:
                var c = new NativeArray<ActionAssetData>(HighActionAssets.Length,
                    allocator);
                for (int i = 0; i < HighActionAssets.Length; i++){
                    var data = HighActionAssets[i];
                    c[i] = data;
                }

                return c;
            case Severity.MID:
                var d = new NativeArray<ActionAssetData>(MidActionAssets.Length,
                    allocator);
                for (int i = 0; i < MidActionAssets.Length; i++){
                    var data = MidActionAssets[i];
                    d[i] = data;
                }

                return d;
            case Severity.LOW:
                var e = new NativeArray<ActionAssetData>(LowActionAssets.Length,
                    allocator);
                for (int i = 0; i < LowActionAssets.Length; i++){
                    var data = LowActionAssets[i];
                    e[i] = data;
                }

                return e;
            case Severity.VERY_LOW:
                var f = new NativeArray<ActionAssetData>(VeryLowActionAssets.Length,
                    allocator);
                for (int i = 0; i < VeryLowActionAssets.Length; i++){
                    var data = VeryLowActionAssets[i];
                    f[i] = data;
                }

                return f;
        }

        return default;
    }

    public NativeArray<GenericTargetAssetData> GetResponseActionTargetsArray(Severity category, Allocator allocator){
        switch (category){
            case Severity.CRITICAL:
                var a = new NativeArray<GenericTargetAssetData>(CriticalActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < CriticalActionTargetAssets.Length; i++){
                    var data = CriticalActionTargetAssets[i];
                    a[i] = data;
                }

                return a;
            case Severity.VERY_HIGH:
                var b = new NativeArray<GenericTargetAssetData>(VeryHighActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < VeryHighActionTargetAssets.Length; i++){
                    var data = VeryHighActionTargetAssets[i];
                    b[i] = data;
                }

                return b;
            case Severity.HIGH:
                var c = new NativeArray<GenericTargetAssetData>(HighActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < HighActionTargetAssets.Length; i++){
                    var data = HighActionTargetAssets[i];
                    c[i] = data;
                }

                return c;
            case Severity.MID:
                var d = new NativeArray<GenericTargetAssetData>(MidActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < MidActionTargetAssets.Length; i++){
                    var data = MidActionTargetAssets[i];
                    d[i] = data;
                }

                return d;
            case Severity.LOW:
                var e = new NativeArray<GenericTargetAssetData>(LowActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < LowActionTargetAssets.Length; i++){
                    var data = LowActionTargetAssets[i];
                    e[i] = data;
                }

                return e;
            case Severity.VERY_LOW:
                var f = new NativeArray<GenericTargetAssetData>(VeryLowActionTargetAssets.Length,
                    allocator);
                for (int i = 0; i < VeryLowActionTargetAssets.Length; i++){
                    var data = VeryLowActionTargetAssets[i];
                    f[i] = data;
                }

                return f;
        }

        return default;
    }

    public NativeArray<ConditionsAssetData> GetResponseActionConditionsArray(Severity category, Allocator allocator){
        switch (category){
            case Severity.CRITICAL:
                var a = new NativeArray<ConditionsAssetData>(CriticalActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < CriticalActionConditionAssets.Length; i++){
                    var data = CriticalActionConditionAssets[i];
                    a[i] = data;
                }

                return a;
            case Severity.VERY_HIGH:
                var b = new NativeArray<ConditionsAssetData>(VeryHighActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < VeryHighActionConditionAssets.Length; i++){
                    var data = VeryHighActionConditionAssets[i];
                    b[i] = data;
                }

                return b;
            case Severity.HIGH:
                var c = new NativeArray<ConditionsAssetData>(HighActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < HighActionConditionAssets.Length; i++){
                    var data = HighActionConditionAssets[i];
                    c[i] = data;
                }

                return c;
            case Severity.MID:
                var d = new NativeArray<ConditionsAssetData>(MidActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < MidActionConditionAssets.Length; i++){
                    var data = MidActionConditionAssets[i];
                    d[i] = data;
                }

                return d;
            case Severity.LOW:
                var e = new NativeArray<ConditionsAssetData>(LowActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < LowActionConditionAssets.Length; i++){
                    var data = LowActionConditionAssets[i];
                    e[i] = data;
                }

                return e;
            case Severity.VERY_LOW:
                var f = new NativeArray<ConditionsAssetData>(VeryLowActionConditionAssets.Length,
                    allocator);
                for (int i = 0; i < VeryLowActionConditionAssets.Length; i++){
                    var data = VeryLowActionConditionAssets[i];
                    f[i] = data;
                }

                return f;
        }

        return default;
    }

    public NativeArray<NpcBaseResponseAssetData> GetResponseArray(Severity category, Allocator allocator){
        switch (category){
            case Severity.CRITICAL:
                var a = new NativeArray<NpcBaseResponseAssetData>(CriticalResponseAssets.Length,
                    allocator);
                for (int i = 0; i < CriticalResponseAssets.Length; i++){
                    var data = CriticalResponseAssets[i];
                    a[i] = data;
                }

                return a;
            case Severity.VERY_HIGH:
                var b = new NativeArray<NpcBaseResponseAssetData>(VeryHighResponseAssets.Length,
                    allocator);
                for (int i = 0; i < VeryHighResponseAssets.Length; i++){
                    var data = VeryHighResponseAssets[i];
                    b[i] = data;
                }

                return b;
            case Severity.HIGH:
                var c = new NativeArray<NpcBaseResponseAssetData>(HighResponseAssets.Length,
                    allocator);
                for (int i = 0; i < HighResponseAssets.Length; i++){
                    var data = HighResponseAssets[i];
                    c[i] = data;
                }

                return c;
            case Severity.MID:
                var d = new NativeArray<NpcBaseResponseAssetData>(MidResponseAssets.Length,
                    allocator);
                for (int i = 0; i < MidResponseAssets.Length; i++){
                    var data = MidResponseAssets[i];
                    d[i] = data;
                }

                return d;
            case Severity.LOW:
                var e = new NativeArray<NpcBaseResponseAssetData>(LowResponseAssets.Length,
                    allocator);
                for (int i = 0; i < LowResponseAssets.Length; i++){
                    var data = LowResponseAssets[i];
                    e[i] = data;
                }

                return e;
            case Severity.VERY_LOW:
                var f = new NativeArray<NpcBaseResponseAssetData>(LowResponseAssets.Length,
                    allocator);
                for (int i = 0; i < LowResponseAssets.Length; i++){
                    var data = LowResponseAssets[i];
                    f[i] = data;
                }

                return f;
        }

        return default;
    }

    public bool TryGetBaseResponseData(Guid responseId, NativeArray<NpcBaseResponseAssetData> responseData,
        out NpcBaseResponseAssetData baseResponseAssetData){
        for (int i = 0; i < responseData.Length; i++){
            if (responseData[i].ResponseId == responseId){
                baseResponseAssetData = responseData[i];
                return true;
            }
        }

        baseResponseAssetData = default;
        return false;
    }

    public void SetResponseActionsTargetsConditions(Guid id, NativeArray<ActionAssetData> actionsData,
        NativeArray<GenericTargetAssetData> targetData, NativeArray<ConditionsAssetData> conditionsData,
        DynamicBuffer<ActionElement> actElements, DynamicBuffer<TargetElement> targElements,
        DynamicBuffer<ConditionElement> condElements){
        for (int i = 0; i < actionsData.Length; i++){
            var nextAct = actionsData[i];
            if (nextAct.ParentId == id){
                var act = nextAct.ToBufferElement();
                act.SourceId = id;
                act.IsEnabled = true;
                actElements.Add(act);

                for (int j = 0; j < targetData.Length; j++){
                    var targ = targetData[j];
                    if (targ.ParentId == act.ActionId){
                        targElements.Add(targ.ToTargetBufferElement());
                        foreach (var targCond in targ.Conditions){
                            condElements.Add(new ConditionElement(){
                                ConditionData = targCond,
                                ParentId = targ.Id
                            });
                        }
                    }
                }

                for (int j = 0; j < conditionsData.Length; j++){
                    var cond = conditionsData[j];
                    if (cond.ParentId == act.ActionId){
                        foreach (var condData in cond.Conditions){
                            condElements.Add(new ConditionElement(){
                                ConditionData = condData,
                                ParentId = act.ActionId
                            });
                        }
                    }
                }
            }
        }
    }
}