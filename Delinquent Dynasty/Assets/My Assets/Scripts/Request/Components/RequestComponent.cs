using System;
using Unity.Entities;

public struct RequestComponent : IComponentData {
    public Guid OriginAction;
    public Entity RequestTarget;
    public Entity RequestOrigin;
    public RequestType RequestType;
    public YesNoChoiceType ChoiceType;
    public double ExpireTime;
    public double StartTime;
    
    public static bool operator ==(RequestComponent c1, RequestComponent c2) 
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RequestComponent c1, RequestComponent c2) 
    {
        return !c1.Equals(c2);
    }
}