using Unity.Entities;

public struct SelectedCharacter : IComponentData {
    // public bool IsPlayerControlled;
    public Entity SecondarySelected;
    public bool UpdateSecondarySelectedUI;

    // inventory ui
    public bool UpdateInventoryUI;
    public bool ShowInventoryUI;

    // character bio
    public bool ShowCharacterBioUI;
    public bool UpdateCharacterBioUI;
    public bool UpdateRelationshipsUI;
    public bool UpdateSkillsUI;
    public bool UpdateInterestUI;
    public bool UpdateTraitsUI;

    // actions
    public bool ShowActionsUI;
    public bool UpdateActionsUI;

    // knowledge
    public bool ShowKnowledgeUI;
    public bool UpdateKnowledgeUI;

    // overview
    public bool UpdateOverviewUI;
    public bool UpdatePhysicalHealthUI;
    public bool UpdateMentalHealthUI;
    public bool UpdateHappinessUI;
    public bool UpdateNourishmentUI;
    public bool UpdateFocusUI;
    public bool UpdateEnergyUI;
    public bool UpdateSleepUI;
    public bool UpdateHygieneUI;

    public bool UpdateCurrentLocation;

    // Interactables
    public bool ShowInteractableUI;
    public bool UpdateInteractableUI;
    public InteractableType InteractableTypeUI;
}