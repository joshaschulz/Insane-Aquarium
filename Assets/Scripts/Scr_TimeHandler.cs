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

    private Scr_GameManager gameManager;

    public float tickInterval = 60; //every 10 in game minutes (600 in game seconds) a tick event will fire
    private float tickAccumulator; //tick will trigger every __ minutes then reset.


    [Header("Time Settings")]
    public float secPerTickEvent; //real seconds per tick event
    private float timeScale; //in-game seconds per real second

    private int numTicksPer10Min;

    public int startHour;
    public int startMinute;
    public int endHour;
    public int endMinute;

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
        gameManager = gameObject.GetComponent<Scr_GameManager>();

        ChangeGameSettings();

        numTicksPer10Min = (int)(600 / tickInterval);

        gameManager.tickEventsPer10Min = numTicksPer10Min;
        secPerTickEvent /= numTicksPer10Min;

        startTimeInSeconds = startHour * 3600 + startMinute * 60;
        endTimeInSeconds = endHour * 3600 + endMinute * 60;

        timeScale = tickInterval / secPerTickEvent;

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

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        secPerTickEvent = settings.secondsPerTickEvent;
        startHour = settings.startHour;
        startMinute = settings.startMinute;
        endHour = settings.endHour;
        endMinute = settings.endMinute;
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

            int hours = ((int)gameSeconds / 3600) % 24;
            int minutes = ((int)gameSeconds / 60) % 60;

            //Debug.Log($"I JUST SENT A TICK EVENT at {hours:00}:{minutes:00}");

            tickAccumulator -= tickInterval;
        }
    }

    public void UpdateTimeScale(float newSecPerTickEvent)
    {
        timeScale = tickInterval / newSecPerTickEvent;
        timeScale *= numTicksPer10Min;

    }
}
