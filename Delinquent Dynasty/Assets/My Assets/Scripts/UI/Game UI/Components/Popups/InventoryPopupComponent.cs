using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// TODO: this and the rest of the popup components seem dumb or at least under utilized
// add more to it, use it more than this seems pointless but meh maybe not
public class InventoryPopupComponent : IComponentData {
    public InventoryPopup Popup;
}