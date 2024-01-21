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
    public int feedingCost;

    public int goldCoinWorth;

    public GameObject goldfish;
    public Vector3 spawnPosition;

    public List<GameObject> foodPelletList = new List<GameObject>();

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
