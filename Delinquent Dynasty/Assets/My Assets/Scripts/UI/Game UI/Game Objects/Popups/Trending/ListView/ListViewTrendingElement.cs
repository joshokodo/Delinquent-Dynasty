using TMPro;
using UIWidgets;

public class ListViewTrendingElement : ListViewItem, IViewData<TrendingElementUI> {
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI ValueText;

    public void SetData(TrendingElementUI item){
        TitleText.text = item.Title.Value;
        ValueText.text = item.Value.Value;
    }
}