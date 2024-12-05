using Unity.Entities;

public struct CreateAssignmentXDocumentByAuthorYOnItemZLogic : IApplyActiveEffect {
    public DynamicBuffer<DocumentSpawnElement> DocumentSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;

        DocumentSpawn.Add(new DocumentSpawnElement(){
            DocumentType = DocumentType.ASSIGNMENT,
            DocumentHolder = tertiaryTarget,
            PrimaryTarget = primaryTarget,
            SecondaryTarget = secondaryTarget
        });
    }
}