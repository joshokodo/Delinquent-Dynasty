using Unity.Collections;
using Unity.Entities;

public struct GangComponent : IComponentData {
    public FixedString128Bytes GangName;
    public FixedString512Bytes GangCreed;
    public int TotalStandingPoints;
    public FixedString64Bytes RankName1;
    public FixedString64Bytes RankName2;
    public FixedString64Bytes RankName3;
    public FixedString64Bytes RankName4;
    public FixedString64Bytes RankName5;
    public FixedString64Bytes RankName6;
    public FixedString64Bytes RankName7;
}