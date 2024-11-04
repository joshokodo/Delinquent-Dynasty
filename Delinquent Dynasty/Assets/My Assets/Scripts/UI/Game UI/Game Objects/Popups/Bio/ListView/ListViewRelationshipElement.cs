using TMPro;
using UIWidgets;
using Unity.Entities;
using UnityEngine;

public class ListViewRelationshipElement : ListViewItem, IViewData<RelationshipElementUI> {
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI InfluenceOverYouText;
    public TextMeshProUGUI InfluenceOverThemText;
    public TextMeshProUGUI YourAdmirationText;
    public TextMeshProUGUI TheirAdmirationText;
    public TextMeshProUGUI YourAttractionText;
    public TextMeshProUGUI TheirAttractionText;
    public TextMeshProUGUI YourFearText;
    public TextMeshProUGUI TheirFearText;
    public TextMeshProUGUI YourEntitlmentText;
    public TextMeshProUGUI TheirEntitlementText;

    public void SetData(RelationshipElementUI item){
        TitleText.text = item.TitleText.Value;
        InfluenceOverYouText.text = item.InfluenceOverYouText.Value;
        InfluenceOverThemText.text = item.InfluenceOverThemText.Value;
        YourAdmirationText.text = item.YourAdmirationText.Value;
        TheirAdmirationText.text = item.TheirAdmirationText.Value;
        YourAttractionText.text = item.YourAttractionText.Value;
        TheirAttractionText.text = item.TheirAttractionText.Value;
        YourFearText.text = item.YourFearText.Value;
        TheirFearText.text = item.TheirFearText.Value;
        YourEntitlmentText.text = item.YourEntitlmentText.Value;
        TheirEntitlementText.text = item.TheirEntitlementText.Value;
    }
}