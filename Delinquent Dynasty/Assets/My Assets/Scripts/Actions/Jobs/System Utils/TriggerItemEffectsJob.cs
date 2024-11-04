using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[BurstCompile]
public partial struct TriggerItemEffectsJob : IJobEntity {
//TODO: add readOnly where able. do for all jobs!!!!
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public ItemDataStore ItemDataStore;
    public CraftingDataStore CraftingDataStore;
    public EntityCommandBuffer Ecb;

    public int NextPhase;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public DynamicBuffer<ItemSpawnElement> ItemSpawnElements;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;

    [ReadOnly] public ComponentLookup<ItemBaseComponent> ItemBaseLookup;
    [ReadOnly] public ComponentLookup<ItemStackComponent> StackLookup;
    [ReadOnly] public ComponentLookup<ItemCraftedQualityComponent> CraftedLookup;
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawn;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore,
        };

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };


        var itemUtils = new ItemUtils(){
            ItemDataStore = ItemDataStore
        };

        var passiveUtils = new PassiveEffectsUtils(){
            CharacterAttributes = AttributesLookup[comp.CharacterEntity],
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            SkillUtils = skillUtils,
            Skills = SkillsLookup[comp.CharacterEntity]
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);

            var targetItem = actionUtils.GetTargetEntity(TargetType.TARGET_ITEM, activeTargets);
            var itemType = ItemBaseLookup[targetItem].ItemType;

            var difficultyLevel = actionData.DifficultyLevel;
            var isSuccessful = true;

            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            if (DynamicActionType.Matches(new DynamicActionType(CommonItemActionType.READ_ITEM))){
                var readData = ItemDataStore.ItemBlobAssets.Value.GetItemReadData(itemType);
                difficultyLevel += readData.DifficultyLevel;
                isSuccessful = Random.ValueRW.IsSuccessful(passiveUtils.GetSkillSuccessChance(
                    actionData.SkillUsed,
                    difficultyLevel));
                ItemDataStore.ItemBlobAssets.Value.SetReadApplyEffects(comp.CharacterEntity, readData, isSuccessful, targetData,
                    ActiveEffectSpawn);
            }
            else if (DynamicActionType.Matches(new DynamicActionType(CommonItemActionType.CONSUME_ITEM))){
                var consumeData =
                    ItemDataStore.ItemBlobAssets.Value.GetItemConsumeData(itemType);
                ItemDataStore.ItemBlobAssets.Value.SetConsumeApplyEffects(comp.CharacterEntity, consumeData, targetData,
                    ActiveEffectSpawn);


                // todo some dry happening here clean up. or dont you lazy ass
                if (consumeData.ItemAfterConsume.IsNull){
                    if (StackLookup.TryGetComponent(targetItem,
                            out ItemStackComponent stackComponent)){
                        stackComponent.stackCount--;
                        if (stackComponent.stackCount <= 0){
                            Ecb.AddComponent(targetItem, new TransformItemComponent(){
                                ItemType = default,
                                ActualInventorySource = comp.CharacterEntity,
                            });
                        }
                        else{
                            Ecb.SetComponent(targetItem, stackComponent);
                        }
                    }
                    else{
                        Ecb.AddComponent(targetItem, new TransformItemComponent(){
                            ItemType = default,
                            ActualInventorySource = comp.CharacterEntity,
                        });
                    }
                }
                else{
                    itemUtils.TriggerCreateItemFor(comp.CharacterEntity, ItemSpawnElements, consumeData.ItemAfterConsume);
                    if (StackLookup.TryGetComponent(targetItem,
                            out ItemStackComponent stackComponent)){
                        stackComponent.stackCount--;
                        if (stackComponent.stackCount <= 0){
                            Ecb.AddComponent(targetItem, new TransformItemComponent(){
                                ItemType = default,
                                ActualInventorySource = comp.CharacterEntity,
                            });
                        }
                        else{
                            Ecb.SetComponent(targetItem, stackComponent);
                        }
                    }
                    else{
                        Ecb.AddComponent(targetItem, new TransformItemComponent(){
                            ItemType = default,
                            ActualInventorySource = comp.CharacterEntity,
                        });
                    }
                }
            }

            if (CraftedLookup.TryGetComponent(targetItem, out ItemCraftedQualityComponent craftedQualityComponent)){
                CraftingDataStore.CraftingBlobAssets.Value.SetCraftedEffects(targetItem, itemType,
                    craftedQualityComponent.QualityType, targetData, ActiveEffectSpawn);
            }

            ActionKnowledgeSpawnElements.Add(new ActionKnowledgeSpawnElement(){
                PerformingCharacter = comp.CharacterEntity,
                DynamicActionType = DynamicActionType,
                IsSuccessful = isSuccessful,
                TargetsData = targetData
            });

            var newState = new CharacterStateChangeSpawnElement(){
                Character = comp.CharacterEntity,
                InventoryChanged = true,
            };

            if (isSuccessful){
                newState.PerformedSuccessfulAction = true;
            }
            else{
                newState.PerformedFailedAction = true;
            }

            StateChangeSpawnElements.Add(newState);

            if (NextPhase < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(act, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
        }
    }
}

public struct TriggerItemEffectsUtil {
    public EntityQuery Query;
    private DynamicActionType _actionType;

    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public ItemDataStore ItemDataStore;
    public CraftingDataStore CraftingDataStore;
    public EntityCommandBuffer Ecb;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<ItemSpawnElement> ItemSpawnElements;
    public RefRW<RandomComponent> Random;

    private ComponentLookup<ItemBaseComponent> _itemBaseLookup;
    private ComponentLookup<ItemStackComponent> _itemStackLookup;
    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;
    
    private ComponentLookup<ItemCraftedQualityComponent> _craftingLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        _actionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _itemStackLookup = state.GetComponentLookup<ItemStackComponent>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();


        _itemBaseLookup = state.GetComponentLookup<ItemBaseComponent>();
        _craftingLookup = state.GetComponentLookup<ItemCraftedQualityComponent>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, _actionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _itemStackLookup.Update(ref state);
        _passiveCompLookup.Update(ref state);
        _itemBaseLookup.Update(ref state);
        _craftingLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
    }

    public TriggerItemEffectsJob GetTriggerItemEffectsJob(int nextPhase){
        return new TriggerItemEffectsJob(){
            ActionDataStore = ActionDataStore,
            DynamicActionType = _actionType,
            SkillDataStore = SkillDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            Ecb = Ecb,
            NextPhase = nextPhase,
            ItemDataStore = ItemDataStore,
            StackLookup = _itemStackLookup,
            ItemSpawnElements = ItemSpawnElements,
            ItemBaseLookup = _itemBaseLookup,
            PassiveCompLookup = _passiveCompLookup,
            Random = Random,
            ActiveEffectSpawn = ActiveEffectsSpawn,
            CraftingDataStore = CraftingDataStore,
            CraftedLookup = _craftingLookup,
            StateChangeSpawnElements = StateChangeSpawnElements,
            AttributesLookup = _attributesLookup,
            PassivesLookup = _passivesLookup,
            SkillsLookup = _skillsLookup,
        };
    }
}