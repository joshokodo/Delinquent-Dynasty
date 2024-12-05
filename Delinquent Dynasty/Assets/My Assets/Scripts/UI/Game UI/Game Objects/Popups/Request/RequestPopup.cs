using System;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class RequestPopup : Popup {
    public TextMeshProUGUI RequestText;
    public Image TimeBar;
    public event Action<bool> ChoiceSelected;

    public override void Close(){
        base.Close();
        RequestText.text = string.Empty;
        TimeBar.fillAmount = 0;
    }

    public void SetTimeBar(float ratio){
        TimeBar.fillAmount = ratio;
    }

    public void ClickedYes(){
        ChoiceSelected?.Invoke(true);
    }
    
    public void ClickedNo(){
        ChoiceSelected?.Invoke(false);
    }
}