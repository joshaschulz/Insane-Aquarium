using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_Customer : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;
    private Scr_FishyGuy Scr_FishyGuy;

    private Camera _Camera;

    public GameObject customerImage;

    public int ticksToSpawnChance;
    public int ticksToExist;

    public bool customerExists = false;
    private int ticksSinceSpawned;

    private bool isCameraInBathroom;

    public GameObject customerFishPrefab;
    public Image customerFishImage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        _Camera = Camera.main;

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
            customerImage.SetActive(true);
        }
        else
        {
            customerImage.SetActive(false);
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

        //spawn customer
        int num = Random.Range(0, ticksToSpawnChance);

        if (num == 0)
        {
            if (!customerExists && !Scr_FishyGuy.fishyGuyExists)
            {
                customerExists = true;
                PickCustomerFish();
            }
        }

    }

    public void PickCustomerFish()
    {
        //picks a random fish prefab
        int num = Random.Range(0, gameManager.fishSprites.Length - 1);
        Sprite fishSprite = gameManager.fishSprites[num];
        GameObject fishPrefab = gameManager.fishPrefabs[num]; //assumes that fish images in the fish images folder and the fish prefabs in the fish prefabs folder are ordered the same

        if (fishSprite)
        {
            customerFishImage.sprite = fishSprite;
            customerFishImage.SetNativeSize();

            customerFishImage.rectTransform.localScale = Vector3.one / 11f; //from native size to about the size of a fish in the tank

            customerFishPrefab = fishPrefab;

        }
        else
        {
            Debug.LogWarning("No Images found in Resources/Assets/Fish!");
        }

    }

    public void DestroyCustomerFish()
    {
        customerFishImage.sprite = null;
    }

    public void SellFish()
    {
        foreach (GameObject baggedFishSocket in gameManager.baggedFishSockets)
        {
            if (baggedFishSocket.transform.childCount != 0)
            {
                GameObject baggedFish = baggedFishSocket.transform.GetChild(0).gameObject;
                Scr_Fish baggedFishScr = baggedFish.GetComponent<Scr_Fish>();

                //player has the fish that the customer wants to buy
                if (baggedFish.CompareTag(customerFishPrefab.tag))
                {
                    gameManager.AddMoneyAmount(baggedFishScr.fishCost);

                    baggedFish.transform.SetParent(null);
                    Destroy(baggedFish);
                    gameManager.ShowHideFishBags();

                    gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.4f, 1f, 1f);
                    gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.4f, 1f, 1f);

                    //maybe play a happy customer noise
                    CustomerGoAway();
                    return;
                }
            }
        }

        //if reach here, it means that the player did not have the fish to sell
        UnableToCompleteTransaction();
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
    }

    private void UnableToCompleteTransaction()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

        // Make cursor icon and selected food button flash red
        gameManager.FlashColor(clickedButton, Color.red, 0.5f, 0.1f);
    }
}
