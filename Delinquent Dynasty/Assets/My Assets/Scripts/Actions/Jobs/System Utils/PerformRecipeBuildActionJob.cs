using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct PerformRecipeBuildActionJob : IJobEntity {
    [ReadOnly] public DynamicActionType DynamicActionType;
    [ReadOnly] public double TotalInGameSeconds;
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public CraftingDataStore CraftingDataStore;
    [ReadOnly] public int PhaseAfterComplete;
    [ReadOnly] public ComponentLookup<ItemBuildInProgressComponent> BuildLookup;
    public float TimeModifier;

    public EntityCommandBuffer.ParallelWriter Ecb;


    public void Execute(Entity e, [EntityIndexInChunk] int sortKey, DynamicBuffer<ActiveActionElement> actions, DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(DynamicActionType, actions, targets,
                TargetType.TARGET_BUILD_IN_PROGRESS, out ActiveActionElement activeAction, out Entity target,
                out int index)){
            var itemType = BuildLookup[target].SuccessfulProduct;

            var recipeData = CraftingDataStore.CraftingBlobAssets.Value.GetRecipeData(itemType);

            var performTime = Mathf.CeilToInt((float) recipeData.BuildIterationTime * TimeModifier);

            actionUtils.HandlePerformance(activeAction, actions, index, TotalInGameSeconds, performTime, Ecb, sortKey,
                e, PhaseAfterComplete);
        }
    }
}

public struct PerformRecipeBuildActionUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public CraftingDataStore CraftingDataStore;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public InGameTime InGameTime;
    public float PerformTimeModifier;

    public EntityQuery Query;

    private ComponentLookup<ItemBuildInProgressComponent> _buildLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;
        Query = state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentFunctionPerformQuery<T>());

        _buildLookup = state.GetComponentLookup<ItemBuildInProgressComponent>();

        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _buildLookup.Update(ref state);
    }

    public PerformRecipeBuildActionJob GetPerformRecipeBuildActionJob(int nextPhase){
        return new PerformRecipeBuildActionJob(){
            Ecb = Ecb,
            TotalInGameSeconds = InGameTime.TotalInGameSeconds,
            DynamicActionType = ActionType,
            ActionDataStore = ActionDataStore,
            PhaseAfterComplete = nextPhase,
            CraftingDataStore = CraftingDataStore,
            TimeModifier = PerformTimeModifier,
            BuildLookup = _buildLookup
        };
    }
}