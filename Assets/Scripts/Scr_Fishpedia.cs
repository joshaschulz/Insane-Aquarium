using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_Fishpedia : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public GameObject[] entries;
    public Button[] buttons;
    public GameObject buttonsPanel;


    public void EnableEntryButton(int _id)
    {
        buttons[_id].gameObject.SetActive(true);
    }
    public void ToggleEntry(int _id)
    {

        if (!entries[_id].activeSelf)
        {
            DisableEntries();
            entries[_id].SetActive(true);
        }
        else
        {
            DisableEntries();
        }

    }
    public void DisableEntries()
    {
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].SetActive(false);

            // Set the entry image to the side fish
            entries[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            entries[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        }
    }

    /*
    public void SetName(int _id, string _newName)
    {
        entries[_id].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _newName;
    }
    public void SetImages(int _id, Sprite _newSideImage, Sprite _newFrontImage)
    {
        entries[_id].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _newSideImage;
        entries[_id].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = _newFrontImage;
    }
    public void SetDescription(int _id, string _newDescription)
    {
        entries[_id].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = _newDescription;
    }
    public int GetStat(int _id, int _statID)
    {
        string entryStatNumbersText = entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text;
        string[] EntryStatNumbers = entryStatNumbersText.Split('\n'); // Split at each line break
        
        return int.Parse(EntryStatNumbers[_statID]);
    }
    public void SetStat(int _id, int _statID, int _newValue)
    {
        string entryStatNumbersText = entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text;
        string[] EntryStatNumbers = entryStatNumbersText.Split('\n'); // Split at each line break

        EntryStatNumbers[_statID] = _newValue.ToString();

        entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text = string.Join("\n", EntryStatNumbers);
    }
    */
    public void ToggleEntryImage()
    {
        Transform activeEntry = null;
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].activeSelf)
                activeEntry = entries[i].transform;
        }

        if (activeEntry.GetChild(0).GetChild(0).gameObject.activeSelf)
        {
            activeEntry.GetChild(0).GetChild(0).gameObject.SetActive(false);
            activeEntry.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            activeEntry.GetChild(0).GetChild(0).gameObject.SetActive(true);
            activeEntry.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_GenUI3, 0.1f, 1.2f);
    }
    private void OnDisable()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_GenUI3, 0.08f, 0.8f);
        DisableEntries();
        buttonsPanel.SetActive(true);
    }
    public void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;
    }
}
