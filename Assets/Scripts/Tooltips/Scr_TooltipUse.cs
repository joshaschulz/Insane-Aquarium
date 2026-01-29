using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_TooltipUse : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Scr_Tooltip tooltip;

    [TextArea]
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }

    private void OnDisable()
    {
        tooltip.Hide();
    }
}

