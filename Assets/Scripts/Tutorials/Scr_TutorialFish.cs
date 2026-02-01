using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_TutorialFish : MonoBehaviour
{
    Scr_GameManager gameManager;
    Scr_Fish fishScript;
    Scr_Tutorials tutorials;

    bool shown1;
    bool shown11;
    bool shown12;

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

        CheckTryingToFeed();
        CheckToStartFreak();
        CheckIfFreaked();
    }

    void CheckShowTutorial1()
    {
        if (fishScript.isHungry && !tutorials.tutorialsShown[1])
        {
            //show fish food button and tutorial box
            tutorials.ShowElement(tutorials.hudButtons[2]);

            tutorials.ShowNextTutorialBox();
        }
    }

    void CheckHideTutorial1()
    {
        if (tutorials.tutorialsShown[1] && !fishScript.isHungry && !shown1)
        {
            shown1 = true;
            tutorials.HideTutorialBox();
            tutorials.ShowNextTutorialBoxDelay(2f);
            tutorials.HideNextTutorialBoxDelay(5.5f);
            gameManager.notifications.ShowDelayed("Fishing is ready!", 7f, false);
            tutorials.ShowNextTutorialBoxDelay(9f);
        }

    }

    void CheckTryingToFeed()
    {
        Vector2 cameraPos = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
        Vector2 foregroundTankPos = new Vector2(gameManager.foregroundTank.transform.position.x, gameManager.foregroundTank.transform.position.y);
        if (fishScript.isHungry && tutorials.tutorialsShown[7] && !tutorials.tutorialsShown[8] && cameraPos == foregroundTankPos)
        {
            gameManager.ClickTutorialFishFood();
        }
    }

    void CheckToStartFreak()
    {
        if (tutorials.tutorialsShown[10] && !fishScript.isHungry && !shown11)
        {
            shown11 = true;

            Scr_Fish[] tutorialFishScrpts = FindObjectsOfType<Scr_Fish>();

            foreach (Scr_Fish fishScript in tutorialFishScrpts)
            {
                fishScript.freakCount = 999992;
            }
        }
    }

    void CheckIfFreaked()
    {
        if (tutorials.tutorialsShown[10] && fishScript.freakCount < 100 && !shown12 && shown11)
        {
            shown12 = true;

            fishScript.freakCount = 0;
            fishScript.minutesUntilFreaky = 1000000;
            tutorials.ShowNextTutorialBox();
            tutorials.HideNextTutorialBoxDelay(4f);
            gameManager.Scr_Customer.InvokeFunctionWithDelay("SpawnTutorialCustomer", 6f);
            tutorials.ShowNextTutorialBoxDelay(6f);
        }
    }
}
