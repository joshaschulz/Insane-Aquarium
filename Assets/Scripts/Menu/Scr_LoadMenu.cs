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

            // DELETE BUTTON (child button inside the prefab)
            Button deleteBtn = btn.transform.Find("Delete Button").GetComponent<Button>();

            deleteBtn.onClick.AddListener(() =>
            {
                DeleteSave(businessName);
            });
        }
    }

    public void DeleteSave(string businessName)
    {
        string path = Application.persistentDataPath + "/" + businessName + ".fishy";

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Deleted save: " + businessName);
        }
        else
        {
            Debug.LogWarning("Tried to delete save but file not found: " + path);
        }

        // refresh list after deletion
        PopulateList();
    }
}
