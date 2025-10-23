using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_UIElementsHandler : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler Scr_TimeHandler;

    public Transform foregroundTank;
    public Transform backgroundTank;

    public Vector2 foregroundTankPosition;
    public Vector2 backgroundTankPosition;

    public GameObject foregroundTankEmpty;
    public GameObject foregroundTankFull;
    public GameObject backgroundTankEmpty;
    public GameObject backgroundTankFull;

    public List<Vector2> tankPositions;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        foregroundTankPosition = new Vector2(foregroundTank.position.x, foregroundTank.position.y);
        backgroundTankPosition = new Vector2(backgroundTank.position.x, backgroundTank.position.y);


        tankPositions = new List<Vector2> {foregroundTankPosition, backgroundTankPosition };
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
