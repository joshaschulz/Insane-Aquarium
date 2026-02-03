using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_MenuPanels : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Animator PanelAnimator;
    private Scr_BubbleButtonManager bubbleButtonManager;

    private Scr_MenuPanels[] allMenuPanels; 

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();

        allMenuPanels = FindObjectsOfType<Scr_MenuPanels>();

        bubbleButtonManager = GetComponentInParent<Scr_BubbleButtonManager>();
    }
    public void PanelOff()
    {
        PanelAnimator.SetBool("isOpen", false);
        PullInWaterSound();
    }
    public void PanelOn()
    {
        // Make all the bubbles unclickable for the duration of the animation, then make them clickable again.
        // HINT: bubbleButtonManager.MakeButtonsUnclickable() and bubbleButtonManager.MakeButtonsClickable()


        // If any other panels are currently on, turn them off, wait a second, then turn this one on. If not, turn on immediately.
        float activationDelay = 0f;
        foreach (Scr_MenuPanels panel in allMenuPanels)
        {
            if (panel.PanelAnimator.GetBool("isOpen"))
            {
                AllPanelsOff();
                activationDelay = 0.5f;
                break;
            }
        }

        Invoke("PullPanel", activationDelay);
    }
    public void PullPanel()
    {
        PanelAnimator.SetBool("isOpen", true);
        Invoke("PullInWaterSound", 0.2f);
        Invoke("CrateCatchSound", 0.2f);

    }
    public void AllPanelsOff()
    {
        foreach (Scr_MenuPanels panel in allMenuPanels)
        {
            if (panel.PanelAnimator.GetBool("isOpen"))
            {
                panel.PanelOff();
            }
        }
    }

    public void PullInWaterSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_moveInWater, 0.3f);
    }
    public void CrateCatchSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_catchCrate, 0.5f);
    }
    public void UIClickSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_woodThud, 0.4f, 0.85f, 1.15f);
    }


}
