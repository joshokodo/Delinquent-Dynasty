using System.Text;
using TMPro;
using UIWidgets;

public class ListViewClassPeriodElement : ListViewItem, IViewData<ClassPeriodElementUI>{
    public TextMeshProUGUI classText;
    public void SetData(ClassPeriodElementUI item){
        StringBuilder stringBuilder = new();
        stringBuilder.Append("P" + item.Period);
        stringBuilder.Append(" ");
        stringBuilder.Append(item.SkillType.ToString());
        stringBuilder.Append("\n");
        stringBuilder.Append(item.Grade.Value);
        stringBuilder.Append("\n");
        stringBuilder.Append(item.AssignmentsTotal.Value);
        classText.text = stringBuilder.ToString();
    }
}