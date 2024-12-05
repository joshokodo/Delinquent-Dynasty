using System;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct RequestElement : IBufferElementData {
    public DynamicActionType ActionType;
    public Entity RequestOrigin;
    public Entity RequestPrimarySubject;
    public RequestType RequestType;
    public YesNoChoiceType ChoiceType;
    public double ExpireTime;
    public double StartTime;
    public DynamicGameEnum PrimaryEnum;
    public DynamicGameEnum SecondaryEnum;
    

    public bool Matches(RequestElement other){
        return other.ActionType.Matches(ActionType)
               && other.RequestOrigin == RequestOrigin
               && other.RequestType == RequestType
               && other.StartTime == StartTime
               && other.PrimaryEnum.Matches(PrimaryEnum)
               && other.SecondaryEnum.Matches(SecondaryEnum);
    }
}