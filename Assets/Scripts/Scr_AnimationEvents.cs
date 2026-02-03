using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_AnimationEvents : MonoBehaviour
{
    private Scr_GameManager gameManager;

    Scr_FishingMinigameChestController chestControllerScr;
    Scr_FishingMinigamePanel fishingPanelScr;

    private int rewardIndex = 0;

    private AudioSource reelAudioSource;
    private AudioSource snapAudioSource;
    public AudioClip reelAudioClip;
    public AudioClip lineSnapClip;



    public void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();

        reelAudioSource = gameObject.AddComponent<AudioSource>();
        snapAudioSource = gameObject.AddComponent<AudioSource>();

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

        gameManager.PlaySoundEffect(gameManager.SFX_crateOpen, 0.8f);
    }

    public void StartPlayFishReeling()
    {
        reelAudioSource.clip = reelAudioClip;
        reelAudioSource.loop = true;
        reelAudioSource.playOnAwake = false;
        reelAudioSource.spatialBlend = 0f; // UI-style 2D sound

        reelAudioSource.pitch = 1f;
        reelAudioSource.volume = 0.5f;
        reelAudioSource.Play();
    }

    public void StopFishReeling()
    {
        reelAudioSource.Stop();
    }

    public void PlayFishLineBreak()
    {
        snapAudioSource.clip = lineSnapClip;
        snapAudioSource.loop = false;
        snapAudioSource.playOnAwake = false;
        snapAudioSource.spatialBlend = 0f; // UI-style 2D sound

        snapAudioSource.pitch = 1f;
        snapAudioSource.volume = 0.5f;
        snapAudioSource.Play();
    }
}
