using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_TutorialFish : MonoBehaviour
{
    Scr_GameManager gameManager;
    Scr_Fish fishScript;
    Scr_Tutorials tutorials;

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
        fishScript = GetComponent<Scr_Fish>();
        tutorials = gameManager.tutorials;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShowTutorial1();
        CheckHideTutorial1();

    }

    void CheckShowTutorial1()
    {
        if (!gameManager.tutorials.tutorialsShown[0] && fishScript.isHungry)
        {
            //show fish food button and tutorial box
            tutorials.ShowElement(tutorials.hudButtons[2]);

            tutorials.ShowTutorialBox(0);
            tutorials.tutorialsShown[0] = true;
        }
    }

    void CheckHideTutorial1()
    {
        if (tutorials.tutorialsShown[0] && !fishScript.isHungry && tutorials.boxes[0].activeSelf)
        {
            tutorials.HideTutorialBox(0);
        }
    }
}
