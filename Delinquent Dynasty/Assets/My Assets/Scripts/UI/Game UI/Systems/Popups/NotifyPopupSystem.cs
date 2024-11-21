using Unity.Entities;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class NotifyPopupSystem : SystemBase {
    private NotifyPopup _popupClone;
    private bool _setPop;
    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<NotifyPopupComponent>()){
                _popupClone = pop.Popup;
            }

            _setPop = true;
        }

        if (SystemAPI.TryGetSingletonEntity<SelectedCharacter>(out Entity e)){
            var selected = SystemAPI.GetAspect<SelectedCharacterAspect>(e);

            if (selected.Selected.ValueRO.TriggerNotifySkillLearned){
                selected.Selected.ValueRW.TriggerNotifySkillLearned = false;
                var msg = "You learned skill " + selected.Selected.ValueRO.NotifySkillLearned + "!";
                TriggerNotify(msg, true);
                selected.Selected.ValueRW.NotifySkillLearned = default;
            }
            if (selected.Selected.ValueRO.TriggerNotifyAttributeLeveledUp){
                selected.Selected.ValueRW.TriggerNotifyAttributeLeveledUp = false;
                var msg = "Your " + selected.Selected.ValueRO.NotifyAttributeLeveledUp + " leveled up!";
                TriggerNotify(msg, true);
                selected.Selected.ValueRW.NotifyAttributeLeveledUp = default;
            }
            if (selected.Selected.ValueRO.TriggerNotifySkillLeveledUp){
                selected.Selected.ValueRW.TriggerNotifySkillLeveledUp = false;
                var msg = "Your " + selected.Selected.ValueRO.NotifySkillLeveledUp + " leveled up!";
                TriggerNotify(msg, true);
                selected.Selected.ValueRW.NotifySkillLeveledUp = default;
            }
            
            if (selected.Selected.ValueRO.TriggerNotifyRecievedText && SystemAPI.HasComponent<CharacterBio>(selected.Selected.ValueRO.RecievedTextFrom)){
                selected.Selected.ValueRW.TriggerNotifyRecievedText = false;
                var msg = "New text from " + SystemAPI.GetComponent<CharacterBio>(selected.Selected.ValueRO.RecievedTextFrom).FullName;
                TriggerNotify(msg, true);
                selected.Selected.ValueRW.NotifySkillLeveledUp = default;
                selected.Selected.ValueRW.RecievedTextFrom = Entity.Null;
            }
            
        }
    }

    public void TriggerNotify(string msg, bool useAuto){

        if (!useAuto){
            _popupClone.simpleNotify.Clone().Show(
                msg,
                customHideDelay: 0f);
        }
        else{
            _popupClone.autoNotify.Clone().Show(
                msg,
                customHideDelay: 10f);
        }
      
    }
}