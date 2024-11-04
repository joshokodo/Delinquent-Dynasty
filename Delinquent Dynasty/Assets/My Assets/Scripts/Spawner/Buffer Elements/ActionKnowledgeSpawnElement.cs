using Unity.Collections;
using Unity.Entities;

public struct ActionKnowledgeSpawnElement : IBufferElementData {
    public Entity PerformingCharacter;
    public Entity Source;
    public DynamicActionType DynamicActionType;
    public bool IsSuccessful;
    public TargetsGroup TargetsData;
    public FixedList4096Bytes<Entity> TargetCharacters;
    public bool HideKnowledgeFromTargets;
}