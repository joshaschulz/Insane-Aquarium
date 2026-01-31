using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_FishyGuy : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Scr_Fishpedia fishpedia;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;
    private Scr_Customer Scr_Customer;

    private Scr_Notifications notifications;

    private Camera _Camera;

    public GameObject fishyGuyImage;

    public Scr_CustomerContact fishyGuyDialogue;

    private int ticksToSpawnInitially;
    private int ticksSinceStart;
    private bool initialSpawnDelayComplete = false;

    public int ticksToSpawnChance;
    private int currentSpawnChance;
    private int minSpawnChance = 2;
    public int ticksToExist;

    public bool fishyGuyExists = false;
    private int ticksSinceSpawned;

    private bool isCameraInBathroom;

    public Image fishyGuyFishImage1;
    private GameObject fishToBuy;
    //public Image fishyGuyFishImage2;

    public TextMeshProUGUI priceText;

    //public GameObject fishBag2;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        notifications = FindObjectOfType<Scr_Notifications>();


        _Camera = Camera.main;

        ChangeGameSettings();

        ticksToSpawnChance *= gameManager.tickEventsPer10Min;
        minSpawnChance *= gameManager.tickEventsPer10Min;
        ticksToExist *= gameManager.tickEventsPer10Min;

        ticksToSpawnInitially = ticksToExist / 2;
        currentSpawnChance = ticksToSpawnChance;


    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        ticksToSpawnChance = settings.fishyGuyTicksToSpawnChance;
        ticksToExist = settings.fishyGuyTicksToExist;
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnEnable()
    {
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();
        Scr_UIElementsHandler = FindObjectOfType<Scr_UIElementsHandler>();
        Scr_Customer = FindObjectOfType<Scr_Customer>();


        float tickIntervalInMinutes = Scr_TimeHandler.tickInterval / 60;

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    public void OnTickEvent()
    {
        if (!gameManager.tutorials.tutorialCompleted) //if tutorial happening
        {
            return;
        }

        //fishy guy won't spawn if you have less than 500 krona
        if (gameManager.moneyAmount < 500)
            return;

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

        if (Scr_Customer.npcCooldownTicksRemaining > 0)
        {
            //Scr_Customer.npcCooldownTicksRemaining--; dont need to decrement since customer is decrementing
            return;
        }

        //count how long customer has existed
        if (fishyGuyExists)
        {
            ticksSinceSpawned++;

            //if they've existed too long, destroy them
            if (ticksSinceSpawned >= ticksToExist)
            {
                FishyGuyGoAway();
                return;
            }

            return;
        }



        //spawn customer
        int num = Random.Range(0, currentSpawnChance);

        if (num == 0)
        {
            if (!fishyGuyExists && !Scr_Customer.customerExists)
            {
                fishyGuyExists = true;
                fishyGuyImage.SetActive(true);
                PickFishyGuyFish();

                currentSpawnChance = ticksToSpawnChance;
            }
        }
        else
        {
            currentSpawnChance = Mathf.Max(minSpawnChance, currentSpawnChance - 1);
        }

    }

    public void PickFishyGuyFish()
    {

        Scr_CustomerContact customerContactToUse = fishyGuyDialogue;

        if (fishyGuyExists)
        {
            gameManager.IncrementVisits(customerContactToUse);

            gameManager.StartCustomerDialogue(customerContactToUse, gameManager.fishyGuyTradePanel);
            gameManager.dialogueBoxCustomer.customerName.text = gameObject.name.ToString();

        }

        //picks a random fish prefab
        Sprite fishSprite1 = gameManager.exoticFishSprites[Random.Range(0, gameManager.exoticFishSprites.Length - 1)];

        foreach (var p in gameManager.exoticFishPrefabs)
        {
            Debug.Log("Comparing " + fishSprite1.name + " to " + p.name);
            if (fishSprite1.name.Contains(p.name))
            {
                fishToBuy = p;
                break;
            }
        }

        Debug.Log("FISHY GUY PICKED" + fishSprite1.name);

        if (fishSprite1)
        {
            fishyGuyFishImage1.sprite = fishSprite1;
            fishyGuyFishImage1.SetNativeSize();
            fishyGuyFishImage1.rectTransform.localScale = Vector3.one / 11f; //from native size to about the size of a fish in the tank

        }
        else
        {
            Debug.LogWarning("No Images found in Resources/Assets/Fish!");
        }

        if (fishToBuy.GetComponent<Scr_Starfish>() != null)
        {
            priceText.text = fishToBuy.GetComponent<Scr_Starfish>().baseFishCost.ToString();
        }

    }

    public void DestroyCustomerFish()
    {
        fishyGuyFishImage1.sprite = null;
        //fishyGuyFishImage2.sprite = null;
    }

    public void BuyFish()
    { 
        if (fishToBuy.GetComponent<Scr_Starfish>() != null)
        {
            Scr_Starfish fishToBuyScr = fishToBuy.GetComponent<Scr_Starfish>();

            if (gameManager.moneyAmount > fishToBuyScr.baseFishCost)
            {
                if (!gameManager.CheckIfFullFishBags())
                {
                    GameObject fish = gameManager.SpawnTempStarfish(fishToBuy, gameManager.fishWaitingArea);
                    if (gameManager.BagFishyGuyStarfish(fish))
                    {
                        gameManager.SubtractMoneyAmount(fishToBuyScr.baseFishCost);

                        notifications.Show($"{fish.tag} was purchased for {fishToBuyScr.baseFishCost} krona!", false);

                        gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
                        gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

                        // NEW FISHPEDIA CODE
                        // Enabling fishpedia buttons upon buying an exotic fish
                        int buttonIndex = 0;
                        foreach (Button button in fishpedia.buttons)
                        {
                            if (fishToBuy.name.Contains(button.name))
                            {
                                Debug.Log("FOUND THE " + button.name + " BUTTON");
                                fishpedia.EnableEntryButton(buttonIndex);
                            }
                            buttonIndex++;
                        }
                        FishyGuyGoAway();
                    }
                    else
                    {
                        notifications.Show("Bag fish slots are full.", 2f, true);

                        UnableToCompleteTransaction();
                        return;
                    }
                }
                else
                {
                    notifications.Show("Bag fish slots are full.", 2f, true);

                    UnableToCompleteTransaction();
                    return;
                }

            }
            else
            {
                notifications.Show("Not enough money to complete transaction.", 2f, true);

                UnableToCompleteTransaction();
                return;
            }
        }


    }


    public void DenyCustomer()
    {
        //maybe play sad customer noise
        FishyGuyGoAway();
    }

    public void FishyGuyGoAway()
    {
        Debug.Log("TRIED FISHY GUY GO AWAY");

        gameManager.dialogueBoxCustomer.StartCustomerCloseBoxEnum(gameManager.fishyGuyTradePanel);

        Debug.Log("FISHY GUY GO AWAY");

        fishyGuyExists = false;
        ticksSinceSpawned = 0;

        fishyGuyImage.SetActive(false);

        Scr_Customer.npcCooldownTicksRemaining = Scr_Customer.cooldownIn10MinUnits * gameManager.tickEventsPer10Min;


    }

    private void UnableToCompleteTransaction()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

        // Make cursor icon and selected food button flash red
        gameManager.FlashColor(clickedButton, Color.red, 0.5f, 0.1f);
    }
}
