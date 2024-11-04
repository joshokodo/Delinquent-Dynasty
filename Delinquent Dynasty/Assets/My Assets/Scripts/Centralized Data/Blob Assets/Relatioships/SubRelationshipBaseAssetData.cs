using System;
using Unity.Collections;

public struct SubRelationshipBaseAssetData {
    public Guid Id;
    public RelationshipSubTitleType RelationshipSubTitleType;
    public FixedString512Bytes RelationshipDescription;
}