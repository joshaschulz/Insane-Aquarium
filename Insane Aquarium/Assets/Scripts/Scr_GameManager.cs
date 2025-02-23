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

    public GameObject tank;

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
    public int goldCoinWorth;
    public int fishIdCounter = 0;

    public Dictionary<GameObject, GameObject> foodFishDictionary; //instance is the key, prefab is the value



    // Food List will also contain all fish, as fish can be foods for other fish
    //public List<GameObject> foodList = new List<GameObject>();

    //public Dictionary<string, List<string>> fishDiets;

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

        foodFishDictionary = new Dictionary<GameObject, GameObject>(); //have to instantiate a dictionary for some reason


        if (!tank.activeSelf)
        {
            tank.SetActive(true);

            UpdateText(moneyText, moneyAmount);
            UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
            UpdateText(fishFood_2_AmountText, fishFood_2_Amount);

            tank.SetActive(false);
        }
    }


    public void DropFood(GameObject _foodToDrop)
    {
        if (_foodToDrop != null)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 spawnTank = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);

            float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float screenHeightWorld = Camera.main.orthographicSize * 2;

            float maxX = spawnTank.x + screenWidthWorld / 2;
            float minX = spawnTank.x - screenWidthWorld / 2;

            Vector2 foodSpawnPos;

            //checks if the placed food is outside the screen
            if (mouseWorldPosition.x > maxX)
            {
                foodSpawnPos = new Vector2(maxX, Camera.main.transform.position.y + screenHeightWorld / 2);

            }
            else if (mouseWorldPosition.x < minX)
            {
                foodSpawnPos = new Vector2(minX, Camera.main.transform.position.y + screenHeightWorld / 2);

            }
            else
            {
                foodSpawnPos = new Vector2(mouseWorldPosition.x, Camera.main.transform.position.y + screenHeightWorld / 2);
            }

            GameObject newfoodPellet = Instantiate(_foodToDrop, foodSpawnPos, Quaternion.identity);

            foodFishDictionary.Add(newfoodPellet, _foodToDrop);
            AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newfoodPellet, _foodToDrop);


            SetFishFoodAmount(_foodToDrop, GetFishFoodAmount(_foodToDrop) - 1);

            PlaySoundEffect(SFX_DropFood, 1, 0.5f, 1.5f);


        }
    }
    public void SpawnFish(GameObject _fishToSpawn)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size

        Vector2 spawnPosition = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * _fishToSpawn.GetComponent<Scr_Fish>().spawnHeightPercentage;

        Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


        float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

        spawnPosition.x = randPosX;


        int fishCost = _fishToSpawn.GetComponent<Scr_Fish>().fishCost;
        if (GetMoneyAmount() >= fishCost)
        {
            SetMoneyAmount(GetMoneyAmount() - fishCost);
            GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);

            foodFishDictionary.Add(newFish, _fishToSpawn);

            AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

            PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);
        }
        else
        {
            Debug.Log("Insufficient Money for Fish : $" + fishCost);
        }

    }

    public void AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(GameObject _spawnedFishOrFood, GameObject _spawnedFishOrFoodPrefab)
    {
        foreach ((GameObject fishOrFoodInstance, GameObject fishOrFoodPrefab) in foodFishDictionary) //loops through all fish or food in the scene
        {
            //adds spawned fish or food to all other fish food diets (if in their diet)

            if (fishOrFoodInstance.GetComponent<Scr_Fish>() != null) //if the gameobject in dictionary is a fish
            {
                Scr_Fish existingFishScript = fishOrFoodInstance.GetComponent<Scr_Fish>();

                if (existingFishScript.fishDiet.Contains(_spawnedFishOrFoodPrefab))
                {
                    if (!existingFishScript.foodInScene.Contains(_spawnedFishOrFood))
                        existingFishScript.foodInScene.Add(_spawnedFishOrFood);
                }
            }

            //adds existing fish or food to the spawned fish food diet (if in their diet)

            if (_spawnedFishOrFood.GetComponent<Scr_Fish>() != null) //if the spawned fish or food is a fish
            {
                Scr_Fish spawnedFishScript = _spawnedFishOrFood.GetComponent<Scr_Fish>();

                if (spawnedFishScript.fishDiet.Contains(fishOrFoodPrefab))
                {
                    if (!spawnedFishScript.foodInScene.Contains(fishOrFoodInstance))
                        spawnedFishScript.foodInScene.Add(fishOrFoodInstance);
                }
            }
        }
    }

    public void RemoveFoodFromExistingFishDiets(GameObject _foodToRemove) //removes fish or food from existing fish diets
    {
        foreach ((GameObject fishOrFoodInstance, GameObject fishOrFoodPrefab) in foodFishDictionary)
        {
            if (fishOrFoodInstance.GetComponent<Scr_Fish>() != null) //if the gameobject in dictionary is a fish
            {
                Scr_Fish existingFishScript = fishOrFoodInstance.GetComponent<Scr_Fish>();

                if (existingFishScript.foodInScene.Contains(_foodToRemove))
                {
                    existingFishScript.foodInScene.Remove(_foodToRemove);
                }
            }
        }
    }

    /*
    public void AddThisFishOrFoodToOtherFishFoodLists(GameObject _foodToAdd, GameObject _foodInstance) //adds the spawning fish or food pellet to foodlists of all other fish in the scene
    {
        foreach ((GameObject fishInstance, GameObject fishPrefab) in fishDictionary)
        {
            Scr_Fish fishScript = fishInstance.GetComponent<Scr_Fish>();
            
            if (fishScript.fishDiet.Contains(_foodToAdd))
            {
                fishScript.foodInScene.Add(_foodInstance);
            }
        }
    }
    public void AddOtherFishToThisFishFoodLists(GameObject _fishToSpawn) //adds existing fish to the spawning fish's food list
    {
        foreach ((GameObject fishInstance, GameObject fishPrefab) in fishDictionary)
        {
            Scr_Fish fishScript = _fishToSpawn.GetComponent<Scr_Fish>();

            if (fishScript.fishDiet.Contains(fishPrefab))
            {
                fishScript.foodInScene.Add(fishInstance);
            }
        }
    }
    public void RemoveThisFishOrFoodFromOtherFishFoodLists(GameObject _foodInstance) //removes the food pellet or fish from the foodlists of all other fish in the scene
    {
        foreach ((GameObject fishInstance, GameObject fishPrefab) in fishDictionary)
        {
            Scr_Fish fishScript = fishInstance.GetComponent<Scr_Fish>();
            if (fishScript.foodInScene.Contains(_foodInstance))
            {
                fishScript.foodInScene.Remove(_foodInstance);
            }
        }
    }
    */

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

    public void ChangeColor(GameObject _Object, Color _colorToChange) // Checks ALL children
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
            if (child.gameObject.GetComponent<SpriteRenderer>() != null)
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
            if (child.gameObject.GetComponent<Image>() != null)
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

    public void MoveToScene(Transform transform)
    {
        _Camera.transform.position = new Vector3(transform.position.x, transform.position.y, _Camera.transform.position.z);
    }

    public void EnableUI(GameObject UI)
    {
        UI.SetActive(true);
    }

    public void DisableUI(GameObject UI)
    {
        UI.SetActive(false);
    }
}
