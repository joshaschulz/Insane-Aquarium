using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_GameManager : MonoBehaviour
{
    public static Scr_GameManager GMinstance;

    private Camera _Camera;

    public int moneyAmount;
    public TextMeshProUGUI moneyText;

    public GameObject foodPellet;
    public GameObject goldCoin;
    public int feedingCost;

    public int goldCoinWorth;
    private int fishIdCounter = 0;

    //public GameObject goldfish;
    //public GameObject bettaFish;
    public Vector3 spawnPosition;

    public List<GameObject> foodPelletList = new List<GameObject>();
    public List<GameObject> fishList = new List<GameObject>();


    private void Awake()
    {
        if (GMinstance == null)
        {
            GMinstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _Camera = Camera.main;

        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "$" + moneyAmount.ToString();
    }

    public int GetMoneyAmount()
    {
        return moneyAmount;
    }
    public void SetMoneyAmount(int _newMoneyAmount)
    {
        moneyAmount = _newMoneyAmount;
        UpdateMoneyText();
    }

    public void DropFood()
    {
        if (GetMoneyAmount() >= feedingCost)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject newfoodPellet = Instantiate(foodPellet, mouseWorldPosition, Quaternion.identity);

            foodPelletList.Add(newfoodPellet);

            SetMoneyAmount(GetMoneyAmount() - feedingCost);

            Debug.Log(fishList.Count);
            if (fishList.Count > 0)
                CalculateClosestPellet();
        }
        else
        {
            Debug.Log("Insufficient Money to Feed");
        }
    }
    public void SpawnFish(GameObject _fishToSpawn)
    {

        GameObject newFish = Instantiate(_fishToSpawn, spawnPosition, Quaternion.identity);
        fishList.Add(newFish);

        newFish.GetComponent<Scr_Move>().fishId = fishIdCounter;
        fishIdCounter++;

        Debug.Log(newFish.GetComponent<Scr_Move>().fishId);

    }

    public void CalculateClosestPellet()
    {
        for (int i = 0; i < fishList.Count; i++)
        {
            GameObject closestFoodPellet = foodPelletList[0];
            float minDistance = float.MaxValue;
            for (int j = 0; j < foodPelletList.Count; j++)
            {
                float distance = Vector2.Distance(fishList[i].transform.position, foodPelletList[j].transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestFoodPellet = foodPelletList[j];
                }
            }
            fishList[i].GetComponent<Scr_Move>().closestPellet = closestFoodPellet;

        }
    }
}
