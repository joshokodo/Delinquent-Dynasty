using System;
using Unity.Collections;
using Unity.Entities;

//TODO: got crazy good performance boost from decoupling npc state from character and having 
public struct NpcStateEntityComponent : IComponentData {
    public Guid ResponseId;
    public Severity Severity;
    public Entity CharacterEntity;
    public bool UpdateActions;
    public bool DeleteCurrentPlans;
    public FixedList4096Bytes<NpcGoalData> Goals;
    public NPCPersonalityType PersonalityType;
}