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
            message = $"• Fish need less care\n• Fish are more valuable\n• Rent stays constant";
        }
        else if (gameObject.name.Contains("Medium"))
        {
            message = $"• Standard fish behavior and value\n• Rent increases daily";
        }
        else if (gameObject.name.Contains("Hard"))
        {
            message = $"• Fish require more care\n• Fish are less valuable\n• Rent increases even faster\n• Time continues while fishing and calling";
        }

        return message;
    }
}
