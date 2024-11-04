using Unity.Entities;

public readonly partial struct SelectedCharacterAspect : IAspect {
    public readonly RefRW<SelectedCharacter> Selected;
}