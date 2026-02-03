using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_BubbleButtonManager : MonoBehaviour
{
    public Button[] bubbleButtons;

    public void MakeButtonsUnclickable()
    {
        foreach (Button bubble in bubbleButtons)
        {
            bubble.interactable = false;
        }
    }
    public void MakeButtonsClickable()
    {
        foreach (Button bubble in bubbleButtons)
        {
            bubble.interactable = true;
        }
    }
}
