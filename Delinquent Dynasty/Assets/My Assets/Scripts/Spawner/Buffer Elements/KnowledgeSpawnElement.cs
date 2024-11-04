using Unity.Collections;
using Unity.Entities;

public struct KnowledgeSpawnElement : IBufferElementData {
    //TODO: later when adding modding or when prime, sec, tertiary is confusing, create constructors/methods with args that make sense (factory buildX(1,2,3))
    public Entity PrimaryTarget;
    public Entity SecondaryTarget;
    public Entity TertiaryTarget;
    public Entity Source;

    public KnowledgeType KnowledgeType;

    public int IntValue;

    public DynamicGameEnum PrimaryEnumValue;
    public DynamicGameEnum SecondaryEnumValue;

    public Entity LearningEntity;

    public KnowledgeSpawnElement AsSecurityCodeAccess(Entity learning, Entity target, Entity knowing,
        Entity lockSocket = default){
        KnowledgeType = KnowledgeType.SECURITY_CODE_ACCESS;
        PrimaryTarget = target;
        SecondaryTarget = knowing;
        TertiaryTarget = lockSocket;
        LearningEntity = learning;
        return this;
    }
}