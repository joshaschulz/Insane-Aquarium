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
    public Scr_CursorFollower cursorFollower;

    public int moneyAmount;
    public TextMeshProUGUI moneyText;

    public int fishFood_1_Amount;
    public TextMeshProUGUI fishFood_1_AmountText;
    public int fishFood_2_Amount;
    public TextMeshProUGUI fishFood_2_AmountText;

    public GameObject fishFood_1_Button;
    public GameObject fishFood_2_Button;
    public GameObject currentFishFoodButtonSelected;

    public GameObject fishFood_1_Prefab;
    public GameObject fishFood_2_Prefab;
    public GameObject currentFishFoodSelected;


    public float groundTimeUntilDespawn;
    public float foodSpawnYLevel;
    public int goldCoinWorth;
    public int fishIdCounter = 0;

    public Vector3 spawnPosition;


    public List<GameObject> foodPelletList = new List<GameObject>();
    public List<GameObject> fishList = new List<GameObject>();

    // List of Sounds
    public AudioClip SFX_DropCoin, SFX_DropFish, SFX_DropFood, SFX_FishDeath, SFX_FishEat, SFX_MoneyPickup, SFX_Select, SFX_Error, SFX_Bubbles1, SFX_Bubbles2;


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

        UpdateText(moneyText, moneyAmount);
        UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
        UpdateText(fishFood_2_AmountText, fishFood_2_Amount);
    }


    public void DropFood(GameObject _foodToDrop)
    {
        if (_foodToDrop != null)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 foodSpawnPos = new Vector2(mouseWorldPosition.x, foodSpawnYLevel);
            GameObject newfoodPellet = Instantiate(_foodToDrop, foodSpawnPos, Quaternion.identity);

            foodPelletList.Add(newfoodPellet);

            SetFishFoodAmount(_foodToDrop, GetFishFoodAmount(_foodToDrop) - 1);

            Debug.Log(fishList.Count);

            PlaySoundEffect(SFX_DropFood, 1, 0.5f, 1.5f);


        }
    }
    public void SpawnFish(GameObject _fishToSpawn)
    {
        //spawn fish at random x coordinate at same designated y coordinate

        //set the x bounds of where the fish can spawn based on the fish to spawn bounding box
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 boundingBoxSize = new Vector2(_fishToSpawn.GetComponent<Scr_Fish>().boundingBox.transform.localScale.x * gameObject.transform.localScale.x, _fishToSpawn.GetComponent<Scr_Fish>().boundingBox.transform.localScale.y * gameObject.transform.localScale.y);

        float tempMinX = -Bounds.x + (0.5f * boundingBoxSize.x * _fishToSpawn.transform.localScale.x);
        float tempMaxX = Bounds.x - (0.5f * boundingBoxSize.x * _fishToSpawn.transform.localScale.x);

        //random x coordinate based on fish bounding box
        float randfloat = Random.Range(tempMinX, tempMaxX);
        spawnPosition.x = randfloat;


        int fishCost = _fishToSpawn.GetComponent<Scr_Fish>().fishCost;
        if (GetMoneyAmount() >= fishCost)
        {
            SetMoneyAmount(GetMoneyAmount() - fishCost);
            GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);
            fishList.Add(newFish);

            PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);
        }
        else
        {
            Debug.Log("Insufficient Money for Fish : $" + fishCost);
        }

    }




    public void UpdateText(TextMeshProUGUI _textObject, int _amount)
    {
        _textObject.GetComponent<Scr_NumberCounter>().SetValue = _amount;
    }


    public int GetMoneyAmount()
    {
        return moneyAmount;
    }
    public void SetMoneyAmount(int _newMoneyAmount)
    {
        moneyAmount = _newMoneyAmount;
        UpdateText(moneyText, moneyAmount);
    }
    public int GetFishFoodAmount(GameObject _fishFoodType)
    {
        if (_fishFoodType == fishFood_1_Prefab)
        {
            return fishFood_1_Amount;
        }
        else if (_fishFoodType == fishFood_2_Prefab)
        {
            return fishFood_2_Amount;
        }
        else
        {
            Debug.Log("INVALID FISH FOOD TYPE");
            return -1;
        }
    }
    public void SetFishFoodAmount(GameObject _fishFoodType, int _newFishFoodAmount)
    {
        if (_fishFoodType == fishFood_1_Prefab)
        {
            fishFood_1_Amount = _newFishFoodAmount;
            UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
        }
        else if (_fishFoodType == fishFood_2_Prefab)
        {
            fishFood_2_Amount = _newFishFoodAmount;
            UpdateText(fishFood_2_AmountText, fishFood_2_Amount);
        }
        else
        {
            Debug.Log("INVALID FISH FOOD TYPE");
        }
    }

    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale)
    {
        GameObject tempAudioObject = new GameObject("TempAudio");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioSource.clip = _soundEffect;
        tempAudioSource.volume = _volumeScale;
        tempAudioSource.Play();

        Destroy(tempAudioObject, _soundEffect.length);

        AS.PlayOneShot(_soundEffect, _volumeScale);
    }
    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale, float _pitch)
    {
        GameObject tempAudioObject = new GameObject("TempAudio");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioSource.clip = _soundEffect;
        tempAudioSource.volume = _volumeScale;
        tempAudioSource.pitch = _pitch;
        tempAudioSource.Play();

        Destroy(tempAudioObject, _soundEffect.length);

        AS.PlayOneShot(_soundEffect, _volumeScale);
    }
    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale, float _lowerPitch, float _upperPitch)
    {
        float newPitch = Random.Range(_lowerPitch, _upperPitch);
        GameObject tempAudioObject = new GameObject("TempAudio");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioSource.clip = _soundEffect;
        tempAudioSource.volume = _volumeScale;
        tempAudioSource.pitch = newPitch;
        tempAudioSource.Play();

        Destroy(tempAudioObject, _soundEffect.length);

        AS.PlayOneShot(_soundEffect, _volumeScale);
    }

    public void PlayRandomSoundEffect(List<AudioClip> _soundEffectsList, List<float> _volumeScalesList)
    {
        int clipIndex = Random.Range(0, _soundEffectsList.Count);
        PlaySoundEffect(_soundEffectsList[clipIndex], _volumeScalesList[clipIndex]);
    }
    public void PlayRandomSoundEffect(List<AudioClip> _soundEffectsList, List<float> _volumeScalesList, List<float> _pitchesList)
    {
        int clipIndex = Random.Range(0, _soundEffectsList.Count);
        PlaySoundEffect(_soundEffectsList[clipIndex], _volumeScalesList[clipIndex], _pitchesList[clipIndex]);
    }
    public void PlayRandomSoundEffect(List<AudioClip> _soundEffectsList, List<float> _volumeScalesList, List<float> _lowerPitchesList, List<float> _upperPitchesList)
    {
        int clipIndex = Random.Range(0, _soundEffectsList.Count);
        PlaySoundEffect(_soundEffectsList[clipIndex], _volumeScalesList[clipIndex], _lowerPitchesList[clipIndex], _upperPitchesList[clipIndex]);
    }

    IEnumerator ResetPitchAfterDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        AS.pitch = 1.0f;
    }


    public void ChangeFishFoodTypeToDrop(GameObject _fishFoodType)
    {
        if (_fishFoodType == null)
        {
            currentFishFoodSelected = null;
            PlaySoundEffect(SFX_Select, 0.7f, 0.8f);
            cursorFollower.GetComponent<Image>().sprite = null;
            cursorFollower.gameObject.SetActive(false);
            currentFishFoodButtonSelected = null;
        }
        else if (_fishFoodType == fishFood_1_Prefab)
        {
            currentFishFoodSelected = fishFood_1_Prefab;
            PlaySoundEffect(SFX_Select, 0.7f);

            cursorFollower.gameObject.SetActive(true);
            cursorFollower.GetComponent<Image>().sprite = fishFood_1_Button.GetComponent<Image>().sprite;
            currentFishFoodButtonSelected = fishFood_1_Button;
        }
        else if (_fishFoodType == fishFood_2_Prefab)
        {
            currentFishFoodSelected = fishFood_2_Prefab;
            PlaySoundEffect(SFX_Select, 0.7f);

            cursorFollower.gameObject.SetActive(true);
            cursorFollower.GetComponent<Image>().sprite = fishFood_2_Button.GetComponent<Image>().sprite;
            currentFishFoodButtonSelected = fishFood_2_Button;
        }
    }

    public void SpawnParticles(GameObject _particles, Vector3 _position, Quaternion _rotation)
    {
        GameObject newParticlesObject = Instantiate(_particles, _position, _rotation);
        ParticleSystem newParticleSystem = newParticlesObject.GetComponent<ParticleSystem>();
        newParticleSystem.Play();
        float totalLifetime = newParticleSystem.main.duration + newParticleSystem.main.startLifetime.constantMax;
        Destroy(newParticlesObject, totalLifetime);
    }

    public void ChangeColor(GameObject _Object, Color _colorToChange) // Checks ALL children, except for those named "Bounding Box"
    {
        // Check for Sprite Renderer components in the object and its children and change the color

        if (_Object.GetComponent<SpriteRenderer>() != null)
        {
            // Change the color of the object
            _Object.GetComponent<SpriteRenderer>().color = _colorToChange;
        }

        // Change the color of the object's children
        foreach (Transform child in _Object.transform)
        {
            if (child.gameObject.GetComponent<SpriteRenderer>() != null && child.name != "Bounding Box")
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = _colorToChange;
            }

            if (child.childCount > 0)
            {
                ChangeColor(child.gameObject, _colorToChange);
            }
        }


        // Check for Image components in the object and its children and change the color

        if (_Object.GetComponent<Image>() != null)
        {
            // Change the color of the object
            _Object.GetComponent<Image>().color = _colorToChange;
        }

        // Change the color of the object's children
        foreach (Transform child in _Object.transform)
        {
            if (child.gameObject.GetComponent<Image>() != null && child.name != "Bounding Box")
            {
                child.gameObject.GetComponent<Image>().color = _colorToChange;
            }

            if (child.childCount > 0)
            {
                ChangeColor(child.gameObject, _colorToChange);
            }
        }



    }
    public void FlashColor(GameObject _Object, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        StartCoroutine(FlashColorCoroutine(_Object, _colorToChange, _flashTime, _flashInterval));
    }
    private IEnumerator FlashColorCoroutine(GameObject _Object, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        float elapsedTime = 0f;
        Color originalColor = Color.white; // Assuming the default color is white.

        while (elapsedTime < _flashTime)
        {
            ChangeColor(_Object, _colorToChange);
            yield return new WaitForSeconds(_flashInterval);
            elapsedTime += _flashInterval;

            ChangeColor(_Object, originalColor);
            yield return new WaitForSeconds(_flashInterval);
            elapsedTime += _flashInterval;
        }
    }
}
