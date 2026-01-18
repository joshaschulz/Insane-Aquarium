using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_AnimationEvents : MonoBehaviour
{
    Scr_FishingMinigameChestController chestControllerScr;
    Scr_FishingMinigamePanel fishingPanelScr;

    private int rewardIndex = 0;

    public void Awake()
    {
        chestControllerScr = FindObjectOfType<Scr_FishingMinigameChestController>();
        fishingPanelScr = FindObjectOfType<Scr_FishingMinigamePanel>();
    }

    public void OnEnable()
    {
        
    }

    public void SetRewardIndex(int index)
    {
        rewardIndex = index;
    }

    public void HideFishingButtons()
    {
        fishingPanelScr.bagFishButton.gameObject.SetActive(false);
        fishingPanelScr.flushFishButton.gameObject.SetActive(false);
    }

    public void OnCrateBreakFinished()
    {
        // hide all rewards first
        for (int i = 0; i < chestControllerScr.rewardPrefabs.Length; i++)
        {
            if (chestControllerScr.rewardPrefabs[i] != null)
                chestControllerScr.rewardPrefabs[i].gameObject.SetActive(false);
        }

        GameObject img = chestControllerScr.rewardPrefabs[rewardIndex];

        Debug.Log(img.name);

        GameObject imgObj = Instantiate(img);
        imgObj.transform.position = transform.position;

        imgObj.transform.SetParent(transform.parent);

        Debug.Log(img);

        imgObj.SetActive(true);

        gameObject.SetActive(false);

        fishingPanelScr.bagFishButton.gameObject.SetActive(true);
        fishingPanelScr.flushFishButton.gameObject.SetActive(true);
        fishingPanelScr.treasureQuantityText.SetActive(true);
    }
}
