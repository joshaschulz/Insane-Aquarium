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
    public  TextMeshProUGUI moneyText;

    public GameObject foodPellet;
    public int feedingCost;

    public int goldCoinWorth;

    public GameObject goldfish;
    public Vector3 spawnPosition;

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

            Vector3 mousePixelPos = Input.mousePosition;

            mousePixelPos.z = 20f;

            Vector3 mouseWorldPosition = _Camera.ScreenToWorldPoint(mousePixelPos);

            mouseWorldPosition.z = 0;

            Instantiate(foodPellet, mouseWorldPosition, Quaternion.identity);

            SetMoneyAmount(GetMoneyAmount() - feedingCost);
        }
        else
        {
            Debug.Log("Insufficient Money to Feed");
        }
    }
    public void SpawnFish()
    {
        Instantiate(goldfish, spawnPosition, Quaternion.identity);
    }
}
