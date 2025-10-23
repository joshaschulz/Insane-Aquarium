using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_UpdateUIOnTickEvent : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;

    public Transform foregroundTank;
    public Transform backgroundTank;

    public GameObject foregroundTankEmpty;
    public GameObject foregroundTankFull;
    public GameObject backgroundTankEmpty;
    public GameObject backgroundTankFull;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

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
        UpdateTankWater();
    }

    public void UpdateTankWater()
    {
        if (gameManager.CheckIfTankHasFish(foregroundTank) != 0)
        {
            foregroundTankFull.SetActive(true);
            foregroundTankEmpty.SetActive(false);
        }
        else
        {
            foregroundTankFull.SetActive(false);
            foregroundTankEmpty.SetActive(true);
        }

        if (gameManager.CheckIfTankHasFish(backgroundTank) != 0)
        {
            backgroundTankFull.SetActive(true);
            backgroundTankEmpty.SetActive(false);
        }
        else
        {
            backgroundTankFull.SetActive(false);
            backgroundTankEmpty.SetActive(true);
        }
    }
}
