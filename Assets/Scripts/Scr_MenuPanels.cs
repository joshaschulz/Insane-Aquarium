using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_MenuPanels : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Animator PanelAnimator;

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
    }
    public void TogglePanel()
    {
        bool currentlyOpen = PanelAnimator.GetBool("isOpen");
        if (currentlyOpen)
            PanelOff();
        else
            PanelOn();
    }
    public void PanelOff()
    {
        PanelAnimator.SetBool("isOpen", false);
        gameManager.PlaySoundEffect(gameManager.SFX_moveInWater, 0.1f, 0.8f);
    }
    public void PanelOn()
    {
        PanelAnimator.SetBool("isOpen", true);
        gameManager.PlaySoundEffect(gameManager.SFX_moveInWater, 0.1f);
    }


    public void RockClickSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_mainMenuButtons, 0.5f);
    }
    public void UIClickSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_nonRockUI, 0.07f);
    }
}
