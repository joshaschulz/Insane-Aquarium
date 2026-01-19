using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_MenuPanels : MonoBehaviour
{
    public Animator PanelAnimator;

    public void TogglePanel()
    {
        bool currentlyOpen = PanelAnimator.GetBool("isOpen");
        PanelAnimator.SetBool("isOpen", !currentlyOpen);
    }
    public void PanelOff()
    {
        PanelAnimator.SetBool("isOpen", false);
    }
    public void PanelOn()
    {
        PanelAnimator.SetBool("isOpen", true);
    }
}
