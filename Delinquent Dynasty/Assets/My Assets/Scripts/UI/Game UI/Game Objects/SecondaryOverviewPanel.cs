using System;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryOverviewPanel : MonoBehaviour {
    public GameObject CharacterSelectPanel;

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI currentLocation;

    public Image gender;
    public Sprite femaleIcon;
    public Sprite maleIcon;
    public TextMeshProUGUI seniority;
    public TextMeshProUGUI traitsText;

    public TextMeshProUGUI strength;
    public TextMeshProUGUI dexterity;
    public TextMeshProUGUI intelligence;
    public TextMeshProUGUI wisdom;
    public TextMeshProUGUI vitality;
    public TextMeshProUGUI charisma;


    public ProgressbarDeterminate physicalHealth;
    public ProgressbarDeterminate sleep;
    public ProgressbarDeterminate nourishment;
    public ProgressbarDeterminate energy;
    public ProgressbarDeterminate focus;
    public ProgressbarDeterminate happiness;
    public ProgressbarDeterminate hygiene;

    public TextMeshProUGUI physicalHealthText;
    public TextMeshProUGUI sleepText;
    public TextMeshProUGUI nourishmentText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI focusText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI hygieneText;

    public Image YourAdmirationPosFill;
    public Image YourAdmirationNegFill;
    public Image TheirAdmirationPosFill;
    public Image TheirAdmirationNegFill;

    public Image YourAttractionPosFill;
    public Image YourAttractionNegFill;
    public Image TheirAttractionPosFill;
    public Image TheirAttractionNegFill;

    public Image YourFearPosFill;
    public Image YourFearNegFill;
    public Image TheirFearPosFill;
    public Image TheirFearNegFill;

    public Image YourEntitlementPosFill;
    public Image TheirEntitlementPosFill;

    public Image YourResentmentPosFill;
    public Image TheirResentmentPosFill;

    public Image YourRapportPosFill;
    public Image TheirRapportPosFill;

    // public Image YourStrifePosFill;
    // public Image TheirStrifePosFill;

    public TooltipStringViewer CharacterTitleTT;
    public TooltipStringViewer AdmirationTT;
    public TooltipStringViewer AttractionTT;
    public TooltipStringViewer FearTT;
    public TooltipStringViewer EntitlementTT;
    public TooltipStringViewer ResentmentTT;
    public TooltipStringViewer RapportTT;
    // public TooltipStringViewer StrifeTT;

    public TextMeshProUGUI influenceText;

    public void ResetWellness(){
        focus.Value = 0;
        focusText.text = "Focus: ???/???";
        focus.Max = 200;
        
        energy.Value = 0;
        energyText.text = "Energy: ???/???";
        energy.Max = 200;
        
        physicalHealth.Value = 0;
        physicalHealthText.text = "Health: ???/???";
        physicalHealth.Max = 200;
        
        happiness.Value = 0;
        happinessText.text = "Happiness: ???/???";
        happiness.Max = 200;
        
        sleep.Value = 0;
        sleepText.text = "Sleep: ???/???";
        sleep.Max = 200;
        
        hygiene.Value = 0;
        hygieneText.text = "Hygiene: ???/???";
        hygiene.Max = 200;
        
        nourishment.Value = 0;
        nourishmentText.text = "Nourishment: ???/???";
        nourishment.Max = 200;
        
    }

    public void ResetAttributes(){
        strength.text = "???";
        intelligence.text = "???";
        dexterity.text = "???";
        charisma.text = "???";
        vitality.text = "???";
        wisdom.text = "???";
    }

    public void ResetRelationship(){
        AdmirationTT.Data = "Admiration\nTheirs: ???\n";
        YourAdmirationPosFill.fillAmount = 0;
        YourAdmirationNegFill.fillAmount = 0;
        TheirAdmirationPosFill.fillAmount = 0;
        TheirAdmirationNegFill.fillAmount = 0;

        AttractionTT.Data = "Attraction\nTheirs: ???\n";
        YourAttractionPosFill.fillAmount = 0;
        YourAttractionNegFill.fillAmount = 0;
        TheirAttractionPosFill.fillAmount = 0;
        TheirAttractionNegFill.fillAmount = 0;

        FearTT.Data = "Fear\nTheirs: ???\n";
        YourFearPosFill.fillAmount = 0;
        YourFearNegFill.fillAmount = 0;
        TheirFearPosFill.fillAmount = 0;
        TheirFearNegFill.fillAmount = 0;

        EntitlementTT.Data = "Entitlement\nTheirs: ???\n";
        YourEntitlementPosFill.fillAmount = 0;
        TheirEntitlementPosFill.fillAmount = 0;

        ResentmentTT.Data = "Resentment\nTheirs: ???\n";
        YourResentmentPosFill.fillAmount = 0;
        TheirResentmentPosFill.fillAmount = 0;

        RapportTT.Data = "Rapport\nTheirs: ???\n";
        YourRapportPosFill.fillAmount = 0;
        TheirRapportPosFill.fillAmount = 0;
    }
}