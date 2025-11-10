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
    }
    public void SetName(int _id, string _newName)
    {
        entries[_id].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _newName;
    }
    public void SetImage(int _id, Sprite _newImage)
    {
        entries[_id].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _newImage;
    }
    public void SetDescription(int _id, string _newDescription)
    {
        entries[_id].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = _newDescription;
    }
    public void SetStat(int _id, int _statID, int _newValue)
    {
        string entryStatNumbersText = entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text;
        string[] EntryStatNumbers = entryStatNumbersText.Split('\n'); // Split at each line break

        EntryStatNumbers[_statID] = _newValue.ToString();

        entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text = string.Join("\n", EntryStatNumbers);
    }

    public int GetStat(int _id, int _statID)
    {
        string entryStatNumbersText = entries[_id].transform.GetChild(2).Find("Statistics Numbers").GetComponent<TextMeshProUGUI>().text;
        string[] EntryStatNumbers = entryStatNumbersText.Split('\n'); // Split at each line break
        
        return int.Parse(EntryStatNumbers[_statID]);
    }

    public void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;
    }
}
