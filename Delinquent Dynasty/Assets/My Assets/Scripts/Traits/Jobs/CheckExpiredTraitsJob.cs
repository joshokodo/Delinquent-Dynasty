using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct CheckExpiredTraitsJob : IJobEntity {
    public InGameTime InGameTime;


    public void Execute(Entity e, DynamicBuffer<TraitElement> traits){
        // for (int i = 0; i < traits.Length; i++){
        //     var trait = traits[i];
        //     var nextExpireTime = trait.ExpireTime;
        //     if (nextExpireTime > 0 && InGameTime.TotalInGameSeconds >= nextExpireTime){
        //         traits.RemoveAt(i);
        //         i--;
        //     }
        // }
    }
}