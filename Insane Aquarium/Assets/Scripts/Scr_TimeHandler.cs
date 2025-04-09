using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Scr_TimeHandler : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent tickEvent;

    public float tickInterval = 600f; //every __ seconds a tick event will fire
    private float tickAccumulator; //tick will trigger every __ minutes then reset.


    [Header("Time Settings")]
    public float timeScale = 60f; //in-game seconds per real second

    public int startHour = 6;
    public int startMinute = 0;
    public int endHour = 7;
    public int endMinute = 10;

    private float gameSeconds; //actual in game seconds that have passed

    private int startTimeInSeconds;
    private int endTimeInSeconds;

    [Header("UI Elements")]
    public TextMeshProUGUI timeDisplayText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        startTimeInSeconds = startHour * 3600 + startMinute * 60;
        endTimeInSeconds = endHour * 3600 + endMinute * 60;

        gameSeconds = startTimeInSeconds; //clock starts at the start time
    }

    // Update is called once per frame
    void Update()
    {
        float deltaGameSeconds = Time.deltaTime * timeScale; //amount of game seconds per frame
        gameSeconds += deltaGameSeconds; //total number of in-game seconds that have passed (60 in-game seconds per real second)
        tickAccumulator += deltaGameSeconds;


        if (gameSeconds >= endTimeInSeconds)
        {
            gameSeconds = startTimeInSeconds + (gameSeconds - endTimeInSeconds);
        }

        UpdateClockDisplay();

        CheckEventTick();
    }

    private void UpdateClockDisplay()
    {
        int hours = ((int)gameSeconds / 3600) % 24;
        int minutes = ((int)gameSeconds / 60) % 60;

        // Format the time as HH:MM (e.g., "19:20").
        string formattedTime = string.Format("{0:00}:{1:00}", hours, minutes);

        if (timeDisplayText != null)
        {
            //timeDisplayText.text = formattedTime;

            if (minutes % 10 == 0) //only update every 10 minutes
            {
                timeDisplayText.text = formattedTime;
            }
        }
    }

    private void CheckEventTick()
    {
        if (tickAccumulator >= tickInterval) //if enough time has passed, fire off a tick event
        {
            //Debug.Log("Event fired!");

            tickEvent?.Invoke(); //? checks if tickEvent is null.

            tickAccumulator -= tickInterval;
        }
    }
}
