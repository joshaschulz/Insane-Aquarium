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

    public Sprite easyImage;
    public Sprite mediumImage;
    public Sprite hardImage;

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

        foreach (string saveName in saves)
        {
            Debug.Log("LOADED: " + saveName);
            int difficulty = int.Parse(saveName[0].ToString());
            Debug.Log("DIFFICULTY: " + difficulty);

            string businessName = saveName.Substring(1);
            Debug.Log("BUSINESS NAME: " + businessName);


            Button btn = Instantiate(saveButtonPrefab, saveListContent);

            btn.GetComponentInChildren<TextMeshProUGUI>().text = businessName;

            GameObject btnImages = btn.transform.Find("Images").gameObject;
            btnImages.transform.GetChild(difficulty).gameObject.SetActive(true);

            btn.onClick.AddListener(() =>
            {
                gameManager.businessName = businessName;
                gameManager.currentDifficulty = difficulty;
                //FindObjectOfType<Scr_SaveLoad>().LoadBusiness();
            });

            // DELETE BUTTON (child button inside the prefab)
            Button deleteBtn = btn.transform.Find("Delete Button").GetComponent<Button>();

            deleteBtn.onClick.AddListener(() =>
            {
                DeleteSave(businessName, difficulty);
            });
        }
    }

    public void DeleteSave(string businessName, int difficulty)
    {
        string path = Application.persistentDataPath + "/"  + difficulty + businessName + ".fishy";

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
