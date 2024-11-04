using Unity.Entities;

public struct XHasFirstLunchPeriodConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var hasFirstLunchPeriod = true;
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var originKnowledge = utils.RoomKnowledgeLookup[target];

        foreach (var knowledgeElement in originKnowledge){
            if (utils.StudentClassPeriodKnowledgeLookup.TryGetBuffer(knowledgeElement.KnowledgeEntity,
                    out DynamicBuffer<StudentClassKnowledgeElement> classKnowledgeElements)){
                foreach (var studentClassKnowledgeElement in classKnowledgeElements){
                    if (studentClassKnowledgeElement.StudentEntity == target &&
                        studentClassKnowledgeElement.Period == 3){
                        hasFirstLunchPeriod = false;
                        break;
                    }
                }

                if (!hasFirstLunchPeriod){
                    break;
                }
            }
        }


        return hasFirstLunchPeriod == conditionData.ExpectedConditionValue;
    }
}