using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Bills : MonoBehaviour
{
    private Scr_GameManager gameManager;

    private Scr_BillRow[] billRows;


    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
        billRows = GetComponentsInChildren<Scr_BillRow>(true);

    }

    private void OnEnable()
    {
        PopulateBills();
    }

    public void PopulateBills()
    {
        foreach (var row in billRows)
        {
            row.Show();
        }    
    }
}
