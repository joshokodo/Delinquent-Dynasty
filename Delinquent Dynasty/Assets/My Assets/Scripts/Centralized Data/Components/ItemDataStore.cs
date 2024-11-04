using Unity.Entities;

public struct ItemDataStore : IComponentData {
    public BlobAssetReference<ItemBlobAssets> ItemBlobAssets;
}