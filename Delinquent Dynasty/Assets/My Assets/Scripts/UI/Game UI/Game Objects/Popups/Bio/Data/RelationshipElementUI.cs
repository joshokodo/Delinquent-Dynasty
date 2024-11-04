using Unity.Collections;
using Unity.Entities;

public struct RelationshipElementUI {
    public FixedString512Bytes TitleText;
    public FixedString128Bytes InfluenceOverYouText;
    public FixedString128Bytes InfluenceOverThemText;
    public FixedString128Bytes YourAdmirationText;
    public FixedString128Bytes TheirAdmirationText;
    public FixedString128Bytes YourAttractionText;
    public FixedString128Bytes TheirAttractionText;
    public FixedString128Bytes YourFearText;
    public FixedString128Bytes TheirFearText;
    public FixedString128Bytes YourEntitlmentText;
    public FixedString128Bytes TheirEntitlementText;
}