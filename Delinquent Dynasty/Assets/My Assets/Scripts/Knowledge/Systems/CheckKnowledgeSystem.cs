using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(PreActionsGroup), OrderLast = true)]
[BurstCompile]
public partial struct CheckKnowledgeSystem : ISystem {
    private EntityCommandBuffer _bsecb;
    private EntityQuery _knowledgeHoldingEntitiesQuery;
    private EntityQuery _knowledgeBaseEntitiesQuery;
    private BufferLookup<CharacterKnowledgeElement> _knowledgeLookup;

    public void OnCreate(ref SystemState state){
        _knowledgeHoldingEntitiesQuery =
            state.GetEntityQuery(CommonSystemUtils.BuildHasAllQuery<CharacterKnowledgeElement>());
        _knowledgeLookup = SystemAPI.GetBufferLookup<CharacterKnowledgeElement>();
    }

    //todo update clear knowledge logic
    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        // _bsecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
        //     .CreateCommandBuffer(state.WorldUnmanaged);
        // _knowledgeLookup.Update(ref state);
        //
        // var knowledgeHoldingEntites = _knowledgeHoldingEntitiesQuery.ToEntityArray(Allocator.TempJob);
        //
        // var h1 = new ClearKnowledgeJob(){
        //         KnowledgeHoldingEntities = knowledgeHoldingEntites,
        //         KnowledgeLookup = _knowledgeLookup,
        //         Ecb = _bsecb
        //     }.Schedule(_knowledgeBaseEntitiesQuery, state.Dependency);
        //
        // state.Dependency = JobHandle.CombineDependencies(h1, state.Dependency);
    }
}

[BurstCompile]
public partial struct ClearKnowledgeJob : IJobEntity {
    public EntityCommandBuffer Ecb;
    [ReadOnly] public NativeArray<Entity> KnowledgeHoldingEntities;
    [ReadOnly] public BufferLookup<CharacterKnowledgeElement> KnowledgeLookup;

    public void Execute(Entity e){
        Ecb.RemoveComponent<CheckKnowledgeForDeleteTag>(e);
        var hasConnection = false;
        foreach (var knowledgeHoldingEntity in KnowledgeHoldingEntities){
            var nextKnowledge = KnowledgeLookup[knowledgeHoldingEntity];
            foreach (var knowledgeElement in nextKnowledge){
                if (knowledgeElement.KnowledgeEntity == e){
                    hasConnection = true;
                    break;
                }
            }

            if (hasConnection){
                break;
            }
        }

        if (!hasConnection){
            Ecb.DestroyEntity(e);
        }
    }
}