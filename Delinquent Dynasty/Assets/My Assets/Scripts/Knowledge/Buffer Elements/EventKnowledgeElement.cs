using Unity.Collections;
using Unity.Entities;

[InternalBufferCapacity(0)]
public struct EventKnowledgeElement : IBufferElementData {
    public Entity PerformingEntity;
    public DynamicActionType ActionType;
    public bool IsSuccessful;
    public TargetsGroup TargetsData; // todo remove if not used later
    public FixedList512Bytes<Entity> TargetCharacters; // TODO: find out cap and see if we can reduce size
    public KnowledgeTimestamp ActionTimestamp;

    public bool ContainsTarget(Entity t){
        foreach (var targ in TargetCharacters){
            if (targ == t){
                return true;
            }
        }

        return false;
    }
}