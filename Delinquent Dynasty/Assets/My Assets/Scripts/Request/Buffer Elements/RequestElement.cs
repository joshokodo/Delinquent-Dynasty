using System;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct RequestElement : IBufferElementData {
    public Guid OriginAction;
    public Entity RequestOrigin;
    public Entity RequestTarget;
    public RequestType RequestType;
    public YesNoChoiceType ChoiceType;
    public double ExpireTime;
    public bool RequestStarted;
}