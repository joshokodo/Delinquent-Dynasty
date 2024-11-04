using Unity.Entities;

public struct CharacterStateChangeSpawnElement : IBufferElementData {
    public Entity Character;
    public bool ActionsChanged;
    public bool SkillsChanged;
    public bool AttributesChanged;
    public bool InterestChanged;
    public bool WellnessChanged;
    public bool TraitsChanged;
    public bool InventoryChanged;
    public bool KnowledgedChanged;
    public bool RelationshipChanged;
    public bool LocationChanged;
    public bool InteractableChanged;
    public bool PerformedSuccessfulAction;
    public bool PerformedFailedAction;
    public bool IsTargetOfExternalEvent;
}