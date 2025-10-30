using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Customer : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    private Camera _Camera;

    public GameObject customerImage;

    public int ticksToSpawnChance;
    public int ticksToExist;

    private bool customerExists = false;
    private int ticksSinceSpawned;

    private bool isCameraInBathroom;

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
            customerExists = false;
            ticksSinceSpawned = 0;
            return;
        }

        //spawn customer
        int num = Random.Range(0, ticksToSpawnChance);

        if (num == 0)
        {
            if (!customerExists)
            {
                customerExists = true;
            }
        }

    }
}
