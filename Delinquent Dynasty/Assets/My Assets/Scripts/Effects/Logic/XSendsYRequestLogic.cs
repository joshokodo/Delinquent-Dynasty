using Unity.Entities;
using UnityEngine;

public struct XSendsYRequestLogic : IApplyActiveEffect{
    public DynamicBuffer<RequestElement> SecondaryRequests;
    public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    public InGameTime InGameTime;

    
    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;

        SecondaryRequests.Add(new RequestElement(){
            RequestOrigin = primaryTarget,
            RequestType = data.PrimaryEnumValue.RequestType,
            StartTime = InGameTime.TotalInGameSeconds,
            ExpireTime = InGameTime.TotalInGameSeconds + GetRequestTime(data.PrimaryEnumValue.RequestType),
            PrimaryEnum = data.SecondaryEnumValue,
            SecondaryEnum = data.TertiaryEnumValue,
        });
    }

    private double GetRequestTime(RequestType type){
        switch (type){
            case RequestType.PHONE_CALL:
                return 30;
            
            default:
                return 0;
        }
    }
}