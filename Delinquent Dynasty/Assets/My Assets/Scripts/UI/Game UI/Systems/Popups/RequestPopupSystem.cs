using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreActionsGroup), OrderFirst = true)]
public partial class RequestPopupSystem : SystemBase {
    private RequestPopup _popupClone;

    private bool _setPop;

    // private RequestComponent _nextRequest;
    private Entity _nextRequestEntity;
    private RequestElement _nextRequestElement;
    private Dictionary<Entity, DynamicBuffer<RequestElement>> _requestMap = new();

    protected override void OnUpdate(){
        if (!_setPop){
            foreach (var pop in SystemAPI.Query<RequestPopupComponent>()){
                _popupClone = pop.Popup;
                _popupClone.ChoiceSelected += MakeChoice;
            }

            _setPop = true;
        }

        if (SystemAPI.TryGetSingletonEntity<SelectedCharacter>(out Entity e)){
            
            _requestMap.Clear();
            _nextRequestElement = default;
            _nextRequestEntity = default;
            
            var request = SystemAPI.GetBuffer<RequestElement>(e);
            if (request.Length > 0){
                _requestMap.Add(e, request);
            }
            

            var items = SystemAPI.GetBuffer<ItemElement>(e);
            foreach (var itemElement in items){
                if (SystemAPI.GetBufferLookup<RequestElement>()
                    .TryGetBuffer(itemElement.ItemEntity, out DynamicBuffer<RequestElement> itemRequest)
                    && itemRequest.Length > 0){
                    _requestMap.Add(itemElement.ItemEntity, itemRequest);
                }
            }

            if (_requestMap.Count > 0){
                if (!_popupClone.isActiveAndEnabled){
                    _popupClone.Show();
                }
                UpdatePopup();
            } else if (_popupClone.isActiveAndEnabled){
                _popupClone.Close();
            }
        }
    }

    private void UpdatePopup(){

        var set = false;
        foreach (var requestMapKey in _requestMap.Keys){
            foreach (var r in _requestMap[requestMapKey]){
                var ratio = (float) (SystemAPI.GetSingleton<InGameTime>().TotalInGameSeconds - r.StartTime) /
                            (float) (r.ExpireTime - r.StartTime);
                switch (r.RequestType){
                    case RequestType.PHONE_CALL:
                        var originCell = SystemAPI.GetComponent<ItemCellPhoneComponent>(r.RequestOrigin);
                        var targetCell = SystemAPI.GetComponent<ItemCellPhoneComponent>(requestMapKey);

                        var targetPhoneTitle =
                            StringUtils.GetItemTypeString(SystemAPI
                                .GetComponent<ItemBaseComponent>(requestMapKey).ItemType) + " (" +
                            SystemAPI.GetComponent<CharacterBio>(targetCell.Owner).FullName + ")";

                        var originPhoneTitle =
                            StringUtils.GetItemTypeString(SystemAPI
                                .GetComponent<ItemBaseComponent>(r.RequestOrigin).ItemType) + " (" +
                            SystemAPI.GetComponent<CharacterBio>(originCell.Owner).FullName + ")";

                        _popupClone.RequestText.text = targetPhoneTitle + " is getting a call from " + originPhoneTitle;
                        set = true;
                        _nextRequestElement = r;
                        _nextRequestEntity = requestMapKey;

                        _popupClone.SetTimeBar(ratio);
                        break;
                }

                if (set){
                    break;
                }
            }

            if (set){
                break;
            }
        }
    }

    private void MakeChoice(bool choice){
        if (_nextRequestEntity != Entity.Null){
            var request = SystemAPI.GetBuffer<RequestElement>(_nextRequestEntity);
            for (var i = 0; i < request.Length; i++){
                var r = request[i];
                if (r.Matches(_nextRequestElement)){
                    r.ChoiceType = choice ? YesNoChoiceType.YES : YesNoChoiceType.NO;
                    request[i] = r;
                    break;
                }
            }
        }
    }

}