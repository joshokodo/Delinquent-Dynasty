using Unity.Entities;

public struct TrendingElement : IBufferElementData {
    public DynamicGameEnum EnumValue;
    public Entity Entity;
    public TrendingData TrendingData;
}