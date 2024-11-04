using System;
using Unity.Collections;

public struct MainRelationshipBaseAssetData {
    public Guid Id;
    public RelationshipMainTitleType RelationshipMainTitleType;
    public FixedString512Bytes RelationshipDescription;
}