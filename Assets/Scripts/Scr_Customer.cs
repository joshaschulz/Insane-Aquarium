using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Scr_Customer : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;
    private Scr_FishyGuy Scr_FishyGuy;

    private Camera _Camera;

    public GameObject customerImagesParent;
    private GameObject[] customerImages;

    public GameObject customer;

    public int ticksToSpawnChance;
    public int ticksToExist;

    public bool customerExists = false;
    private int ticksSinceSpawned;

    private bool isCameraInBathroom;

    public GameObject customerFishPrefab;
    public Image customerFishImage;
    public int customerFishQuantity;
    public TextMeshProUGUI quantityText;

    public Scr_TankBounds[] forSaleTanks;
    private int ticksSinceOrderCreated = 0;

    private bool customerRecentlyLeft = false; //used to have a 1 tick gap in customer spawning


    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        _Camera = Camera.main;
        ChangeGameSettings();

        ticksToSpawnChance *= gameManager.tickEventsPer10Min;
        ticksToExist *= gameManager.tickEventsPer10Min;

        GetPotentialCustomers();
        PickCustomer();

    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        ticksToSpawnChance = settings.customerTicksToSpawnChance;
        ticksToExist = settings.customerTicksToExist;
    }

    // Update is called once per frame
    void Update()
    {
        //if the camera is not in the bathroom, hide the customer
        Vector2 cameraPos = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

        if (cameraPos == Scr_UIElementsHandler.bathroomPosition)
            isCameraInBathroom = true;
        else
            isCameraInBathroom = false;


        if (isCameraInBathroom && customerExists)
        {
            customer.SetActive(true);
        }
        else
        {
            customer.SetActive(false);
        }

    }

    private void OnEnable()
    {
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();
        Scr_UIElementsHandler = FindObjectOfType<Scr_UIElementsHandler>();
        Scr_FishyGuy = FindObjectOfType<Scr_FishyGuy>();



        float tickIntervalInMinutes = Scr_TimeHandler.tickInterval / 60;

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    public void OnTickEvent()
    {
        gameManager.RecalculateCustomerAttractionRate(); //maybe we dont want this in tickevent. possibly just on buy, birth, sell fish.
        Debug.Log("Customer attraction rate: " + gameManager.customerAttractionRate);

        //count how long customer has existed
        if (customerExists)
        {
            ticksSinceSpawned++;
        }

        //if they've existed too long, destroy them
        if (ticksSinceSpawned >= ticksToExist)
        {
            CustomerGoAway();
            return;
        }
        
        // handle delayed "for sale tank" auto purchase
        if (customerExists)
        {
            ticksSinceOrderCreated++;

            // after exactly 1 tick, try the auto-buy once
            if (ticksSinceOrderCreated >= 1)
            {
                TryAutoPurchaseFromForSaleTank();
            }
        }

        if (customerRecentlyLeft) //waits atleast 1 tick since a customer left for another to spawn
        {
            // consume the cooldown: skip this tick's spawn chance,
            // but allow spawning again next tick
            customerRecentlyLeft = false;
            return;
        }

        //spawn customer
        if (!customerExists && !Scr_FishyGuy.fishyGuyExists)
        {
            // base chance: 1 / ticksToSpawnChance
            float baseChance = 1f / ticksToSpawnChance;

            // scale by attraction (1.0–2.0)
            float chance = baseChance * gameManager.customerAttractionRate;

            // optional: safety clamp, so it never becomes crazy high
            chance = Mathf.Min(chance, 0.9f); // max 90% chance per tick

            if (Random.value < chance)
            {
                customerExists = true;
                PickCustomer();
                PickCustomerFishAndQuantity();
            }
        }

    }

    private void TryAutoPurchaseFromForSaleTank()
    {
        int required = Mathf.Max(1, customerFishQuantity);

        if (forSaleTanks == null || forSaleTanks.Length == 0)
            return;

        // Each candidate is: (tank, list of fish in that tank that match)
        List<(Scr_TankBounds tank, List<Scr_Fish> fish)> candidateTanks =
            new List<(Scr_TankBounds, List<Scr_Fish>)>();

        foreach (var tank in forSaleTanks)
        {
            if (tank == null)
                continue;

            // Only consider tanks whose for-sale sign is active
            if (!tank.IsForSaleActive)
                continue;

            Bounds b = tank.GetBounds();
            List<Scr_Fish> matchingFish = new List<Scr_Fish>();

            // Loop all fish in the scene and see which are inside THIS tank
            foreach (var kvp in gameManager.foodFishDictionary)
            {
                GameObject instance = kvp.Key;
                GameObject prefab = kvp.Value;

                if (prefab.GetComponent<Scr_Starfish>() != null)
                    continue;

                // Must be the species the customer wants
                if (prefab != customerFishPrefab)
                    continue;

                Scr_Fish fishScr = instance.GetComponent<Scr_Fish>();
                if (fishScr == null)
                    continue;

                // Must be inside this tank's bounds
                if (b.Contains(instance.transform.position))
                {
                    matchingFish.Add(fishScr);
                }
            }

            // This tank qualifies if it alone has enough fish
            if (matchingFish.Count >= required)
            {
                candidateTanks.Add((tank, matchingFish));
            }
        }

        // No for-sale tank has enough fish → no auto-sale this tick
        if (candidateTanks.Count == 0)
        {
            Debug.Log("Auto-buy failed: No for-sale tank has enough " +
                      customerFishPrefab.name + " (needed " + required + ").");
            return;
        }

        // Pick one qualifying tank at random
        int chosenIndex = Random.Range(0, candidateTanks.Count);
        var chosenTank = candidateTanks[chosenIndex];
        List<Scr_Fish> fishToSell = chosenTank.fish;

        int money = 0;
        for (int i = 0; i < required; i++)
        {
            Scr_Fish fish = fishToSell[i];
            money += fish.fishValue;

            if (gameManager.foodFishDictionary.ContainsKey(fish.gameObject))
            {
                gameManager.foodFishDictionary.Remove(fish.gameObject);
            }

            Destroy(fish.gameObject);
        }

        if (money > 0)
            gameManager.AddMoneyAmount(money);

        gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
        gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

        Debug.Log("Auto-sale success: Sold " + required + " " +
                  customerFishPrefab.name + " from tank " + chosenTank.tank.name);

        CustomerGoAway();
    }


    public void PickCustomer()
    {
        GameObject activeCustomer = customerImages[Random.Range(0, customerImages.Length)];

        activeCustomer.SetActive(true);

        foreach (GameObject customer in customerImages)
        {
            if (customer != activeCustomer)
            {
                customer.SetActive(false);
            }
        }
    }


    public void GetPotentialCustomers()
    {
        int count = customerImagesParent.transform.childCount;
        customerImages = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            customerImages[i] = customerImagesParent.transform.GetChild(i).gameObject;
        }
    }

    public void PickCustomerFishAndQuantity() //based on total fish appeal stat
    {
        // 1) Build species → weight dictionary
        Dictionary<GameObject, int> speciesWeights = new Dictionary<GameObject, int>();
        int totalWeight = 0;

        foreach (var kvp in gameManager.foodFishDictionary)
        {
            GameObject instance = kvp.Key;
            GameObject prefab = kvp.Value;

            // --- [A] skip starfish entirely when building weights ---
            if (prefab.GetComponent<Scr_Starfish>() != null)
                continue;

            Scr_Fish fishScript = instance.GetComponent<Scr_Fish>();
            if (fishScript == null)
                continue;

            int appeal = Mathf.Max(0, fishScript.appeal);
            int weight = 1 + appeal;  // 0⭐ → 1, 5⭐ → 6

            if (!speciesWeights.ContainsKey(prefab))
                speciesWeights[prefab] = 0;

            speciesWeights[prefab] += weight;
            totalWeight += weight;
        }

        GameObject chosenPrefab = null;
        Sprite fishSprite = null;

        // 2) Either use weighted random OR fallback, but DON'T return
        if (totalWeight == 0 || speciesWeights.Count == 0)
        {
            Debug.LogWarning("No fish species with appeal found in scene; falling back to uniform random prefab.");

            // --- [B] fallback: pick a random NON-starfish prefab ---
            List<int> nonStarfishIndexes = new List<int>();
            for (int i = 0; i < gameManager.fishPrefabs.Length; i++)
            {
                if (gameManager.fishPrefabs[i].GetComponent<Scr_Starfish>() == null)
                {
                    nonStarfishIndexes.Add(i);
                }
            }

            if (nonStarfishIndexes.Count == 0)
            {
                Debug.LogWarning("All fishPrefabs are starfish; no valid customer fish to choose.");
                return;
            }

            int randIdx = Random.Range(0, nonStarfishIndexes.Count);
            int num = nonStarfishIndexes[randIdx];

            chosenPrefab = gameManager.fishPrefabs[num];
            fishSprite = gameManager.fishSprites[num];
        }
        else
        {
            // 3) Weighted random pick over species
            int roll = Random.Range(0, totalWeight); // [0, totalWeight)

            foreach (var kvp in speciesWeights)
            {
                GameObject prefab = kvp.Key;
                int weight = kvp.Value;

                if (roll < weight)
                {
                    chosenPrefab = prefab;
                    break;
                }

                roll -= weight;
            }

            if (chosenPrefab == null)
            {
                Debug.LogWarning("Weighted species selection failed; no prefab chosen.");
                return;
            }

            // 4) Map prefab → sprite using your parallel arrays
            int index = System.Array.IndexOf(gameManager.fishPrefabs, chosenPrefab);
            if (index < 0 || index >= gameManager.fishSprites.Length)
            {
                Debug.LogWarning("Chosen fish prefab not found in fishPrefabs array.");
                return;
            }

            fishSprite = gameManager.fishSprites[index];
        }

        // Now we SHOULD have chosenPrefab and fishSprite (either via weighted or fallback)
        if (fishSprite != null)
        {
            customerFishImage.sprite = fishSprite;
            customerFishImage.SetNativeSize();
            customerFishImage.rectTransform.localScale = Vector3.one / 11f;
            customerFishPrefab = chosenPrefab;
        }
        else
        {
            Debug.LogWarning("No sprite found for chosen fish prefab.");
        }

        // --------------------------------------------------------
        // PICK QUANTITY OF FISH TO BUY BASED ON TOTAL NUMBER
        // --------------------------------------------------------

        // Count how many fish of this species are currently in the scene
        int speciesCount = 0;

        foreach (var kvp in gameManager.foodFishDictionary)
        {
            GameObject instance = kvp.Key;
            GameObject prefab = kvp.Value;

            if (prefab != chosenPrefab)
                continue;

            if (instance.GetComponent<Scr_Fish>() == null)
                continue; // skip food

            speciesCount++;
        }

        // Determine how many the customer wants based on the rules
        int quantity; // default to 1 ALWAYS

        // If there are real fish of that species, apply your rules
        if (speciesCount > 0)
        {
            if (speciesCount <= 10)
                quantity = 1;
            else if (speciesCount <= 20)
                quantity = 2;
            else
                quantity = 3;

            // Never ask for more than exist
            quantity = Mathf.Min(quantity, speciesCount);
        }
        else
        {
            // speciesCount == 0
            // This is the "fallback species" case.
            // Customer still wants 1 of that fish.
            quantity = 1;
        }

        // Assign to the customer
        customerFishQuantity = quantity;
        quantityText.text = "x" + customerFishQuantity;

        Debug.Log("CUSTOMER WANTS THIS NUMBER OF " + chosenPrefab.name.ToUpper() + ": " + customerFishQuantity);

        ticksSinceOrderCreated = 0;
    }



    public void DestroyCustomerFish()
    {
        customerFishImage.sprite = null;
    }

    public void SellFish()
    {
        int requiredQuantity = Mathf.Max(1, customerFishQuantity); // always at least 1

        // collect all bagged fish that match the requested species
        List<Scr_Fish> matchingFish = new List<Scr_Fish>();

        foreach (GameObject socket in gameManager.baggedFishSockets)
        {
            if (socket.transform.childCount == 0)
                continue;

            GameObject baggedFishObj = socket.transform.GetChild(0).gameObject;
            if (!baggedFishObj.CompareTag(customerFishPrefab.tag))
                continue;

            Scr_Fish fishScr = baggedFishObj.GetComponent<Scr_Fish>();
            if (fishScr != null)
                matchingFish.Add(fishScr);
        }

        // not enough fish to fulfill the order
        if (matchingFish.Count < requiredQuantity)
        {
            UnableToCompleteTransaction();
            return;
        }

        // sell exactly requiredQuantity fish
        int totalMoney = 0;
        for (int i = 0; i < requiredQuantity; i++)
        {
            Scr_Fish fish = matchingFish[i];
            totalMoney += fish.fishValue;

            Transform t = fish.transform;
            t.SetParent(null);
            Destroy(t.gameObject);
        }

        if (totalMoney > 0)
            gameManager.AddMoneyAmount(totalMoney);

        gameManager.ShowHideFishBags();

        gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
        gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

        CustomerGoAway();
    }

    public void DenyCustomer()
    {
        //maybe play sad customer noise
        CustomerGoAway();
    }

    public void CustomerGoAway()
    {
        customerExists = false;
        ticksSinceSpawned = 0;
        DestroyCustomerFish();

        ticksSinceOrderCreated = 0;

        customerRecentlyLeft = true;  // ← mark that someone just left
    }

    private void UnableToCompleteTransaction()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

        // Make cursor icon and selected food button flash red
        gameManager.FlashColor(clickedButton, Color.red, 0.5f, 0.1f);
    }
}
