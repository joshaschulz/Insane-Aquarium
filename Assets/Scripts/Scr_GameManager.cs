using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class Scr_GameManager : MonoBehaviour
{
    public static Scr_GameManager GMinstance;
    private Camera _Camera;

    //Scriptable Objects game settings
    public static Scr_GameSettings ActiveSettings { get; private set; }

    [Header("Mode")]
    public bool useTestSettings = false;

    [Header("Settings Profiles")]
    public Scr_GameSettings buildSettings;
    public Scr_GameSettings testSettings;

    private int fastForwardSetting = 1;


    public AudioSource AS;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot underwaterSnapshot;
    public Scr_CursorFollower cursorFollower;
    private Scr_SpawnToiletFish Scr_SpawnToiletFish;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;
    private Scr_FishyGuy Scr_FishyGuy;
    private Scr_Customer Scr_Customer;

    public GameObject rodIdle, rodHooked;

    public GameObject tank;

    public float foregroundTankPoopLevel = 0;
    public float backgroundTankPoopLevel = 0;
    public GameObject foregroundTankPoopOverlay;
    public GameObject backgroundTankPoopOverlay;

    public TextMeshProUGUI phoneNumber; //number entered on the phone

    //numbers on the stall
    private GameObject tempPhoneAudioSource;
    public Scr_PhoneContact[] contacts;
    public string currentlyCalling = "";

    public Scr_Dialogue dialogueBox;

    public float radiationHueShift;

    public int moneyAmount = 0;
    public TextMeshProUGUI moneyText;

    public int fishFood_1_Amount;
    public TextMeshProUGUI fishFood_1_AmountText;
    public int fishFood_2_Amount;
    public TextMeshProUGUI fishFood_2_AmountText;
    public int fishFood_3_Amount;
    public TextMeshProUGUI fishFood_3_AmountText;

    public GameObject fishFood_1_Button;
    public GameObject fishFood_2_Button;
    public GameObject fishFood_3_Button;
    public GameObject currentFishFoodButtonSelected;

    public GameObject fishFood_1_Prefab;
    public GameObject fishFood_2_Prefab;
    public GameObject fishFood_3_Prefab;
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

    public GameObject baggedFish_Socket1;
    public GameObject baggedFish_Socket2;
    public GameObject baggedFish_Socket3;

    public GameObject[] baggedFishSockets;

    public GameObject[] fishPrefabs;
    public Sprite[] fishSprites;

    public Transform foregroundTank;
    public Transform backgroundTank;
    public SpriteRenderer[] tankSpriteRenderers;



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
    public AudioClip SFX_DropCoin, SFX_DropFish, SFX_DropFood, SFX_FishDeath, SFX_FishEat, SFX_MoneyPickup, SFX_Select, SFX_Error, SFX_Bubbles1, SFX_Bubbles2, SFX_BagFish, SFX_Reeling, SFX_FishHitToilet, SFX_ToiletSplash;
    public AudioClip SFX_Bag, SFX_CashRegister, SFX_FishHooked, SFX_FlipPhoneHigh, SFX_FlipPhoneLow, SFX_Flush, SFX_GenUI1, SFX_GenUI2, SFX_GenUI3, SFX_LineSnap, SFX_MoneyCounter, SFX_Pop, SFX_Snap, SFX_TextScroll, SFX_TextScrollEnd, SFX_FishGrow, SFX_Fart1, SFX_Fart2;
    public AudioClip SFX_Keypad1, SFX_Keypad2, SFX_Keypad3, SFX_Keypad4, SFX_Keypad5, SFX_Keypad6, SFX_Keypad7, SFX_Keypad8, SFX_Keypad9, SFX_Keypad0, SFX_KeypadDel, SFX_KeypadEnter, SFX_CallFail, SFX_CallRinging, SFX_CallHangUp;


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

        fishPrefabs = Resources.LoadAll<GameObject>("Prefabs/Fish");
        fishSprites = Resources.LoadAll<Sprite>("Fish");

        //Debug.Log("THIS NUMBER OF FISH IMAGES: " + fishSprites.Length);


        baggedFishSockets = new GameObject[] {baggedFish_Socket1, baggedFish_Socket2, baggedFish_Socket3 };

        ActiveSettings = useTestSettings ? testSettings : buildSettings;
        ChangeGameSettings();

        UpdateSceneTexts();


        //baggedFish = new List<(GameObject, int)>(); //have to instantiate this thing for some reason

        /*
        if (!tank.activeSelf)
        {
            tank.SetActive(true);

            //UpdateText(moneyText, moneyAmount);
            //UpdateText(fishFood_1_AmountText, fishFood_1_Amount);
            //UpdateText(fishFood_2_AmountText, fishFood_2_Amount);
            // Add more fish food types here...

            tank.SetActive(false);
        }
        */
    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = ActiveSettings;

        moneyAmount = settings.moneyAmount;
        Debug.Log("MONEY AMOUNT: "+ settings.moneyAmount);
        fishFood_1_Amount = settings.fishFood1Amount;
        fishFood_2_Amount = settings.fishFood2Amount;
        fishFood_3_Amount = settings.fishFood3Amount;
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
            PlaySoundEffect(SFX_Pop, 0.05f, 0.8f, 1.2f);


        }
    }
    public void DropFish()
    {
        GameObject baggedFishButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        GameObject baggedFishButtonToUse = null;
        GameObject baggedFishSocketToUse = null;

        if (baggedFishButton != null)
        {
            if (baggedFishButton == baggedFish_Button1)
            {
                baggedFishButtonToUse = baggedFish_Button1;
                baggedFishSocketToUse = baggedFish_Socket1;
            }
            else if (baggedFishButton == baggedFish_Button2)
            {
                baggedFishButtonToUse = baggedFish_Button2;
                baggedFishSocketToUse = baggedFish_Socket2;
            }
            else if (baggedFishButton == baggedFish_Button3)
            {
                baggedFishButtonToUse = baggedFish_Button3;
                baggedFishSocketToUse = baggedFish_Socket3;
            }
            else
            {
                Debug.Log("No Button selected");
            }
        }


        if (baggedFishButton != null && baggedFishSocketToUse.transform.childCount != 0)
        {
            if (CheckIfInTank()) //only release fish if in a tank
            {
                GameObject releasedFish = baggedFishSocketToUse.transform.GetChild(0).gameObject;
                Scr_Fish releasedFishScript = releasedFish.GetComponent<Scr_Fish>();
                Scr_FishAnimation releasedFishAnimScript = releasedFish.GetComponent<Scr_FishAnimation>();

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
                releasedFishScript.enabled = true;


                ShowHideFishBags();

                foodFishDictionary.Add(releasedFish, releasedFishScript.thisPrefab);


                AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(releasedFish, releasedFishScript.thisPrefab);

                PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);

                Debug.Log(releasedFish.name + " was released");


                releasedFish.GetComponent<CircleCollider2D>().enabled = true;
                releasedFish.transform.localScale = releasedFishScript.originalScale;

                SetSortingGroupToLayer(releasedFish, "Game Objects");

                releasedFishScript.Start();
                releasedFishAnimScript.Awake();

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


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * _fishToSpawn.GetComponent<Scr_Fish>().spawnHeight;

        Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


        float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

        spawnPosition.x = randPosX;

        GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);



        foodFishDictionary.Add(newFish, _fishToSpawn);
        Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();
        newFishScript.thisPrefab = _fishToSpawn;

        newFishScript.ChangeGameSettings();
        newFishScript.GenerateRandomStats();

        if (!newFishScript.grown)
        {
            MakeFishSmaller(newFish); //want to spawn fish as child and then have it grow over time
        }

        AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

        PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);

        //Scr_UIElementsHandler.UpdateTankWater();

    }

    public void SpawnFish(GameObject _fishToSpawn, Transform _pos)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size

        Vector2 spawnPosition = new Vector2(_pos.position.x, _pos.position.y);


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        spawnPosition.y = (spawnPosition.y - screenHeightWorld / 2) + screenHeightWorld * _fishToSpawn.GetComponent<Scr_Fish>().spawnHeight;

        Vector2 randomSpawnBounds = new Vector2(spawnPosition.x - screenWidthWorld / 2, spawnPosition.x + screenWidthWorld / 2);


        float randPosX = Random.Range(randomSpawnBounds.x, randomSpawnBounds.y);

        spawnPosition.x = randPosX;

        GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);



        foodFishDictionary.Add(newFish, _fishToSpawn);
        Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();
        newFishScript.thisPrefab = _fishToSpawn;

        newFishScript.ChangeGameSettings();
        newFishScript.GenerateRandomStats();


        if (!newFishScript.grown)
        {
            MakeFishSmaller(newFish); //want to spawn fish as child and then have it grow over time
        }

        AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

        PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);

        //Scr_UIElementsHandler.UpdateTankWater();

    }

    public GameObject SpawnBoughtFish(GameObject _fishToSpawn, Transform _pos)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size

        Vector2 spawnPosition = new Vector2(_pos.position.x, _pos.position.y);

        GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);

        Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();
        newFishScript.thisPrefab = _fishToSpawn;
        newFishScript.ChangeGameSettings();
        newFishScript.GenerateRandomStats();

        if (!newFishScript.grown)
        {
            MakeFishSmaller(newFish); //want to spawn fish as child and then have it grow over time
        }

        return newFish;
    }

    public GameObject SpawnBabyFish(GameObject _fishToSpawn, GameObject _parentFish)
    {
        //spawn fish at random x coordinate at same designated y coordinate
        //set the x bounds of where the fish can spawn based on screen size
        Scr_Fish parentFishScr = _parentFish.GetComponent<Scr_Fish>();
        Vector2 spawnPosition;
        GameObject newFish;

        if (_parentFish.transform.position.y - 2 < _parentFish.GetComponent<Scr_Fish>().minY)
        {
            spawnPosition = new Vector2(_parentFish.transform.position.x, _parentFish.transform.position.y);
            newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);
            newFish.GetComponent<Scr_FishAnimation>().frontAnimator.Rebind();
            newFish.GetComponent<Scr_FishAnimation>().frontAnimator.Update(0f);
        }
        else
        {
            spawnPosition = new Vector2(_parentFish.transform.position.x, _parentFish.transform.position.y - 2);
            newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);
        }


        MakeFishSmaller(newFish);

        Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();


        foodFishDictionary.Add(newFish, _fishToSpawn);
        newFishScript.thisPrefab = _fishToSpawn;

        newFishScript.grown = false;


        float hueForBabyFish = parentFishScr.GetComponent<Scr_FishHue>().GetHue();
        if (parentFishScr.radiated)
        {
            int sign = (Random.Range(0, 2) == 0) ? -1 : 1;
            hueForBabyFish += sign * radiationHueShift;
        }
        newFishScript.GetComponent<Scr_FishHue>().SetHue(hueForBabyFish);


        AddFoodToSpawnedFishDietAndSpawnedFishToExistingFishDiets(newFish, _fishToSpawn);

        PlaySoundEffect(SFX_DropFish, 1, 0.5f, 1.5f);

        //Scr_UIElementsHandler.UpdateTankWater();

        return newFish;
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
                    if (existingFishScript.FindClosestFood() == null)
                    {
                        existingFishScript.IdleOrMove();
                    }
                }
            }
        }
    }

    public void ClearFoodFromFishFoodList(GameObject _fish)
    {
        Scr_Fish fishScript = _fish.GetComponent<Scr_Fish>();

        fishScript.foodInScene.Clear();
    }


    public void FreakyFishReset(GameObject _fishThatFreaked) // Reseting all freaky fish to Idle or Move when there are no more available fish to freak on
    {
        foreach ((GameObject fishOrFoodInstance, GameObject fishOrFoodPrefab) in foodFishDictionary)
        {
            if (fishOrFoodInstance.GetComponent<Scr_Fish>() != null) //if the gameobject in dictionary is a fish
            {
                Scr_Fish existingFishScript = fishOrFoodInstance.GetComponent<Scr_Fish>();

                if (existingFishScript.isFreaky && fishOrFoodInstance.CompareTag(_fishThatFreaked.tag))
                {
                    if (existingFishScript.FindClosestMate() == null)
                    {
                        existingFishScript.IdleOrMove();
                    }
                }
            }
        }
    }



    public void BagAFish(GameObject _fishToBag)
    {
        GameObject baggedFishButtonToUse;
        GameObject baggedFishSocketToUse;
        // Check to see if there is at least 1 of 3 bags available


        if (baggedFish_Socket1.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button1;
            baggedFishSocketToUse = baggedFish_Socket1;
        }
        else if (baggedFish_Socket2.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button2;
            baggedFishSocketToUse = baggedFish_Socket2;
        }
        else if (baggedFish_Socket3.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button3;
            baggedFishSocketToUse = baggedFish_Socket3;
        }
        else
        {
            Debug.Log("All fish bags were taken up!");
            // Perhaps disable the button to bag more fish in this case
            return;
        }

        ShowHideFishBags();

        Debug.Log(_fishToBag.name + " was bagged");
        PlaySoundEffect(SFX_BagFish, 1);
        PlaySoundEffect(SFX_Bag, 1);
        Scr_Fish fishScript = _fishToBag.GetComponent<Scr_Fish>();
        Scr_FishAnimation fishAnimScript = _fishToBag.GetComponent<Scr_FishAnimation>();
        SpawnParticles(fishScript.bubblesEffectPrefab, transform.position, transform.rotation, null);


        // Teleport the fish to the BaggedFishButton it is to be associated with, deactivate its fish script and other components, make it uneatable

        foodFishDictionary.Remove(_fishToBag);
        RemoveFoodFromExistingFishDiets(_fishToBag);

        //reset fish's food in scene
        ClearFoodFromFishFoodList(_fishToBag);

        baggedFishButtonToUse.SetActive(true);
        //_fishToBag.transform.SetParent(baggedFishButtonToUse.transform);
        _fishToBag.transform.SetParent(baggedFishSocketToUse.transform);

        fishScript.originalScale = _fishToBag.transform.localScale;

        SetSortingGroupToLayer(_fishToBag, "UI");


        fishScript.SetTarget(_fishToBag.transform.position);
        fishAnimScript.SetState(Scr_FishAnimation.FishState.Idle);
        fishScript.CancelInvoke();
        fishScript.enabled = false;
        _fishToBag.GetComponent<CircleCollider2D>().enabled = false;

        FindObjectOfType<Scr_FishInfoPanel>().HideIfFish(fishScript);



        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonToUse.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;
        _fishToBag.transform.localScale /= baggedFishButtonWidth * 4f;

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonToUse.transform.position);
        _fishToBag.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, _fishToBag.transform.position.z);

        // Figure out how to make the bagged fish render in front of the other tank fish and go back to normal upon dropping into tank
    }

    public void BagToiletFish(GameObject _fishToBag)
    {
        GameObject baggedFishButtonToUse;
        GameObject baggedFishSocketToUse;
        // Check to see if there is at least 1 of 3 bags available
        if (baggedFish_Socket1.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button1;
            baggedFishSocketToUse = baggedFish_Socket1;
        }
        else if (baggedFish_Socket2.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button2;
            baggedFishSocketToUse = baggedFish_Socket2;
        }
        else if (baggedFish_Socket3.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button3;
            baggedFishSocketToUse = baggedFish_Socket3;
        }
        else
        {
            Debug.Log("All fish bags were taken up!");
            // Perhaps disable the button to bag more fish in this case
            Destroy(_fishToBag);
            return;
        }

        ShowHideFishBags();

        Debug.Log(_fishToBag.name + " was bagged");
        PlaySoundEffect(SFX_BagFish, 1);
        PlaySoundEffect(SFX_Bag, 1);
        Scr_Fish fishScript = _fishToBag.GetComponent<Scr_Fish>();
        Scr_FishAnimation fishAnimScript = _fishToBag.GetComponent<Scr_FishAnimation>();
        fishScript.enabled = true;


        baggedFishButtonToUse.SetActive(true);
        _fishToBag.transform.SetParent(baggedFishSocketToUse.transform);


        fishScript.SetTarget(_fishToBag.transform.position);
        fishAnimScript.SetState(Scr_FishAnimation.FishState.Idle);
        fishScript.CancelInvoke();

        _fishToBag.transform.localEulerAngles = Vector3.zero;
        var s = _fishToBag.transform.localScale;
        _fishToBag.transform.localScale = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));

        fishScript.originalScale = _fishToBag.transform.localScale;

        fishScript.grown = true;

        fishScript.ChangeGameSettings();
        fishScript.GenerateRandomStats();

        fishScript.enabled = false;
        _fishToBag.GetComponent<CircleCollider2D>().enabled = false;


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonToUse.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;
        _fishToBag.transform.localScale /= baggedFishButtonWidth * 4f;

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonToUse.transform.position);
        _fishToBag.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, _fishToBag.transform.position.z);

        // Figure out how to make the bagged fish render in front of the other tank fish and go back to normal upon dropping into tank
    }

    public bool BagFishyGuyFish(GameObject _fishToBag)
    {
        GameObject baggedFishButtonToUse;
        GameObject baggedFishSocketToUse;
        // Check to see if there is at least 1 of 3 bags available
        if (baggedFish_Socket1.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button1;
            baggedFishSocketToUse = baggedFish_Socket1;
        }
        else if (baggedFish_Socket2.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button2;
            baggedFishSocketToUse = baggedFish_Socket2;
        }
        else if (baggedFish_Socket3.transform.childCount == 0)
        {
            baggedFishButtonToUse = baggedFish_Button3;
            baggedFishSocketToUse = baggedFish_Socket3;
        }
        else
        {
            Debug.Log("All fish bags were taken up!");
            // Perhaps disable the button to bag more fish in this case
            return false;
        }

        ShowHideFishBags();

        Debug.Log(_fishToBag.name + " was bagged");
        Scr_Fish fishScript = _fishToBag.GetComponent<Scr_Fish>();
        Scr_FishAnimation fishAnimScript = _fishToBag.GetComponent<Scr_FishAnimation>();
        fishAnimScript.frontAnimator.Rebind();
        fishAnimScript.frontAnimator.Update(0f);

        fishScript.enabled = true;


        baggedFishButtonToUse.SetActive(true);
        _fishToBag.transform.SetParent(baggedFishSocketToUse.transform);


        fishScript.SetTarget(_fishToBag.transform.position);
        fishAnimScript.SetState(Scr_FishAnimation.FishState.Idle);
        fishScript.CancelInvoke();

        _fishToBag.transform.localEulerAngles = Vector3.zero;
        var s = _fishToBag.transform.localScale;
        _fishToBag.transform.localScale = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));

        fishScript.originalScale = _fishToBag.transform.localScale;

        fishScript.grown = true;

        fishScript.GenerateRandomStats();


        fishScript.enabled = false;
        _fishToBag.GetComponent<CircleCollider2D>().enabled = false;


        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        float baggedFishButtonWidth = (baggedFishButtonToUse.GetComponent<RectTransform>().rect.width / Camera.main.pixelWidth) * screenWidthWorld;
        _fishToBag.transform.localScale /= baggedFishButtonWidth * 4f;

        // Move the fish to the position where the fishbag button appears to be in the world
        Vector3 baggedFishButtonPosition = Camera.main.ScreenToWorldPoint(baggedFishButtonToUse.transform.position);
        _fishToBag.transform.position = new Vector3(baggedFishButtonPosition.x, baggedFishButtonPosition.y - 0.2f, _fishToBag.transform.position.z);

        // Figure out how to make the bagged fish render in front of the other tank fish and go back to normal upon dropping into tank
        return true;
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
    public void AddMoneyAmount(int _moneyToAdd)
    {
        moneyAmount += _moneyToAdd;
        UpdateText(moneyText, moneyAmount);
    }
    public void SubtractMoneyAmount(int _moneyToSubtract)
    {
        moneyAmount -= _moneyToSubtract;
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
        else if (_fishFoodType == fishFood_3_Prefab)
        {
            return fishFood_3_Amount;
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
        else if (_fishFoodType == fishFood_3_Prefab)
        {
            fishFood_3_Amount = _newFishFoodAmount;
            UpdateText(fishFood_3_AmountText, fishFood_3_Amount);
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


    public void EnableUnderwaterAudio()
    {
        underwaterSnapshot.TransitionTo(0.3f);
        Debug.Log("underwater sounds");
    }
    public void DisableUnderwaterAudio()
    {
        normalSnapshot.TransitionTo(0.3f);
        Debug.Log("normal sounds");
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
        else if (_fishFoodType == fishFood_3_Prefab)
        {
            currentFishFoodSelected = fishFood_3_Prefab;
            PlaySoundEffect(SFX_Select, 0.7f);
            ChangeCursorFollower(fishFood_3_Button.GetComponent<Image>().sprite);
            currentFishFoodButtonSelected = fishFood_3_Button;
        }
    }
    

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
    public void UpdatePoopLevel(Vector2 _tank, int _poopAmount)
    {

        if (_tank == new Vector2(foregroundTank.position.x, foregroundTank.position.y))
        {
            foregroundTankPoopLevel = Mathf.Clamp(foregroundTankPoopLevel + _poopAmount, 0, 100);

            SpriteRenderer foregroundTankPoopOverlaySR = foregroundTankPoopOverlay.GetComponent<SpriteRenderer>();

            Color c = foregroundTankPoopOverlaySR.color;
            c.a = Mathf.Clamp01(foregroundTankPoopLevel / 100);
            foregroundTankPoopOverlaySR.color = c;
        }
        else if (_tank == new Vector2(backgroundTank.position.x, backgroundTank.position.y))
        {
            backgroundTankPoopLevel = Mathf.Clamp(backgroundTankPoopLevel + _poopAmount, 0, 100);

            SpriteRenderer backgroundTankPoopOverlaySR = backgroundTankPoopOverlay.GetComponent<SpriteRenderer>();

            Color c = backgroundTankPoopOverlaySR.color;
            c.a = Mathf.Clamp01(backgroundTankPoopLevel / 100);
            backgroundTankPoopOverlaySR.color = c;
        }
    }


    public void SpawnParticles(GameObject _particles, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        GameObject newParticlesObject;
        if (_parent)
        {
            newParticlesObject = Instantiate(_particles, _position, _rotation, _parent);
        }
        else
        {
            newParticlesObject = Instantiate(_particles, _position, _rotation);
        }

        ParticleSystem newParticleSystem = newParticlesObject.GetComponent<ParticleSystem>();
        newParticleSystem.Play();


        if (!newParticleSystem.main.loop)
        {
            float totalLifetime = newParticleSystem.main.duration + newParticleSystem.main.startLifetime.constantMax;
            Destroy(newParticlesObject, totalLifetime);
        }
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

    public void FlashColor(GameObject _object, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        StartCoroutine(FlashColorCoroutine(_object, _colorToChange, _flashTime, _flashInterval));
    }

    private IEnumerator FlashColorCoroutine(GameObject _object, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        float elapsedTime = 0f;
        Color originalColor = Color.white; // Assuming the default color is white.

        while (elapsedTime < _flashTime)
        {
            ChangeColor(_object, _colorToChange);
            yield return new WaitForSeconds(_flashInterval);
            elapsedTime += _flashInterval;

            ChangeColor(_object, originalColor);
            yield return new WaitForSeconds(_flashInterval);
            elapsedTime += _flashInterval;
        }
    }

    public void FlashTextColor(TextMeshProUGUI _textObject, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        StartCoroutine(FlashTextColorCoroutine(_textObject, _colorToChange, _flashTime, _flashInterval));
    }
    private IEnumerator FlashTextColorCoroutine(TextMeshProUGUI _textObject, Color _colorToChange, float _flashTime, float _flashInterval)
    {
        float elapsedTime = 0f;
        Color originalColor = Color.white;

        while (elapsedTime < _flashTime)
        {
            _textObject.color = _colorToChange;
            yield return new WaitForSeconds(_flashInterval);
            elapsedTime += _flashInterval;

            _textObject.color = originalColor;
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

    public void EnableButton(Button _button)
    {
        _button.interactable = true;
    }
    public void DisableButton(Button _button)
    {
        _button.interactable = false;
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
        UpdateText(fishFood_3_AmountText, fishFood_3_Amount);

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

    public void SetSortingGroupToLayer(GameObject _Object, string _SortingLayerName)
    {
        if (SortingLayer.NameToID(_SortingLayerName) != 0)
        {
            if (_Object.GetComponent<SortingGroup>())
            {
                _Object.GetComponent<SortingGroup>().sortingLayerName = _SortingLayerName;
            }

            foreach (Transform child in _Object.transform)
            {
                if (child.gameObject.GetComponent<SortingGroup>())
                {
                    child.gameObject.GetComponent<SortingGroup>().sortingLayerName = _SortingLayerName;
                }

                if (child.childCount > 0)
                {
                    SetSortingGroupToLayer(child.gameObject, _SortingLayerName);
                }
            }
        }
        else
        {
            Debug.LogError("Sorting Layer Invalid!");
        }
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

    public void ShowHideFishBags()
    {
        //GameObject baggedFishButtonToUse;
        //GameObject baggedFishSocketToUse;

        // Check to see if there is at least 1 of 3 bags available
        if (baggedFish_Socket1.transform.childCount == 0)
        {
            baggedFish_Button1.SetActive(false);
        }
        else
        {
            baggedFish_Button1.SetActive(true);
        }

        if (baggedFish_Socket2.transform.childCount == 0)
        {
            baggedFish_Button2.SetActive(false);
        }
        else
        {
            baggedFish_Button2.SetActive(true);
        }

        if (baggedFish_Socket3.transform.childCount == 0)
        {
            baggedFish_Button3.SetActive(false);
        }
        else
        {
            baggedFish_Button3.SetActive(true);
        }
    }

    public Vector2 GetTankPos(Transform pos)
    {
        foreach (SpriteRenderer tank in tankSpriteRenderers)
        {
            if (tank.bounds.Contains(pos.position))
            {
                return new Vector2(tank.transform.position.x, tank.transform.position.y);
            }
        }

        return new Vector2();
    }

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


        // NEW PURCHASING CODE
        if (currentlyCalling == "The Hungry Guppy")
        {
            if (key == 1)
            {
                if (GetMoneyAmount() >= fishFood_1_Prefab.GetComponent<Scr_FoodBehavior>().price)
                {
                    SetFishFoodAmount(fishFood_1_Prefab, fishFood_1_Amount + 1);
                    SubtractMoneyAmount(fishFood_1_Prefab.GetComponent<Scr_FoodBehavior>().price);
                }
                else // Not enough money for purchase
                {
                    PlaySoundEffect(SFX_Error, 0.3f);
                    Debug.Log("Not enough money!");

                    // Make money text flash red
                    FlashTextColor(moneyText, Color.red, 0.5f, 0.1f);
                }
            }
            else if (key == 2)
            {
                if (GetMoneyAmount() >= fishFood_2_Prefab.GetComponent<Scr_FoodBehavior>().price)
                {
                    SetFishFoodAmount(fishFood_2_Prefab, fishFood_2_Amount + 1);
                    SubtractMoneyAmount(fishFood_2_Prefab.GetComponent<Scr_FoodBehavior>().price);
                }
                else // Not enough money for purchase
                {
                    PlaySoundEffect(SFX_Error, 0.3f);
                    Debug.Log("Not enough money!");

                    // Make money text flash red
                    FlashTextColor(moneyText, Color.red, 0.5f, 0.1f);
                }
            }
            else if (key == 3)
            {
                if (GetMoneyAmount() >= fishFood_3_Prefab.GetComponent<Scr_FoodBehavior>().price)
                {
                    SetFishFoodAmount(fishFood_3_Prefab, fishFood_3_Amount + 1);
                    SubtractMoneyAmount(fishFood_3_Prefab.GetComponent<Scr_FoodBehavior>().price);
                }
                else // Not enough money for purchase
                {
                    PlaySoundEffect(SFX_Error, 0.3f);
                    Debug.Log("Not enough money!");

                    // Make money text flash red
                    FlashTextColor(moneyText, Color.red, 0.5f, 0.1f);
                }
            }
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



            Scr_PhoneContact contact = null;

            // Find the contact with the matching number
            for (int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].phoneNumber == numberToCall)
                {
                    contact = contacts[i];
                    break;
                }
            }

            if (contact != null)
            {
                // Play global ringing sound
                tempPhoneAudioSource = PlaySoundEffectDontDestroy(SFX_CallRinging, 0.1f, 1);
                Destroy(tempPhoneAudioSource.gameObject, tempPhoneAudioSource.GetComponent<AudioSource>().clip.length);

                // Wait for the sound to finish, then open dialogue
                StartCoroutine(WaitForCallToFinishThenStartDialogue(contact, tempPhoneAudioSource.GetComponent<AudioSource>()));
            }
            else
            {
                // Play call fail sound
                tempPhoneAudioSource = PlaySoundEffectDontDestroy(SFX_CallFail, 0.5f, 1);
                Destroy(tempPhoneAudioSource.gameObject, tempPhoneAudioSource.GetComponent<AudioSource>().clip.length);
            }






        }
        else if (key == 999) //used for clearing on enabling/disabling phone
        {
            phoneNumber.text = "";

            currentlyCalling = string.Empty;
        }

    }

    // Coroutine: wait for ringing to finish, then start dialogue
    private IEnumerator WaitForCallToFinishThenStartDialogue(Scr_PhoneContact contact, AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);

        // Set dialogue lines
        dialogueBox.lines = contact.dialogueLines;

        // Open dialogue box (with your animation)
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.StartDialogue();

        // Trigger contact-specific behavior
        contact.OnCallAnswered(this);
    }

    public void SetFastForwardSetting()
    {
        fastForwardSetting++;

        if (fastForwardSetting > 3)
        {
            fastForwardSetting = 1;
        }

        Scr_TimeHandler.UpdateTimeScale(ActiveSettings.secondsPerTickEvent / GetFastForwardSettingFactor());
    }

    public float GetFastForwardSettingFactor()
    {
        float fastForwardSettingFactor;

        if (fastForwardSetting == 3)
        {
            fastForwardSettingFactor = ActiveSettings.fastForwardFastest;
        }
        else if (fastForwardSetting == 2)
        {
            fastForwardSettingFactor = ActiveSettings.fastForwardFast;
        }
        else
        {
            fastForwardSettingFactor = ActiveSettings.fastForwardNormal;
        }

        return fastForwardSettingFactor;
    }
}
