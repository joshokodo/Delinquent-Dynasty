using System;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class RequestPopup : Popup {
    public TextMeshProUGUI RequestText;
    public Image TimeBar;
    public event Action YesSelected;
    public event Action NoSelected;

    public void SetTimeBar(float ratio){
        TimeBar.fillAmount = ratio;
    }

    public void ClickedYes(){
        YesSelected?.Invoke();
    }
    
    public void ClickedNo(){
        NoSelected?.Invoke();
    }
}