using Unity.Collections;
using Unity.Entities;

public struct TextMessageKnowledgeComponent : IComponentData {
    public TextCompoentBaseData BaseData;
    public bool IsSuccessful;
}