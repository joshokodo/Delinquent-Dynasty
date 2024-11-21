using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[BurstCompile]
public partial struct TriggerSendTextMessageJob : IJobEntity {
// todo try to make this job parallel?
    [ReadOnly] public ActionDataStore ActionDataStore;
    [ReadOnly] public SkillDataStore SkillDataStore;

    public DynamicActionType DynamicActionType;

    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnSpawn;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> Random;
    
    [ReadOnly] public ComponentLookup<PassiveEffectComponent> PassiveCompLookup;
    [ReadOnly] public BufferLookup<PassiveEffectElement> PassivesLookup;
    [ReadOnly] public BufferLookup<CharacterAttributeElement> AttributesLookup;
    [ReadOnly] public BufferLookup<SkillElement> SkillsLookup;

    public BufferLookup<TextMessageElement> TextLookup;
    public ComponentLookup<ItemCellPhoneComponent> CellLookup;
    public BufferLookup<AccessKnowledgeElement> AccessLookup;
    public ComponentLookup<AccessKnowledgeComponent> AccessCompLookup;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        bool isSuccessful;

        var skillUtils = new SkillUtils(){
            SkillDataStore = SkillDataStore,
        };

        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };
        
        var passiveUtils = new PassiveEffectsUtils(){
            CharacterAttributes = AttributesLookup[comp.CharacterEntity],
            Passives = PassivesLookup[comp.CharacterEntity],
            PassiveCompLookup = PassiveCompLookup,
            SkillUtils = skillUtils,
            Skills = SkillsLookup[comp.CharacterEntity],
        };

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets,
                out ActiveActionElement activeAction, out FixedList4096Bytes<ActiveActionTargetElement> activeTargets,
                out int index)){
        
            var targetData = new TargetsGroup();
            targetData.SetTargets(comp.CharacterEntity, activeTargets);

            var originPhone = targetData.GetTargetEntity(TargetType.TARGET_ITEM);
            var targPhoneNum = targetData.GetTargetEntity(TargetType.TARGET_PHONE);

            if (TextLookup.TryGetBuffer(targPhoneNum, out DynamicBuffer<TextMessageElement> texts)){
                
                var actionData = ActionDataStore.ActionsBlobAssets.Value.GetActionBaseData(DynamicActionType);


                if (actionData.SkillUsed == SkillType.COMMON){
                    isSuccessful = true;
                }
                else{
                    var chance = passiveUtils.GetSkillSuccessChance(actionData.SkillUsed,
                        actionData.DifficultyLevel);
                    isSuccessful = Random.ValueRW.IsSuccessful(chance);
                }
                
                texts.Add(new TextMessageElement(){
                    OriginPhone = originPhone,
                    ActionType = DynamicActionType,
                    IsSuccessful = isSuccessful,
                    SkillLevel = passiveUtils.GetNaturalAndBonusSkillLevel(actionData.SkillUsed)
                });

                var targetCell =  CellLookup[targPhoneNum];
                if (targetCell.MessageNotificationMode != NotificationSoundMode.SILENT){
                    var originOwner = CellLookup[originPhone].Owner;
                    var targetOwner = targetCell.Owner;

                    // todo: possible that 2 or more text happen at the same time and only one is shown. if we care, fix this somehow, make spawn combined all and add to fixed list otherwise another buffer on selected character (eww)
                    var receivedText = new CharacterStateChangeSpawnElement(){
                        Character = targetOwner,
                        RecievedText = true,
                        RecievedTextFrom = originOwner,
                    };
                    
                    StateChangeSpawnElements.Add(receivedText);
                }
            
                
             
                
                var newState = new CharacterStateChangeSpawnElement(){
                    Character = comp.CharacterEntity,
                };

                if (isSuccessful){
                    newState.PerformedSuccessfulAction = true;
                }
                else{
                    newState.PerformedFailedAction = true;
                }

                StateChangeSpawnElements.Add(newState);

                actionUtils.SetCostEffects(comp.CharacterEntity, passiveUtils, actionData, ActiveEffectSpawnSpawn);
                
                var performActionKnowledge = new ActionKnowledgeSpawnElement(){
                    PerformingCharacter = comp.CharacterEntity,
                    DynamicActionType = DynamicActionType,
                    IsSuccessful = isSuccessful,
                    TargetsData = targetData,
                };
                
                var learningTargets = new FixedList4096Bytes<Entity>();
                learningTargets.Add(comp.CharacterEntity);

                performActionKnowledge.TargetCharacters = learningTargets;

                ActionKnowledgeSpawnElements.Add(performActionKnowledge);

                var found = false;
                var phoneKnowledge = AccessLookup[originPhone];
                foreach (var accessKnowledgeElement in phoneKnowledge){
                    var accessComp = AccessCompLookup[accessKnowledgeElement.KnowledgeEntity];
                    if (accessComp.AccessTargetEntity == targPhoneNum){
                        found = true;
                        break;
                    }
                }

                if (!found){
                    KnowledgeSpawnElements.Add(
                        new KnowledgeSpawnElement().AsPhoneNumber(originPhone, targPhoneNum, comp.CharacterEntity));
                }

                found = false;
                var targPhoneKnowledge = AccessLookup[targPhoneNum];
                foreach (var accessKnowledgeElement in targPhoneKnowledge){
                    var accessComp = AccessCompLookup[accessKnowledgeElement.KnowledgeEntity];
                    if (accessComp.AccessTargetEntity == originPhone){
                        found = true;
                        break;
                    }
                }

                if (!found){
                    KnowledgeSpawnElements.Add(
                        new KnowledgeSpawnElement().AsPhoneNumber(targPhoneNum, originPhone, originPhone));
                }
            }

            if (NextPhase < 0){
                actionUtils.StartPhase(DynamicActionType, Ecb, e, -1);
                actionUtils.CompleteAction(activeAction, actions, index);
            }
            else{
                actionUtils.StartPhase(DynamicActionType, Ecb, e, NextPhase);
            }
        }
    }
}

