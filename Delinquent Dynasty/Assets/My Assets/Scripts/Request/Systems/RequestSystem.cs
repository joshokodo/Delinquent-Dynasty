using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderLast = true)]
[BurstCompile]
public partial struct RequestSystem : ISystem {
    private ComponentLookup<ItemCellPhoneComponent> _cellLookup;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<ActionDataStore>();

        _cellLookup = state.GetComponentLookup<ItemCellPhoneComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        
        _cellLookup.Update(ref state);

        var time = SystemAPI.GetSingleton<InGameTime>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        JobHandle commonJh = new CommonRequestJob(){
            InGameTime = time,
            Ecb = ecb,
            CellLookup = _cellLookup,
        }.Schedule(state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(commonJh, state.Dependency);
    }
}

[BurstCompile]
public partial struct CommonRequestJob : IJobEntity {
    public InGameTime InGameTime;
    public EntityCommandBuffer Ecb;
    public ComponentLookup<ItemCellPhoneComponent> CellLookup;
    
    public void Execute(Entity e, DynamicBuffer<RequestElement> request){
        for (var i = 0; i < request.Length;){
            var r = request[i];
            var finished = false;
            if (InGameTime.TotalInGameSeconds > r.ExpireTime || r.ChoiceType == YesNoChoiceType.NO){
                finished = true;
                ApplyRequest(e, r, false);
            }
            else if (r.ChoiceType == YesNoChoiceType.YES){
                finished = true;
                ApplyRequest(e, r, true);
            }

            if (finished){
                request.RemoveAt(i);
            } else {
                i++;
            }
        }
    }

    public void ApplyRequest(Entity e, RequestElement request, bool isYes){
        switch (request.RequestType){
            case RequestType.PHONE_CALL:
                if (isYes){
                    var cell = CellLookup[e];
                    cell.PhoneCallWith = request.RequestOrigin;
                    Ecb.SetComponent(e, cell);
                    
                    cell = CellLookup[request.RequestOrigin];
                    cell.PhoneCallWith = e;
                    Ecb.SetComponent(request.RequestOrigin, cell);
                }
                break;
        }
    }
    
}