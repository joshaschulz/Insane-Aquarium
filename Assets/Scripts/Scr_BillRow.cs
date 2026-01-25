using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_BillRow : MonoBehaviour
{
    public TextMeshProUGUI amountText;

    public void Show()
    {
        if (amountText.text == "0")
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

    }
}
