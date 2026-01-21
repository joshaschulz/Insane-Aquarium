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
    public int ticksToExist;

    public bool fishyGuyExists = false;
    private int ticksSinceSpawned;

    private bool isCameraInBathroom;

    public Image fishyGuyFishImage1;
    private GameObject fishToBuy;
    //public Image fishyGuyFishImage2;

    public TextMeshProUGUI priceText;

    public GameObject fishBag1;
    //public GameObject fishBag2;

    public int cooldownIn10MinUnits = 1; // e.g. 1 = 10 minutes, 2 = 20 minutes

    private int fishyGuyCooldownTicksRemaining = 0;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        notifications = FindObjectOfType<Scr_Notifications>();


        _Camera = Camera.main;

        ChangeGameSettings();

        ticksToSpawnChance *= gameManager.tickEventsPer10Min;
        ticksToExist *= gameManager.tickEventsPer10Min;

        ticksToSpawnInitially = ticksToExist / 2;


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
        //if the camera is not in the bathroom, hide the customer
        Vector2 cameraPos = new Vector2(_Camera.transform.position.x, _Camera.transform.position.y);

        if (cameraPos == Scr_UIElementsHandler.bathroomPosition)
            isCameraInBathroom = true;
        else
            isCameraInBathroom = false;


        if (isCameraInBathroom && fishyGuyExists)
        {
            fishyGuyImage.SetActive(true);
        }
        else
        {
            fishyGuyImage.SetActive(false);
        }

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

        if (fishyGuyCooldownTicksRemaining > 0)
        {
            fishyGuyCooldownTicksRemaining--;
            return;
        }

        //count how long customer has existed
        if (fishyGuyExists)
        {
            ticksSinceSpawned++;
        }

        //if they've existed too long, destroy them
        if (ticksSinceSpawned >= ticksToExist)
        {
            FishyGuyGoAway();
            return;
        }

        //spawn customer
        int num = Random.Range(0, ticksToSpawnChance);

        if (num == 0)
        {
            if (!fishyGuyExists && !Scr_Customer.customerExists)
            {
                fishyGuyExists = true;
                PickFishyGuyFish();
            }
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

            if (gameManager.moneyAmount > fishToBuyScr.baseFishCost && !gameManager.CheckIfFullFishBags())
            {
                GameObject fish = gameManager.SpawnTempStarfish(fishToBuy, gameManager.fishWaitingArea);
                if (gameManager.BagFishyGuyStarfish(fish))
                {
                    gameManager.SubtractMoneyAmount(fishToBuyScr.baseFishCost);

                    notifications.Show($"{fish.tag} was purchased for {fishToBuyScr.baseFishCost} krona!");

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
                    UnableToCompleteTransaction();
                    return;
                }
            }
            else
            {
                UnableToCompleteTransaction();
                return;
            }
        }


    }

    private string NormalizeName(string name)
    {
        string[] suffixes = { " Front", " Back", " Left", " Right", " Top", " Bottom" };
        foreach (var s in suffixes)
        {
            if (name.EndsWith(s, System.StringComparison.OrdinalIgnoreCase))
                return name.Substring(0, name.Length - s.Length).Trim();
        }
        // fallback: drop anything after first space/underscore/dash
        int cut = name.IndexOfAny(new[] { ' ', '_', '-' });
        return cut >= 0 ? name.Substring(0, cut).Trim() : name.Trim();
    }

    public void HideBoughtFishBag(GameObject fishBag)
    {
        fishBag.SetActive(false);
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

        fishBag1.SetActive(true);
        //fishBag2.SetActive(true);

        fishyGuyCooldownTicksRemaining = cooldownIn10MinUnits * gameManager.tickEventsPer10Min;

        //DestroyCustomerFish();

    }

    private void UnableToCompleteTransaction()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

        // Make cursor icon and selected food button flash red
        gameManager.FlashColor(clickedButton, Color.red, 0.5f, 0.1f);

        notifications.Show("Not enough money to complete transaction.", 2f);

    }
}
