using Unity.Entities;

public struct CharactersDataStore : IComponentData {
    public BlobAssetReference<RelationshipBlobAssets> RelationshipBlobAssets;
}