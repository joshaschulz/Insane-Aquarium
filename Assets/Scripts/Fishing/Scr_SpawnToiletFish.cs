using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SpawnToiletFish : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public GameObject fishingPoleIdle;
    public GameObject fishingPoleHooked;
    private Scr_TimeHandler Scr_TimeHandler;

    public bool shouldSpawn;
    public bool toiletFishExist;

    public int ticksToSpawnChance;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

    }

    // Update is called once per frame
    void Update()
    {
        
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
        //Debug.Log($"{gameObject.name} received a tick event!");

        int num = Random.Range(0, ticksToSpawnChance);

        Debug.Log(num + ", Toilet fish exists: " + toiletFishExist);

        if (shouldSpawn)
        {
            //fish escaped

            /*
            gameManager.PlaySoundEffect(gameManager.SFX_FishHitToilet, 1f);
            gameManager.PlaySoundEffect(gameManager.SFX_ToiletSplash, 0.7f);
            */

            //maybe another line snapping sound



            fishingPoleHooked.SetActive(false);

            fishingPoleIdle.SetActive(true);

            shouldSpawn = false;

            return;
        }

        if (num == 0 && !toiletFishExist) //1 in 6 chance to spawn fish (about every hour)
        {
            Debug.Log("Spawned fish");

            if (fishingPoleIdle.activeSelf)
                fishingPoleHooked.SetActive(true);

            fishingPoleIdle.SetActive(false);

            shouldSpawn = true;
        }



        // Your fish behavior here, e.g., update hunger status.
    }
}
