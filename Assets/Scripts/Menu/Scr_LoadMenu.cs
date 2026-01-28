using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_LoadMenu : MonoBehaviour
{
    [SerializeField] Transform saveListContent;   // the object inside border
    [SerializeField] Button saveButtonPrefab;
    [SerializeField] Scr_GameManager gameManager;

    void OnEnable()
    {
        PopulateList();
    }

    void PopulateList()
    {
        // clear old
        for (int i = saveListContent.childCount - 1; i >= 0; i--)
            Destroy(saveListContent.GetChild(i).gameObject);

        // get save names from YOUR save system
        string[] saves = Scr_SaveSystem.GetAllBusinessSaves();

        foreach (string businessName in saves)
        {
            Button btn = Instantiate(saveButtonPrefab, saveListContent);

            btn.GetComponentInChildren<TextMeshProUGUI>().text = businessName;

            btn.onClick.AddListener(() =>
            {
                gameManager.businessName = businessName;
                //FindObjectOfType<Scr_SaveLoad>().LoadBusiness();
            });
        }
    }
}
