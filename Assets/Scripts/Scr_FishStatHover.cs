using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishStatType
{
    Appeal,
    Freak,
    Hunger,
    Poop,
    Price
}

public class Scr_FishStatHover : MonoBehaviour
{
    public FishStatType statType;
    public Scr_Tooltip tooltip;


    private void OnMouseEnter()
    {
        string text = FindObjectOfType<Scr_FishInfoPanel>().GetStatTooltipText(statType);
        tooltip.Show(text);
    }

    private void OnMouseExit()
    {
        tooltip.Hide();
    }
    private void OnDisable()
    {
        tooltip.Hide();
    }

}
