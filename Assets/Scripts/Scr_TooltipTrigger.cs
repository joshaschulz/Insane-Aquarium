using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_TooltipTrigger : MonoBehaviour
{
    [TextArea]
    public string tooltipText;

    private void OnMouseEnter()
    {
        if (Scr_Tooltip.Instance != null)
            Scr_Tooltip.Instance.Show(tooltipText);
        else
            Debug.LogError("NO TOOLTIP INSTANCE IN SCENE!");
    }

    private void OnMouseExit()
    {

        if (Scr_Tooltip.Instance != null)
            Scr_Tooltip.Instance.Hide();
    }
}
