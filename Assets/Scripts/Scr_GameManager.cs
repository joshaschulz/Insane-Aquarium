using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Scr_GameManager : MonoBehaviour
{
    public static Scr_GameManager GMinstance;
    private Camera _Camera;

    public AudioSource AS;
    public Scr_CursorFollower cursorFollower;
    private Scr_SpawnToiletFish Scr_SpawnToiletFish;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    public GameObject rodIdle, rodHooked;

    public GameObject tank;


    public TextMeshProUGUI phoneNumber; //number entered on the phone

    //numbers on the stall
    public string[] phonebook;
    private GameObject tempPhoneAudioSource;


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

    public GameObject fishBag_Button;
    //public List<(GameObject, int)> baggedFish; // Fish Prefab, HungerCount
    public bool canIBagFish;

    public GameObject baggedFish_Button1;
    public GameObject baggedFish_Button2;
    public GameObject baggedFish_Button3;

    // Click Bag button to turn cursor image to bag and allow for capturing of fish with left click.
    // This should also deselect any currently selected fish food to drop.
    // Bagged fish are removed from the tank and any other fishes' diets.
    // Bagged fish do not restore hungry count, that value is stored until unbagged.
    // They are represented as a bagged [fish type] button under the Bag button.
    // Clicking this button will turn cursor image to bagged [fish type] image and allow for releasing of fish with left click.
    // Releasing a fish will remove the bagged [fish type] button.

    // Customers will occasionally request a fish that you have in one of your tanks in return for some money.
    // You can then find and bag the fish.
    // Then clicking on the "checkmark" dialogue option will remove the bagged fish from your inventory, give you the money promised, and cause the customer to leave happily.
    // By clicking on the "x" dialog option, the customer will leave angrily.




    // Food List will also contain all fish, as fish can be foods for other fish
    //public List<GameObject> foodList = new List<GameObject>();

    //public Dictionary<string, List<string>> fishDiets;

    // List of Sounds
    public AudioClip SFX_DropCoin, SFX_DropFish, SFX_DropFood, SFX_FishDeath, SFX_FishEat, SFX_MoneyPickup, SFX_Select, SFX_Error, SFX_Bubbles1, SFX_Bubbles2, SFX_BagFish, SFX_Reeling, SFX_FishHitToilet, SFX_ToiletFlush, SFX_ToiletSplash;
    public AudioClip SFX_Keypad1, SFX_Keypad2, SFX_Keypad3, SFX_Keypad4, SFX_Keypad5, SFX_Keypad6, SFX_Keypad7, SFX_Keypad8, SFX_Keypad9, SFX_Keypad0, SFX_KeypadDel, SFX_KeypadEnter, SFX_CallFail, SFX_CallRinging, SFX_CallHangUp;


    public void ClickKeypad(int key)
    {

        AudioClip keypadPressed = SFX_Keypad1;

        // Handle pitch for all keys
        switch (key)
        {
            case 1: keypadPressed = SFX_Keypad1; break;
            case 2: keypadPressed = SFX_Keypad2; break;
            case 3: keypadPressed = SFX_Keypad3; break;
            case 4: keypadPressed = SFX_Keypad4; break;
            case 5: keypadPressed = SFX_Keypad5; break;
            case 6: keypadPressed = SFX_Keypad6; break;
            case 7: keypadPressed = SFX_Keypad7; break;
            case 8: keypadPressed = SFX_Keypad8; break;
            case 9: keypadPressed = SFX_Keypad9; break;
            case 0: keypadPressed = SFX_Keypad0; break;
            case -1: keypadPressed = SFX_KeypadDel; break;
            case 10: keypadPressed = SFX_KeypadEnter; break;
        }

        // Play the sound (only skip for 999 which has no sound)
        if (key != 999)
        {
            PlaySoundEffect(keypadPressed, 1f, 1);
        }


        // Handle functionality
        if (key >= 0 && key <= 9) //press number
        {
            if (phoneNumber.text.Length < 10)
                phoneNumber.text += key.ToString();
        }
        else if (key == -1) //delete
        {
            if (phoneNumber.text.Length > 0)
                phoneNumber.text = phoneNumber.text.Substring(0, phoneNumber.text.Length - 1);
        }
        else if (key == 10) //enter
        {
            Debug.Log($"Entered Number: {phoneNumber.text}");

            //if phone calling sound already playing, destroy it before playing again
            if (tempPhoneAudioSource != null)
            {
                AudioSource tempAudio = tempPhoneAudioSource.GetComponent<AudioSource>();

                if (tempAudio.isPlaying)
                {
                    Debug.Log("Cancelled");
                    tempAudio.Stop();
                    Destroy(tempPhoneAudioSource);
                }

            }

            string numberToCall = phoneNumber.text;
            phoneNumber.text = "";

            //if number doesnt exist as callable, play call fail sound, otherwise play the ringing sound
            if (phonebook.Contains(numberToCall))
            {
                tempPhoneAudioSource = PlaySoundEffectDontDestroy(SFX_CallRinging, 0.5f, 1);
                Destroy(tempPhoneAudioSource, tempPhoneAudioSource.GetComponent<AudioSource>().clip.length);
            }
            else
            {
                tempPhoneAudioSource = PlaySoundEffectDontDestroy(SFX_CallFail, 2f, 1);
                Destroy(tempPhoneAudioSource, tempPhoneAudioSource.GetComponent<AudioSource>().clip.length);
            }


        }
        else if (key == 999) //used for clearing on enabling/disabling phone
        {
            phoneNumber.text = "";
        }

    }

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

        Scr_SpawnToiletFish = FindObjectOfType<Scr_SpawnToiletFish>();
        Scr_UIElementsHandler = FindObjectOfType<Scr_UIElementsHandler>();


        _Camera = Camera.main;

        foodFishDictionary = new Dictionary<GameObject, GameObject>(); //have to instantiate a dictionary for some reason
        //baggedFish = new List<(GameObject, int)>(); //have to instantiate this thing for some reason

        /*
        if (!tank.activeSelf)
        {
            tank.SetActive(true);

            //UpdateText(moneyText, moneyAmount);
            //UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
            //UpdateText(fishFood_2_AmountText, fishFood_2_Amount);

            tank.SetActive(false);
        }
        */
    }

    private void OnEnable()
    {
        // Optionally, get a reference to the TickHandler (assuming there's only one or it’s a singleton)
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();


        float tickIntervalInMinutes = Scr_TimeHandler.tickInterval / 60;

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    public void OnTickEvent()
    {
        //maybe want tick events on game manager?
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
    public void DropFish()
    {
        GameObject baggedFishButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (baggedFishButton != null && baggedFishButton.transform.childCount != 0)
        {
            if (CheckIfInTank()) //only release fish if in a tank
            {
                GameObject releasedFish = baggedFishButton.transform.GetChild(0).gameObject;
                Scr_Fish releasedFishScript = releasedFish.GetComponent<Scr_Fish>();

                //spawn fish at random x coordinate at same designated y coordinate
                //set the x bounds of where the fish can spawn based on screen size

                Vector2 spawnPosition = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

                float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
                float screenHeightWorld = Camera.main.orthographicSize * 2;

                spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * releasedFishScript.spawnHeight;

                Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


                float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

                spawnPosition.x = randPosX;

                releasedFish.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, releasedFish.transform.position.z);


                // Instead of spawning in a new fish, move this fish to the correct spot
                releasedFish.transform.SetParent(null);

                foodFishDictionary.Add(releasedFish, releasedFishScript.thisPrefab);

                AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(releasedFish, releasedFishScript.thisPrefab);

                PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);

                Debug.Log(releasedFish.name + " was released");


                releasedFishScript.enabled = true;
                releasedFish.GetComponent<CircleCollider2D>().enabled = true;
                releasedFish.transform.localScale = releasedFishScript.originalScale;

                releasedFishScript.Start();

                //baggedFishButton.SetActive(false);
                // DeselectBaggedFish();
            }


        }
    }
    public void SpawnFish(GameObject _fishToSpawn)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size

        Vector2 spawnPosition = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

        //keeps track of number of fish in each tank
        /*if (_Camera.transform.position.x == tank1.position.x)
        {
            tank1Fish++;
        }
        else if (_Camera.transform.position.x == tank2.position.x)
        {
            tank2Fish++;
        }*/

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * _fishToSpawn.GetComponent<Scr_Fish>().spawnHeight;

        Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


        float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

        spawnPosition.x = randPosX;


        int fishCost = _fishToSpawn.GetComponent<Scr_Fish>().fishCost;
        if (GetMoneyAmount() >= fishCost)
        {
            SetMoneyAmount(GetMoneyAmount() - fishCost);
            GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);



            foodFishDictionary.Add(newFish, _fishToSpawn);
            Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();
            newFishScript.thisPrefab = _fishToSpawn;

            if (!newFishScript.grown)
            {
                MakeFishSmaller(newFish); //want to spawn fish as child and then have it grow over time
            }

            AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

            PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);
        }
        else
        {
            Debug.Log("Insufficient Money for Fish : $" + fishCost);
        }

        //Scr_UIElementsHandler.UpdateTankWater();

    }

    public void SpawnFish(GameObject _fishToSpawn, Transform tank) //spawn fish in a tank (don't have to be in the tank to spawn the fish)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size

        Vector2 spawnPosition = new Vector2(tank.transform.position.x, tank.transform.position.y);

        /*if (tank == tank1)
        {
            tank1Fish++;
        }
        else if (tank == tank2)
        {
            tank2Fish++;
        }*/

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * _fishToSpawn.GetComponent<Scr_Fish>().spawnHeight;

        Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


        float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

        spawnPosition.x = randPosX;


        int fishCost = _fishToSpawn.GetComponent<Scr_Fish>().fishCost;
        if (GetMoneyAmount() >= fishCost)
        {
            SetMoneyAmount(GetMoneyAmount() - fishCost);
            GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);

            MakeFishSmaller(newFish); //want to spawn fish as child and then have it grow over time

            foodFishDictionary.Add(newFish, _fishToSpawn);
            Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();
            newFishScript.thisPrefab = _fishToSpawn;


            AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

            PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);
        }
        else
        {
            Debug.Log("Insufficient Money for Fish : $" + fishCost);
        }



        //Scr_UIElementsHandler.UpdateTankWater();

    }

    public void AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(GameObject _spawnedFishOrFood, GameObject _spawnedFishOrFoodPrefab)
    {
        foreach ((GameObject fishOrFoodInstance, GameObject fishOrFoodPrefab) in foodFishDictionary) //loops through all fish or food in the scene
        {
            //adds spawned fish or food to all other fish food diets (if in their diet)

            if (fishOrFoodInstance.GetComponent<Scr_Fish>() != null) //if the gameobject in dictionary is a fish
            {
                Scr_Fish existingFishScript = fishOrFoodInstance.GetComponent<Scr_Fish>();

                Debug.Log(existingFishScript.fishDiet[0] + " compared to " + _spawnedFishOrFoodPrefab);


                if (existingFishScript.fishDiet.Contains(_spawnedFishOrFoodPrefab))
                {
                    //Debug.Log(gameObject + "can eat " + _spawnedFishOrFood);

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

    public void ClearFoodFromFishFoodList(GameObject _fish)
    {
        Scr_Fish fishScript = _fish.GetComponent<Scr_Fish>();

        fishScript.foodInScene.Clear();
    }


    public void BagAFish(GameObject _fishToBag)
    {
        GameObject baggedFishButtonToUse;
        // Check to see if there is at least 1 of 3 bags available
        if (baggedFish_Button1.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button1;
        }
        else if (baggedFish_Button2.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button2;
        }
        else if (baggedFish_Button3.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button3;
        }
        else
        {
            Debug.Log("All fish bags were taken up!");
            // Perhaps disable the button to bag more fish in this case
            return;
        }

        Debug.Log(_fishToBag.name + " was bagged");
        PlaySoundEffect(SFX_BagFish, 1);
        Scr_Fish fishScript = _fishToBag.GetComponent<Scr_Fish>();
        SpawnParticles(fishScript.bubblesEffectPrefab, transform.position, transform.rotation);


        // Teleport the fish to the BaggedFishButton it is to be associated with, deactivate its fish script and other components, make it uneatable

        foodFishDictionary.Remove(_fishToBag);
        RemoveFoodFromExistingFishDiets(_fishToBag);

        //reset fish's food in scene
        ClearFoodFromFishFoodList(_fishToBag);

        baggedFishButtonToUse.SetActive(true);
        _fishToBag.transform.SetParent(baggedFishButtonToUse.transform);

        fishScript.originalScale = _fishToBag.transform.localScale;

        fishScript.SetTarget(_fishToBag.transform.position);
        fishScript.FaceForward();
        fishScript.CancelInvoke();
        fishScript.enabled = false;
        _fishToBag.GetComponent<CircleCollider2D>().enabled = false;


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonToUse.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;
        _fishToBag.transform.localScale /= baggedFishButtonWidth * 12f;

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonToUse.transform.position);
        _fishToBag.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, _fishToBag.transform.position.z);

        // Figure out how to make the bagged fish render in front of the other tank fish and go back to normal upon dropping into tank
    }

    public void BagToiletFish(GameObject _fishToBag)
    {
        GameObject baggedFishButtonToUse;
        // Check to see if there is at least 1 of 3 bags available
        if (baggedFish_Button1.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button1;
        }
        else if (baggedFish_Button2.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button2;
        }
        else if (baggedFish_Button3.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button3;
        }
        else
        {
            Debug.Log("All fish bags were taken up!");
            // Perhaps disable the button to bag more fish in this case
            return;
        }

        Debug.Log(_fishToBag.name + " was bagged");
        PlaySoundEffect(SFX_BagFish, 1);
        Scr_Fish fishScript = _fishToBag.GetComponent<Scr_Fish>();
        fishScript.enabled = true;


        baggedFishButtonToUse.SetActive(true);
        _fishToBag.transform.SetParent(baggedFishButtonToUse.transform);


        fishScript.SetTarget(_fishToBag.transform.position);
        fishScript.FaceForward();
        fishScript.CancelInvoke();
        fishScript.enabled = false;
        _fishToBag.GetComponent<CircleCollider2D>().enabled = false;


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonToUse.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;
        _fishToBag.transform.localScale /= baggedFishButtonWidth * 12f;

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonToUse.transform.position);
        _fishToBag.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, _fishToBag.transform.position.z);

        // Figure out how to make the bagged fish render in front of the other tank fish and go back to normal upon dropping into tank
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
        PlaySoundEffect(_soundEffect, _volumeScale, 1, 1);
    }
    public void PlaySoundEffect(AudioClip _soundEffect, float _volumeScale, float _pitch)
    {
        PlaySoundEffect(_soundEffect, _volumeScale, _pitch, _pitch);
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


    public GameObject PlaySoundEffectDontDestroy(AudioClip _soundEffect, float _volumeScale)
    {
        // This function plays a sound effect and does not destroy the audio source object afterwards, instead it returns it
        return PlaySoundEffectDontDestroy(_soundEffect, _volumeScale, 1, 1);
    }
    public GameObject PlaySoundEffectDontDestroy(AudioClip _soundEffect, float _volumeScale, float _pitch)
    {
        // This function plays a sound effect and does not destroy the audio source object afterwards, instead it returns it
        return PlaySoundEffectDontDestroy(_soundEffect, _volumeScale, _pitch, _pitch);
    }
    public GameObject PlaySoundEffectDontDestroy(AudioClip _soundEffect, float _volumeScale, float _lowerPitch, float _upperPitch)
    {
        // This function plays a sound effect and does not destroy the audio source object afterwards, instead it returns it

        float newPitch = Random.Range(_lowerPitch, _upperPitch);
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.SetParent(null);
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioSource.clip = _soundEffect;
        tempAudioSource.volume = _volumeScale;
        tempAudioSource.pitch = newPitch;
        tempAudioSource.Play();

        //AS.PlayOneShot(_soundEffect, _volumeScale);
        return tempAudioObject;
    }

    IEnumerator ResetPitchAfterDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        AS.pitch = 1.0f;
    }

    public void ChangeFishFoodTypeToDrop(GameObject _fishFoodType)
    {
        // If the fish bag was selected and a food is clicked
        if (_fishFoodType != null && canIBagFish)
        {
            DeselectFishBag();
        }
        // If a bagged fish was selected and a food is clicked
        /*
        if (_fishFoodType != null)
        {
            DeselectBaggedFish();
        }
        */

        if (_fishFoodType == null)
        {
            currentFishFoodSelected = null;
            PlaySoundEffect(SFX_Select, 0.7f, 0.8f);
            ChangeCursorFollower(null);
            currentFishFoodButtonSelected = null;
        }
        else if (_fishFoodType == fishFood_1_Prefab)
        {
            currentFishFoodSelected = fishFood_1_Prefab;
            PlaySoundEffect(SFX_Select, 0.7f);
            ChangeCursorFollower(fishFood_1_Button.GetComponent<Image>().sprite);
            currentFishFoodButtonSelected = fishFood_1_Button;
        }
        else if (_fishFoodType == fishFood_2_Prefab)
        {
            currentFishFoodSelected = fishFood_2_Prefab;
            PlaySoundEffect(SFX_Select, 0.7f);
            ChangeCursorFollower(fishFood_2_Button.GetComponent<Image>().sprite);
            currentFishFoodButtonSelected = fishFood_2_Button;
        }
    }
    /*
    public void SelectBaggedFish(GameObject _baggedFishButton)
    {
        // If the fish bag is currently selected, remove it
        if (canIBagFish)
        {
            DeselectFishBag();
        }

        // If a fish food is currently selected, remove it
        if (currentFishFoodSelected != null)
        {
            ChangeFishFoodTypeToDrop(null);
        }

<<<<<<< HEAD
=======
        // Change the cursor image to a fish bag image and play a sound effect

        GameObject baggedFishButtonCopy = Instantiate(_baggedFishButton, cursorFollower.transform);

        GameObject baggedFishCopy = baggedFishButtonCopy.transform.GetChild(0).gameObject;

        //Scr_Fish baggedFishCopyScript = baggedFishCopy.GetComponent<Scr_Fish>();

        baggedFishCopy.transform.SetParent(baggedFishButtonCopy.transform);


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonCopy.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;

        Debug.Log(baggedFishButtonWidth);

        baggedFishCopy.transform.localScale = new Vector3(baggedFishButtonCopy.GetComponent<RectTransform>().rect.width, baggedFishButtonCopy.GetComponent<RectTransform>().rect.height, baggedFishCopy.transform.localScale.z);

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonCopy.transform.position);
        baggedFishCopy.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, baggedFishCopy.transform.position.z);


        baggedFishButtonCopy.GetComponent<Button>().enabled = false;
        baggedFishButtonCopy.GetComponent<Image>().raycastTarget = false;
        baggedFishButtonCopy.transform.localPosition = Vector3.zero;
        baggedFishButtonCopy.transform.GetChild(0).gameObject.transform.localPosition = Vector3.zero;

        cursorFollower.gameObject.SetActive(true);
        cursorFollower.GetComponent<Image>().enabled = false;



>>>>>>> e35f5737fab0f5df1b478f6eaed454b47707d761
        PlaySoundEffect(SFX_Select, 0.7f);

        // Set a state where clicking on the tank will drop the fish childed to this bagged fish button
        //fishToDrop = _baggedFishButton.transform.GetChild(0).gameObject;
        currentBaggedFishButtonSelected = _baggedFishButton;
    }

    public void DeselectBaggedFish()
    {
<<<<<<< HEAD
=======
        // Change the cursor image to nothing and play a sound effect
        ChangeCursorFollower(null);
        cursorFollower.GetComponent<Image>().enabled = true;
        Destroy(cursorFollower.transform.GetChild(0).gameObject);

>>>>>>> e35f5737fab0f5df1b478f6eaed454b47707d761
        PlaySoundEffect(SFX_Select, 0.7f, 0.8f);

        currentBaggedFishButtonSelected = null;
    }
        */

    public void SelectFishBag()
    {
        // If a fish food is currently selected, remove it
        if (currentFishFoodSelected != null)
        {
            ChangeFishFoodTypeToDrop(null);
        }

        /*
        // If a bagged fish is currently selected, remove it
        if (currentBaggedFishButtonSelected != null)
        {
            DeselectBaggedFish();
        }
        */

        // Change the cursor image to a fish bag image and play a sound effect
        ChangeCursorFollower(fishBag_Button.GetComponent<Image>().sprite);
        PlaySoundEffect(SFX_Select, 0.7f);

        // Set a state where clicking on a fish will remove it from the scene and add it to baggedFish
        canIBagFish = true;
    }
    public void DeselectFishBag()
    {
        // Change the cursor image to nothing and play a sound effect
        ChangeCursorFollower(null);
        PlaySoundEffect(SFX_Select, 0.7f, 0.8f);

        // Set a state where clicking on a fish will NOT remove it from the scene and add it to baggedFish
        canIBagFish = false;
    }

    public void ChangeCursorFollower(Sprite _cursorSprite)
    {
        if (_cursorSprite != null)
        {
            cursorFollower.gameObject.SetActive(true);
            cursorFollower.GetComponent<Image>().sprite = _cursorSprite;
        }
        else
        {
            cursorFollower.GetComponent<Image>().sprite = null;
            cursorFollower.gameObject.SetActive(false);
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

        // Deselect any currently selected fish food or bagging state
        if (currentFishFoodSelected != null)
        {
            ChangeFishFoodTypeToDrop(null);
        }
        if (canIBagFish)
        {
            DeselectFishBag();
        }
    }

    public void EnableElement(GameObject _element)
    {
        _element.SetActive(true);
    }

    public void DisableElement(GameObject _element)
    {
        _element.SetActive(false);
    }

    public void RodToDisplay()
    {
        rodIdle.SetActive(!Scr_SpawnToiletFish.shouldSpawn);
        rodHooked.SetActive(Scr_SpawnToiletFish.shouldSpawn);
    }

    public void UpdateSceneTexts()
    {
        UpdateText(moneyText, moneyAmount);
        UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
        UpdateText(fishFood_2_AmountText, fishFood_2_Amount);

        // Add more text boxes and values as we make them...
    }

    public int CheckIfTankHasFish(Transform tank)
    {
        int tankFish = 0;

        foreach (var fish in foodFishDictionary)
        {
            GameObject fishObject = fish.Key;
            var fishScript = fishObject.GetComponent<Scr_Fish>();


            if (fishScript != null)
            {
                if (fishScript.spawnTank == new Vector2(tank.position.x, tank.position.y))
                {
                    tankFish++;
                }
            }
        }

        return tankFish;
    }

    public bool CheckIfInTank()
    {
        Vector2 cameraPos = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

        return Scr_UIElementsHandler.tankPositions.Contains(cameraPos);

    }

    public void MakeFishSmaller(GameObject fish)
    {
        fish.transform.localScale = new Vector3(fish.transform.localScale.x / 2, fish.transform.localScale.y / 2, fish.transform.localScale.z); //child is half size as adult
    }

    public void MakeFishBigger(GameObject fish)
    {
        fish.transform.localScale = new Vector3(fish.transform.localScale.x * 2, fish.transform.localScale.y * 2, fish.transform.localScale.z); //child is half size as adult
    }

    public IEnumerator RecenterThenUnlock()
    {
        // 0) Make sure we're starting from a clean state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        yield return null;                         // give Unity a frame to apply state

        // 1) Lock (Unity will snap to center on the *next* frame)
        Cursor.lockState = CursorLockMode.Locked;

        // 2) Wait until end of frame, then one more frame (robust across platforms)
        yield return new WaitForEndOfFrame();
        yield return null;

        // 3) Unlock — position stays at the center
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;                     // optional

        Debug.Log("Done with recentering cursor!");
    }
}
