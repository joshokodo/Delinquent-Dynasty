using Unity.Entities;
using UnityEngine;

public class NpcStateAuthoring : MonoBehaviour { }

public class NpcStateBaker : Baker<NpcStateAuthoring> {
    public override void Bake(NpcStateAuthoring authoring){
        AddComponent<NpcStateEntityComponent>();
        AddComponent<NpcCheckCharacterBioTag>();
        AddComponent<NpcCheckAttributeStatusTag>();
        AddComponent<NpcCheckTimeBaseTag>();
        AddComponent<NpcCheckLocationBaseTag>();
        AddComponent<NpcCheckSkillStatusTag>();
        AddComponent<NpcCheckWellnessStatusTag>();
        AddComponent<NpcCheckInterestStatusTag>();
        AddComponent<NpcCheckExternalEventsTag>();
        AddComponent<NpcCheckAcademicStatusTag>();
        AddComponent<NpcResponseUpdateTag>();
        AddBuffer<NPCScenarioElement>();
    }
}