public struct TriggerSendTextMessageUtil {
    public DynamicActionType ActionType;
    public ActionDataStore ActionDataStore;
    public SkillDataStore SkillDataStore;
    public EntityCommandBuffer Ecb;
    public RefRW<RandomComponent> Random;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectSpawnElements;
    public DynamicBuffer<ActionKnowledgeSpawnElement> ActionKnowledgeSpawnElements;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;

    public EntityQuery Query;

    private ComponentLookup<PassiveEffectComponent> _passiveCompLookup;
    private ComponentLookup<AccessKnowledgeComponent> _accessCompLookup;
    private ComponentLookup<ItemCellPhoneComponent> _cellLookup;

    private BufferLookup<AccessKnowledgeElement> _accessLookup;
    private BufferLookup<TextMessageElement> _textLookup;
    private BufferLookup<PassiveEffectElement> _passivesLookup;
    private BufferLookup<CharacterAttributeElement> _attributesLookup;
    private BufferLookup<SkillElement> _skillsLookup;

    public void SetUp<T>(ref SystemState state, DynamicActionType actionType, int phase){
        ActionType = actionType;

        Query =
            state.GetEntityQuery(CommonSystemUtils.BuildCommonActionTargetsComponentConditionsFunctionPerformQuery<T>());

        _passivesLookup = state.GetBufferLookup<PassiveEffectElement>();
        _attributesLookup = state.GetBufferLookup<CharacterAttributeElement>();
        _skillsLookup = state.GetBufferLookup<SkillElement>();
        _passiveCompLookup = state.GetComponentLookup<PassiveEffectComponent>();
        _textLookup = state.GetBufferLookup<TextMessageElement>();
        _accessCompLookup = state.GetComponentLookup<AccessKnowledgeComponent>();
        _cellLookup = state.GetComponentLookup<ItemCellPhoneComponent>();
        _accessLookup = state.GetBufferLookup<AccessKnowledgeElement>();


        Query = CommonSystemUtils.SetFunctionSharedComp<T>(Query, phase, ActionType);
    }

    public void UpdateBufferLookups(ref SystemState state){
        _passiveCompLookup.Update(ref state);
        _passivesLookup.Update(ref state);
        _attributesLookup.Update(ref state);
        _skillsLookup.Update(ref state);
        _textLookup.Update(ref state);
        _accessCompLookup.Update(ref state);
        _accessLookup.Update(ref state);
        _cellLookup.Update(ref state);
    }

    public TriggerSendTextMessageJob GetTriggerActionEffectsJob(int nextPhase){
        return new TriggerSendTextMessageJob(){
            Random = Random,
            ActionDataStore = ActionDataStore,
            DynamicActionType = ActionType,
            SkillDataStore = SkillDataStore,
            ActionKnowledgeSpawnElements = ActionKnowledgeSpawnElements,
            ActiveEffectSpawnSpawn = ActiveEffectSpawnElements,
            PassiveCompLookup = _passiveCompLookup,
            StateChangeSpawnElements = StateChangeSpawnElements,
            PassivesLookup = _passivesLookup,
            NextPhase = nextPhase,
            AttributesLookup = _attributesLookup,
            SkillsLookup = _skillsLookup,
            Ecb = Ecb,
            TextLookup = _textLookup,
            AccessLookup = _accessLookup,
            AccessCompLookup = _accessCompLookup,
            KnowledgeSpawnElements = KnowledgeSpawnElements,
            CellLookup = _cellLookup
        };
    }
}