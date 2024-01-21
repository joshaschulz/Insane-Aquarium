using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_GameManager : MonoBehaviour
{
    public static Scr_GameManager GMinstance;

    public int moneyAmount;
    public  TextMeshProUGUI moneyText;

    public int goldCoinWorth;

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
}
