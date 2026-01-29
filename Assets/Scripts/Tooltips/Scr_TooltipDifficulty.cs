using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_TooltipDifficulty : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Scr_Tooltip tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(DetermineMessage());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }

    private void OnDisable()
    {
        tooltip.Hide();
    }

    private string DetermineMessage()
    {
        string message = "";

        if (gameObject.name.Contains("Easy"))
        {
            message = $"+25% fishkeeping\n-25% rent and taxes";
        }
        else if (gameObject.name.Contains("Medium"))
        {
            message = $"+0% fishkeeping\n-0% rent and taxes";
        }
        else if (gameObject.name.Contains("Hard"))
        {
            message = $"-25% fishkeeping\n+25% rent and taxes\nTime is paused less";
        }

        return message;
    }
}
