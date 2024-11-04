using Unity.Entities;

public struct SkillDataStore : IComponentData {
    public BlobAssetReference<SkillBlobAssets> SkillBlobAssets;
    public int SkillsCount;
}