using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_SpawnToiletFish : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public GameObject fishingPoleIdle;
    public GameObject fishingPoleHooked;
    private Scr_TimeHandler Scr_TimeHandler;

    public bool shouldSpawn;
    public bool toiletFishExist;

    public int ticksToSpawnChance;

    public Transform StallTransform;
    public GameObject bathroom;
    public GameObject rod;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        ChangeGameSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        ticksToSpawnChance = settings.toiletFishTicksToSpawnChance;
    }

    private void OnEnable()
    {
        shouldSpawn = false;
        toiletFishExist = false;

        // Optionally, get a reference to the TickHandler (assuming there's only one or it’s a singleton)
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();

        float tickIntervalInMinutes = Scr_TimeHandler.tickInterval / 60;

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks.
        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.RemoveListener(OnTickEvent);
        }

    }

    
    public void OnTickEvent()
    {
        /*
        //Debug.Log($"{gameObject.name} received a tick event!");

        int num = Random.Range(0, ticksToSpawnChance);

        //Debug.Log(num + ", Toilet fish exists: " + toiletFishExist);

        if (fishingPoleHooked.activeSelf)
        {
            //fish escaped
            

            //maybe another line snapping sound



            fishingPoleHooked.SetActive(false);

            fishingPoleIdle.SetActive(true);

            shouldSpawn = false;

            if (!toiletFishExist)
                gameManager.PlaySoundEffect(gameManager.SFX_LineSnap, 0.3f);

            return;
        }


        if (num == 0 && !toiletFishExist) //1 in 6 chance to spawn fish (about every hour)
        {
            //Debug.Log("Spawned fish");

            if (fishingPoleIdle.activeSelf)
                fishingPoleHooked.SetActive(true);

            fishingPoleIdle.SetActive(false);
            //gameManager.PlaySoundEffect(gameManager.SFX_FishHooked, 0.2f);

            if (gameManager.baggedFish_Socket1.transform.childCount == 0 || gameManager.baggedFish_Socket2.transform.childCount == 0 || gameManager.baggedFish_Socket3.transform.childCount == 0)
            {
                shouldSpawn = true;
            }
        }

        // Your fish behavior here, e.g., update hunger status.

        */
    }

    public void SpawnToiletFish()
    {
        if (!toiletFishExist)
        {
            if (gameManager.baggedFish_Socket1.transform.childCount == 0 || gameManager.baggedFish_Socket2.transform.childCount == 0 || gameManager.baggedFish_Socket3.transform.childCount == 0)
            {
                shouldSpawn = true;
            }
        }
    }

    public void FishingPoleHookedButton(Button clickedButton)
    {
        //GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (gameManager.baggedFish_Socket1.transform.childCount == 0 || gameManager.baggedFish_Socket2.transform.childCount == 0 || gameManager.baggedFish_Socket3.transform.childCount == 0)
        {
            gameManager.MoveToSceneOrPause(StallTransform);
            gameManager.DisableElement(bathroom);
            gameManager.EnableElement(rod);
        }
        else
        {

            //Debug.Log(clickedButton.name);

            gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);

            // Make cursor icon and selected food button flash red
            gameManager.FlashColor(clickedButton.gameObject, Color.red, 0.5f, 0.1f);
        }
    }
}
