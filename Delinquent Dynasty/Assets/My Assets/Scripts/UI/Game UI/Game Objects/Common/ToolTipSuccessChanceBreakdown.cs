using TMPro;
using UIWidgets;
using UnityEngine;

public class ToolTipSuccessChanceBreakdown : Tooltip<SuccessChanceBreakdownDTO, ToolTipSuccessChanceBreakdown> {
    public TextMeshProUGUI numeratorText; // skill power
    public TextMeshProUGUI numeratorBreakdown;
    public TextMeshProUGUI denominatorText; // difficulty
    public TextMeshProUGUI denominatorBreakdown;
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI bonusBreakdown;
    public TextMeshProUGUI successChance;

    protected override void UpdateView(){ }

    protected override void SetData(SuccessChanceBreakdownDTO data){
        numeratorText.text = data.Numerator;
        numeratorBreakdown.text = data.NumeratorBreakdown;
        denominatorText.text = data.Denominator;
        denominatorBreakdown.text = data.DenominatorBreakdown;
        bonusText.text = data.Bonus;
        bonusBreakdown.text = data.BonusBreakdown;
        successChance.text = data.successChance;
    }
}

public class SuccessChanceBreakdownDTO {
    public string Numerator;
    public string NumeratorBreakdown;
    public string Denominator;
    public string DenominatorBreakdown;
    public string Bonus;
    public string BonusBreakdown;
    public string successChance;
}