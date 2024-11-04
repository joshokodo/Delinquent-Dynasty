using Unity.Entities;

public struct TraitDataStore : IComponentData {
    public BlobAssetReference<TraitBlobAssets> TraitBlobAssets;
}