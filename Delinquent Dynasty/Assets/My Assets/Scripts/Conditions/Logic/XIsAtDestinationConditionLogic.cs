using System;
using Unity.Entities;

public struct XIsAtDestinationConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        // var perform = utils.CharacterStateLookup[utils.OriginCharacter];
        // var atDest = perform.GetLocomotionState().HasStarted
        //              && perform.GetLocomotionState().ActionType != ActionType.NONE 
        //              && new LocomotionUtils().HasReachedDestination(utils.AgentBodyLookup[utils.OriginCharacter]);
        // return atDest == conditionData.BoolValue;  
        throw new NotImplementedException("FINISH THIS");
        return false;
    }
}