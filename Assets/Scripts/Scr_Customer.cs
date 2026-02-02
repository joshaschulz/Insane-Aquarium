using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Scr_Customer : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;
    private Scr_FishyGuy Scr_FishyGuy;

    private Scr_Notifications notifications;

    private Camera _Camera;

    public GameObject oscar;

    public GameObject customerImagesParent;
    private GameObject[] customerImages;

    public Scr_CustomerContact[] customerDialogues;
    private Scr_CustomerContact customerContactToUse;

    public GameObject customer;

    public int ticksToSpawnChance;
    public int ticksToExist;

    public bool customerExists = false;
    private int ticksSinceSpawned;

    private int ticksToSpawnInitially;
    private int ticksSinceStart;
    private bool initialSpawnDelayComplete = false;

    private bool isCameraInBathroom;

    public GameObject customerFishPrefab;
    public Image customerFishImage;
    public int customerFishQuantity;
    public TextMeshProUGUI quantityText;

    public GameObject rejectButton;
    public GameObject acceptButton;
    public GameObject tutorialAcceptButton;
    public GameObject borderTutorialRocks;

    public Scr_TankBounds[] forSaleTanks;
    private int ticksSinceOrderCreated = 0;

    public int cooldownIn10MinUnits = 2; // e.g. 1 = 10 minutes, 2 = 20 minutes

    public int npcCooldownTicksRemaining = 0;

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>(); ;
    }
    // Start is called before the first frame update
    void Start()
    {

        notifications = FindObjectOfType<Scr_Notifications>();

        _Camera = Camera.main;
        ChangeGameSettings();

        ticksToSpawnChance *= gameManager.tickEventsPer10Min;
        ticksToExist *= gameManager.tickEventsPer10Min;

        ticksToSpawnInitially = ticksToExist / 2;

        GetPotentialCustomers();
        //PickCustomer();

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

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTickEvent()
    {
        if (!gameManager.tutorials.tutorialCompleted) //if tutorial happening
        {
            return;
        }

        //startup delay before allowing ANY customer spawn
        if (!initialSpawnDelayComplete)
        {
            ticksSinceStart++;

            if (ticksSinceStart < ticksToSpawnInitially)
            {
                return; //too early to spawn
            }

            initialSpawnDelayComplete = true; //from now on spawning is allowed
        }

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

        if (npcCooldownTicksRemaining > 0)
        {
            npcCooldownTicksRemaining--;
            return;
        }

        if (!CheckIfFishExist())
            return;

        //spawn customer
        if (!customerExists && !Scr_FishyGuy.fishyGuyExists)
        {
            int spawnChanceToUse = ticksToSpawnChance;
            if (gameManager.skills.currentCustomerServiceSkills[2])
            {
                spawnChanceToUse = ticksToSpawnChance / 2;
            }


            if (Random.Range(0, spawnChanceToUse) == 0)
            {
                customerExists = true;
                customer.SetActive(true);
                PickCustomer();
                PickCustomerFishAndQuantity();
            }
        }

    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        ticksToSpawnChance = settings.customerTicksToSpawnChance;
        ticksToExist = settings.customerTicksToExist;
    }

    public bool CheckIfFishExist()
    {
        if (gameManager.foodFishDictionary.Count == 0)
            return false;

        foreach (KeyValuePair<GameObject, GameObject> kvp in gameManager.foodFishDictionary)
        {
            if (kvp.Key.GetComponent<Scr_Fish>())
                return true; //atleast 1 fish in a tank
        }

        return false; //no fish in any tanks
    }

    private void TryAutoPurchaseFromForSaleTank()
    {
        if (!gameManager.skills.currentCustomerServiceSkills[4]) //if no oscar, don't auto purchase
            return;

        if (!gameManager.oscar.transform.GetChild(0).gameObject.activeSelf) //if he's not in his working state
            return;

        if (forSaleTanks == null || forSaleTanks.Length == 0)
            return;

        int required = Mathf.Max(1, customerFishQuantity);

        Dictionary<GameObject, GameObject> foodFishDictionaryCopy = new Dictionary<GameObject, GameObject>(gameManager.foodFishDictionary);

        Dictionary<GameObject, GameObject> forSaleFish = new Dictionary<GameObject, GameObject>();
        //GETS ALL AVAILABLE FOR SALE FISH
        foreach (var tank in forSaleTanks)
        {
            // Only consider tanks whose for-sale sign is active
            if (!tank.IsForSaleActive)
                continue;

            Bounds b = tank.GetBounds();
            List<Scr_Fish> matchingFish = new List<Scr_Fish>();

            // Loop all fish in the scene and see which are inside THIS tank
            foreach (var kvp in foodFishDictionaryCopy)
            {
                GameObject instance = kvp.Key;
                GameObject prefab = kvp.Value;

                if (prefab.GetComponent<Scr_ExoticFish>() != null)
                    continue;

                Scr_Fish fishScr = instance.GetComponent<Scr_Fish>();
                if (fishScr == null)
                    continue;

                if (b.Contains(instance.transform.position))
                {
                    forSaleFish.Add(instance, prefab);
                }
            }
        }

        if (forSaleFish.Count == 0)
            return;


        int money = 0;
        bool sold = false;
        int numSold = 0;
        for (int i = 0; i < required; i++)
        {
            Scr_Fish fishToSell = null;

            foreach (var kvp in forSaleFish)
            {
                if (kvp.Value == customerFishPrefab)
                {
                    fishToSell = kvp.Key.GetComponent<Scr_Fish>();
                }
            }

            if (fishToSell != null)
            {
                money += fishToSell.fishValue;
                numSold++;
                sold = true;

                if (gameManager.foodFishDictionary.ContainsKey(fishToSell.gameObject))
                {
                    gameManager.foodFishDictionary.Remove(fishToSell.gameObject);
                }

                fishToSell.currentTankScript.RemoveFish(fishToSell.thisPrefab);

                Destroy(fishToSell.gameObject);

            }

        }

        if (sold)
        {
            bool tipped = false;

            if (gameManager.skills.currentCustomerServiceSkills[1]) //customer tip skill
            {
                money = (int)(money * 1.1);
                tipped = true;
            }

            notifications.Show($"{(tipped ? "Customer tipped 10%! " : "")} {numSold} {customerFishPrefab.tag} was sold for {money} krona!", false);
            gameManager.AddMoneyAmount(money);

            gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
            gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

            CustomerGoAway();

        }

    }


    public void PickCustomer()
    {
        rejectButton.transform.GetChild(0).gameObject.SetActive(true);

        GameObject activeCustomer = customerImages[Random.Range(0, customerImages.Length)];

        activeCustomer.SetActive(true);

        foreach (Scr_CustomerContact customerContact in customerDialogues)
        {
            if (customerContact.contactName.Equals(activeCustomer.name))
            {
                Debug.Log("PICKED " + customerContact.contactName);
                customerContactToUse = customerContact;
            }
        }

        if (customerExists)
        {

            gameManager.IncrementVisits(customerContactToUse);
            gameManager.StartCustomerDialogue(customerContactToUse, gameManager.customerTradePanel);
            gameManager.dialogueBoxCustomer.customerName.text = activeCustomer.name.ToString();

            Debug.Log("GOT TO ONENABLE WITH " + customerContactToUse.name);
        }

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
        if (forSaleTanks == null || forSaleTanks.Length == 0)
            return;

        Dictionary<GameObject, GameObject> foodFishDictionaryCopy = new Dictionary<GameObject, GameObject>(gameManager.foodFishDictionary);

        //GETS ALL FISH IN THE SCENE
        Dictionary<GameObject, int> allFishAndQuantity = new Dictionary<GameObject, int>();
        foreach (var kvp in foodFishDictionaryCopy)
        {
            GameObject instance = kvp.Key;
            GameObject prefab = kvp.Value;

            Scr_Fish fishScr = instance.GetComponent<Scr_Fish>();
            if (fishScr == null)
                continue;

            if (allFishAndQuantity.ContainsKey(prefab))
            {
                allFishAndQuantity[prefab]++;
            }
            else
            {
                allFishAndQuantity.Add(prefab, 1);
            }
        }

        Debug.Log("GOT ALL FISH");

        Dictionary<GameObject, int> forSaleFishAndQuantity = new Dictionary<GameObject, int>();
        if (allFishAndQuantity.Count > 0) //ONLY CHECK FOR SALE TANKS IF THERE ARE FISH IN THE SCENE
        {
            //GETS ALL AVAILABLE FOR SALE FISH
            foreach (var tank in forSaleTanks)
            {
                // Only consider tanks whose for-sale sign is active
                if (!tank.IsForSaleActive)
                    continue;

                Bounds b = tank.GetBounds();
                List<Scr_Fish> matchingFish = new List<Scr_Fish>();

                // Loop all fish in the scene and see which are inside THIS tank
                foreach (var kvp in foodFishDictionaryCopy)
                {
                    GameObject instance = kvp.Key;
                    GameObject prefab = kvp.Value;

                    if (prefab.GetComponent<Scr_ExoticFish>() != null)
                        continue;

                    Scr_Fish fishScr = instance.GetComponent<Scr_Fish>();
                    if (fishScr == null)
                        continue;

                    if (b.Contains(instance.transform.position))
                    {
                        if (forSaleFishAndQuantity.ContainsKey(prefab))
                        {
                            forSaleFishAndQuantity[prefab]++;
                        }
                        else
                        {
                            forSaleFishAndQuantity.Add(prefab, 1);
                        }
                        //foodFishDictionaryCopy.Remove(instance);
                    }
                }
            }
        }

        Debug.Log("GOT ALL FOR SALE FISH");


        //CHOOSE FISH AND QUANTITY
        GameObject chosenPrefab;
        int chosenQuantity;
        Dictionary<GameObject, int> fishAndQuantityToUse;

        if (forSaleFishAndQuantity.Count == 0)//if there were no for sale stickers, pick a random fish and quantity
        {
            chosenPrefab = gameManager.fishPrefabs[Random.Range(0, gameManager.fishPrefabs.Length)];

            if (allFishAndQuantity.Keys.Contains(chosenPrefab))//if the randomly selected prefab exists, choose quantity based on the num
            {
                chosenQuantity = allFishAndQuantity[chosenPrefab];
            }
            else
            {
                chosenQuantity = 1;
            }
        }
        else // there are for sale tanks
        {
            // STEP 1: get total number of fish across all prefabs
            int totalFish = 0;
            foreach (var kvp in forSaleFishAndQuantity)
                totalFish += kvp.Value;

            // safety check
            if (totalFish == 0)
                return;

            chosenPrefab = null;
            chosenQuantity = 0;

            // STEP 2: pick a random "fish slot"
            int roll = Random.Range(0, totalFish);

            // STEP 3: walk through cumulative counts
            int runningTotal = 0;
            foreach (var kvp in forSaleFishAndQuantity)
            {
                runningTotal += kvp.Value;

                if (roll < runningTotal)
                {
                    chosenPrefab = kvp.Key;
                    chosenQuantity = kvp.Value;
                    break;
                }
            }
        }



        customerFishPrefab = chosenPrefab;

        if (gameManager.skills.currentCustomerServiceSkills[3]) // Reel Deal skill
        {
            // every 5 fish increases purchase amount by 1
            customerFishQuantity = Mathf.Max(1, Mathf.CeilToInt(chosenQuantity / 5f));
        }
        else
        {
            customerFishQuantity = 1;
        }

        quantityText.text = "x" + customerFishQuantity;

        SetFishImage(chosenPrefab);

        Debug.Log("CUSTOMER WANTS THIS NUMBER OF " + chosenPrefab.name.ToUpper() + ": " + customerFishQuantity);

        ticksSinceOrderCreated = 0;
    }

    private void SetFishImage(GameObject prefab)
    {
        customerFishImage.sprite = prefab.GetComponent<Scr_FishAnimation>().sideSprite;
        customerFishImage.SetNativeSize();
        customerFishImage.rectTransform.localScale = Vector3.one / 9f;
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

        if (matchingFish.Count == 0)
        {
            notifications.Show("You have no fish in your bags to sell.", false);
            return;
        }

        List<Scr_Fish> fishToSell = new List<Scr_Fish>();

        int amountToTake = Mathf.Min(requiredQuantity, matchingFish.Count);

        for (int i = 0; i < amountToTake; i++)
        {
            fishToSell.Add(matchingFish[i]);
        }

        // sell exactly requiredQuantity fish
        int totalMoney = 0;

        foreach (Scr_Fish fish in fishToSell)
        {

            totalMoney += fish.fishValue;

            Transform t = fish.transform;
            t.SetParent(null);
            Destroy(t.gameObject);
        }

        bool tipped = false;
        if (gameManager.skills.currentCustomerServiceSkills[1]) //customer tip skill
        {
            totalMoney = (int)(totalMoney * 1.1);
            tipped = true;
        }

        notifications.Show($"{(tipped ? "Customer tipped 10%! " : "")}{fishToSell.Count} {fishToSell[0].tag} was sold for {totalMoney} krona!", false);

        gameManager.AddMoneyAmount(totalMoney);


        gameManager.ShowHideFishBags();

        gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
        gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

        //gameManager.dialogueBoxCustomer.StartCloseBoxEnum();

        CustomerGoAway();

        if (!gameManager.tutorials.tutorialCompleted)
        {
            gameManager.Scr_TimeHandler.SetGameSeconds(gameManager.Scr_TimeHandler.endTimeInSeconds);
            gameManager.Scr_TimeHandler.UpdateClockDisplay();
            gameManager.tutorials.HideTutorialBox();
            gameManager.SetMoneyAmount(400);
            gameManager.EndDay();

            gameManager.tutorials.ShowNextTutorialBoxDelay(2.2f);

            HideTutorialAccept();
        }
    }

    public void DenyCustomer()
    {
        //maybe play sad customer noise
        CustomerGoAway();
    }

    public void CustomerGoAway()
    {
        gameManager.dialogueBoxCustomer.StartCustomerCloseBoxEnum(gameManager.customerTradePanel);

        customerExists = false;
        ticksSinceSpawned = 0;
        //DestroyCustomerFish();
        customer.SetActive(false);

        ticksSinceOrderCreated = 0;

        npcCooldownTicksRemaining = cooldownIn10MinUnits * gameManager.tickEventsPer10Min;
    }

    private void UnableToCompleteTransaction()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

        // Make cursor icon and selected food button flash red
        gameManager.FlashColor(clickedButton, Color.red, 0.5f, 0.1f);

        notifications.Show("Not enough fish in bags to complete transaction.", 2f, true);
    }

    private bool CheckIfLegendaryFishPresent() //return true if there is a legendary fish of the same species in a tank
    {
        foreach (var obj in gameManager.foodFishDictionary)
        {
            if (obj.Key.GetComponent<Scr_Fish>() != null)
            {
                if (obj.Key.CompareTag(customerFishPrefab.tag))
                {
                    if (obj.Key.GetComponent<Scr_Fish>().legendary)
                        return true;
                }

            }
        }

        return false;
    }

    public void SpawnTutorialCustomer()
    {
        customer.SetActive(true);

        GameObject activeCustomer = customerImages[0]; //spawn mr duckworth

        activeCustomer.SetActive(true);

        foreach (Scr_CustomerContact customerContact in customerDialogues)
        {
            if (customerContact.contactName.Equals(activeCustomer.name))
            {
                Debug.Log("PICKED " + customerContact.contactName);
                customerContactToUse = customerContact;
            }
        }

        gameManager.IncrementVisits(customerContactToUse);
        gameManager.StartCustomerDialogue(customerContactToUse, gameManager.customerTradePanel);
        gameManager.dialogueBoxCustomer.customerName.text = activeCustomer.name.ToString();


        foreach (GameObject customer in customerImages)
        {
            if (customer != activeCustomer)
            {
                customer.SetActive(false);
            }
        }


        customerFishPrefab = gameManager.fishPrefabs[3]; //pick goldfish

        customerFishQuantity = 1;

        quantityText.text = "x" + customerFishQuantity;

        SetFishImage(customerFishPrefab);

        ShowTutorialAccept();
    }

    void ShowTutorialAccept()
    {
        gameManager.DisableButton(rejectButton.GetComponent<Button>());
        gameManager.DisableButton(acceptButton.GetComponent<Button>());
        rejectButton.transform.GetChild(0).gameObject.SetActive(false);
        acceptButton.transform.GetChild(0).gameObject.SetActive(false);
        borderTutorialRocks.SetActive(false);

        tutorialAcceptButton.SetActive(true);
        gameManager.EnableButton(tutorialAcceptButton.GetComponent<Button>());
    }

    void HideTutorialAccept()
    {
        tutorialAcceptButton.SetActive(false);

        gameManager.EnableButton(rejectButton.GetComponent<Button>());
        gameManager.EnableButton(acceptButton.GetComponent<Button>());
        rejectButton.transform.GetChild(0).gameObject.SetActive(true);
        acceptButton.transform.GetChild(0).gameObject.SetActive(true);
        borderTutorialRocks.SetActive(true);
    }

    public void InvokeFunctionWithDelay(string functionName, float delay)
    {
        Invoke(functionName, delay);
    }
}
