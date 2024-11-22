using Unity.Entities;

public struct AppDataStore : IComponentData {
    public BlobAssetReference<AppBlobAssets> AppBlobAssets;
}