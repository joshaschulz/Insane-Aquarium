using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings")]
public class Scr_GameSettings : ScriptableObject
{
    [Header("Fishing Minigame")]
    public int toiletFishTicksToSpawnChance; //AllScenes/Stall/ClockSpawnFish
    public float toiletFishReelInSpeed; //AllScenes/Stall/Rod
    public float toiletFishLateralPullStrength; //AllScenes/Stall/Rod

    [Header("Customers")]
    public int fishyGuyTicksToSpawnChance; //AllScenes/FishyGuy
    public int fishyGuyTicksToExist; //AllScenes/FishyGuy
    public int customerTicksToSpawnChance; //AllScenes/Customer
    public int customerTicksToExist; //AllScenes/Customer

    [Header("Tick System")] //GameManager
    //real seconds per tick event
    public float secondsPerTickEvent;
    public int startHour = 6;
    public int startMinute = 0;
    public int endHour = 7;
    public int endMinute = 10;
    public float fastForwardNormal = 1;
    public float fastForwardFast = 2;
    public float fastForwardFastest = 5;

    [Header("Economy")]
    public int moneyAmount; //GameManager
    public int fishFood1Amount; //GameManager
    public int fishFood2Amount; //GameManager
    public int fishFood3Amount; //GameManager


    [Header("Fish")]
    [Header("Goldfish")]
    public int minutesUntilGrown_Goldfish;
    public int minutesUntilHungry_Goldfish;
    public int minutesUntilFreaky_Goldfish;
    public int minutesUntilPoop_Goldfish;
    public int minutesUntilDead_Goldfish;
    public float baseSpeed_Goldfish;
    public int baseFishCost_Goldfish;
    public int fishValue_Goldfish;

    [Header("Betta Fish")]
    public int minutesUntilGrown_BettaFish;
    public int minutesUntilHungry_BettaFish;
    public int minutesUntilFreaky_BettaFish;
    public int minutesUntilPoop_BettaFish;
    public int minutesUntilDead_BettaFish;
    public float baseSpeed_BettaFish;
    public int baseFishCost_BettaFish;
    public int fishValue_BettaFish;


    [Header("Piranha")]
    public int minutesUntilGrown_Piranha;
    public int minutesUntilHungry_Piranha;
    public int minutesUntilFreaky_Piranha;
    public int minutesUntilPoop_Piranha;
    public int minutesUntilDead_Piranha;
    public float baseSpeed_Piranha;
    public int baseFishCost_Piranha;
    public int fishValue_Piranha;


    [Header("Clownfish")]
    public int minutesUntilGrown_Clownfish;
    public int minutesUntilHungry_Clownfish;
    public int minutesUntilFreaky_Clownfish;
    public int minutesUntilPoop_Clownfish;
    public int minutesUntilDead_Clownfish;
    public float baseSpeed_Clownfish;
    public int baseFishCost_Clownfish;
    public int fishValue_Clownfish;


    [Header("Blue Tang")]
    public int minutesUntilGrown_BlueTang;
    public int minutesUntilHungry_BlueTang;
    public int minutesUntilFreaky_BlueTang;
    public int minutesUntilPoop_BlueTang;
    public int minutesUntilDead_BlueTang;
    public float baseSpeed_BlueTang;
    public int baseFishCost_BlueTang;
    public int fishValue_BlueTang;

}
