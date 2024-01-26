using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_GameManager : MonoBehaviour
{
    public static Scr_GameManager GMinstance;

    private Camera _Camera;

    public AudioSource AS;

    public int moneyAmount;
    public TextMeshProUGUI moneyText;

    public GameObject foodPellet;
    public GameObject goldCoin;
    public int feedingCost;
    public float groundTimeUntilDespawn;

    public int goldCoinWorth;
    public int fishIdCounter = 0;

    public Vector3 spawnPosition;

    public List<GameObject> foodPelletList = new List<GameObject>();
    public List<GameObject> fishList = new List<GameObject>();

    // List of Sounds

    public AudioClip SFX_DropCoin, SFX_DropFish, SFX_DropFood, SFX_FishDeath, SFX_FishEat, SFX_MoneyPickup;


    private void Awake()
    {
        if (GMinstance == null)
        {
            GMinstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _Camera = Camera.main;

        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        //moneyText.text = "$" + moneyAmount.ToString();
        moneyText.GetComponent<Scr_NumberCounter>().SetValue = moneyAmount;
    }

    public int GetMoneyAmount()
    {
        return moneyAmount;
    }
    public void SetMoneyAmount(int _newMoneyAmount)
    {
        moneyAmount = _newMoneyAmount;
        UpdateMoneyText();
    }

    public void DropFood()
    {
        if (GetMoneyAmount() >= feedingCost)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject newfoodPellet = Instantiate(foodPellet, mouseWorldPosition, Quaternion.identity);

            foodPelletList.Add(newfoodPellet);

            SetMoneyAmount(GetMoneyAmount() - feedingCost);

            Debug.Log(fishList.Count);

            PlaySoundEffect(SFX_DropFood, 1, 0.5f, 1.5f);

        }
        else
        {
            Debug.Log("Insufficient Money to Feed : $" + feedingCost);
        }
    }
    public void SpawnFish(GameObject _fishToSpawn)
    {
        int fishCost = _fishToSpawn.GetComponent<Scr_Move>().fishCost;
        if (GetMoneyAmount() >= fishCost)
        {
            SetMoneyAmount(GetMoneyAmount() - fishCost);
            GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);

            PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);
            Debug.Log(newFish.GetComponent<Scr_Move>().fishId);
        }
        else
        {
            Debug.Log("Insufficient Money for Fish : $" + fishCost);
        }


    }

    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale)
    {
        AS.PlayOneShot(_soundEffect, _volumeScale);
    }
    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale, float _pitch)
    {
        AS.pitch = _pitch;
        AS.PlayOneShot(_soundEffect, _volumeScale);
        StartCoroutine(ResetPitchAfterDelay(_soundEffect.length));
    }
    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale, float _lowerPitch, float _upperPitch)
    {
        float newPitch = Random.Range(_lowerPitch, _upperPitch);
        AS.pitch = newPitch;
        AS.PlayOneShot(_soundEffect, _volumeScale);
        StartCoroutine(ResetPitchAfterDelay(_soundEffect.length));
    }
    IEnumerator ResetPitchAfterDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        AS.pitch = 1.0f;
    }
}
