using System.Data;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class RequestPopupSystem : SystemBase {
    private RequestPopup _popupClone;
    private bool _setPop;
    private RequestComponent _nextRequest;
    private Entity _nextRequestOrigin;

    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<RequestPopupComponent>()){
                _popupClone = pop.Popup;
                _popupClone.YesSelected += Accept;
                _popupClone.NoSelected += Reject;
            }

            _setPop = true;
        }

        if (SystemAPI.TryGetSingletonEntity<SelectedCharacter>(out Entity e)){
            var selected = SystemAPI.GetAspect<SelectedCharacterAspect>(e);
            var items = SystemAPI.GetBuffer<ItemElement>(e);

            if (_nextRequest == default){
                foreach (var (requestComp, entityOrigin) in SystemAPI.Query<RequestComponent>().WithEntityAccess()){
                    if (entityOrigin == e){
                        continue;
                    }

                    if (requestComp.RequestTarget == e){
                        _nextRequest = requestComp;
                        _nextRequestOrigin = entityOrigin;
                        break;
                    }
                    else if (SystemAPI.HasComponent<ItemCellPhoneComponent>(requestComp.RequestTarget)){
                        foreach (var item in items){
                            if (item.ItemEntity == requestComp.RequestTarget){
                                _nextRequest = requestComp;
                                _nextRequestOrigin = entityOrigin;
                                break;
                            }
                        }

                        if (_nextRequest != default){
                            break;
                        }
                    }
                }
            }
            else{
                var found = false;
                foreach (var (requestComp, entityOrigin) in SystemAPI.Query<RequestComponent>().WithEntityAccess()){
                    if (entityOrigin == e){
                        continue;
                    }

                    if (requestComp == _nextRequest){
                        found = true;
                        break;
                    }
                }

                if (!found){
                    _nextRequest = default;
                }
                else{
                    switch (_nextRequest.RequestType){
                        case RequestType.PHONE_CALL:
                            var foundCell = false;
                            foreach (var itemElement in items){
                                if (itemElement.ItemEntity == _nextRequest.RequestTarget){
                                    foundCell = true;
                                    break;
                                }
                            }

                            if (!foundCell){
                                _nextRequest = default;
                                _nextRequestOrigin = default;
                            }

                            break;
                    }
                }
            }

            if (_nextRequest != default && !_popupClone.isActiveAndEnabled){
                switch (_nextRequest.RequestType){
                    case RequestType.PHONE_CALL:
                        var originCell = SystemAPI.GetComponent<ItemCellPhoneComponent>(_nextRequest.RequestOrigin);
                        var targetCell = SystemAPI.GetComponent<ItemCellPhoneComponent>(_nextRequest.RequestTarget);

                        var targetPhoneTitle =
                            StringUtils.GetItemTypeString(SystemAPI
                                .GetComponent<ItemBaseComponent>(_nextRequest.RequestTarget).ItemType) + " (" +
                            SystemAPI.GetComponent<CharacterBio>(targetCell.Owner).FullName + ")";

                        var originPhoneTitle =
                            StringUtils.GetItemTypeString(SystemAPI
                                .GetComponent<ItemBaseComponent>(_nextRequest.RequestOrigin).ItemType) + " (" +
                            SystemAPI.GetComponent<CharacterBio>(originCell.Owner).FullName + ")";

                        _popupClone.RequestText.text = targetPhoneTitle + " is getting a call from " + originPhoneTitle;
                        break;
                }

                _popupClone.Show();
            }
            else if (_nextRequest == default && _popupClone.isActiveAndEnabled){
                _popupClone.Close();
            }
            else if (_nextRequest != default && _popupClone.isActiveAndEnabled){
                var ratio = (float) (SystemAPI.GetSingleton<InGameTime>().TotalInGameSeconds - _nextRequest.StartTime) / (float) (_nextRequest.ExpireTime - _nextRequest.StartTime);
                _popupClone.SetTimeBar(ratio);
            }
        }
    }

    private void Reject(){
        if (_nextRequestOrigin != Entity.Null && _nextRequest != default){
            _nextRequest.ChoiceType = YesNoChoiceType.NO;
            EntityManager.SetComponentData(_nextRequestOrigin, _nextRequest);
        }
    }

    private void Accept(){
        if (_nextRequestOrigin != Entity.Null && _nextRequest != default){
            _nextRequest.ChoiceType = YesNoChoiceType.YES;
            EntityManager.SetComponentData(_nextRequestOrigin, _nextRequest);
        }
    }
}