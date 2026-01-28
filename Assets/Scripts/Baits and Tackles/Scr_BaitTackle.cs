using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_BaitTackle : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public List<GameObject> baitList;
    public List<GameObject> tackleList;

    public List<TextMeshProUGUI> baitAmountTexts;

    public List<int> baitCosts;
    public List<int> tackleCosts;

    public List<int> baitListAmount;
    public List<int> tackleListAmount;

    public int currentBaitEquipped;
    public int currentTackleEquipped;


    [Header("Bait Factors")]
    public int earthwormNumOf10Minutes = 1;
    public float peanutButterSlowFactor = 0.8f;

    [Header("Tackle Factors")]
    public float leadBobberDrainFactor = 0.5f;

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
    }

    private void OnEnable()
    {
        RefreshBaitAndTackleVisuals();
    }
    public void AddBait(int index, int amount)
    {
        if (baitListAmount[index] != -1)
            baitListAmount[index] += amount;
        else
            baitListAmount[index] += amount + 1; //it was -1 (represents locked) so have to add another 1
    }

    public void SubtractBait(int index, int amount)
    {
        baitListAmount[index] -= amount;

        baitListAmount[index] = Mathf.Max(baitListAmount[index], 0);

        Debug.Log("New " + baitList[currentBaitEquipped].name + " bait amount: " + baitListAmount[currentBaitEquipped]);

        if (baitListAmount[index] == 0)
        {
            gameManager.notifications.Show("You're all out of " + baitList[currentBaitEquipped].name + " bait.", true);

            if (currentBaitEquipped == index)
            {
                baitList[currentBaitEquipped].transform.Find("Selected Border").gameObject.SetActive(false);
                currentBaitEquipped = -1;
            }
        }
    }

    public void AddTackle(int index)
    {
        tackleListAmount[index] = 1;
    }
    public void EquipBait(int index)
    {
        if (currentBaitEquipped != -1)
        {
            baitList[currentBaitEquipped].transform.Find("Selected Border").gameObject.SetActive(false);
        }

        if (currentBaitEquipped == index)
        {
            Debug.Log(baitList[currentBaitEquipped].name + " bait unequipped!");

            currentBaitEquipped = -1;
            return;
        }

        if (baitListAmount[index] > 0)
        {
            currentBaitEquipped = index;

            baitList[currentBaitEquipped].transform.Find("Selected Border").gameObject.SetActive(true);
            Debug.Log(baitList[currentBaitEquipped].name + " bait equipped!");
        }

    }

    public void EquipTackle(int index)
    {
        if (currentTackleEquipped != -1)
        {
            tackleList[currentTackleEquipped].transform.Find("Selected Border").gameObject.SetActive(false);
        }

        if (currentTackleEquipped == index)
        {
            Debug.Log(tackleList[currentTackleEquipped].name + " tackle unequipped!");

            currentTackleEquipped = -1;
            return;
        }

        if (tackleListAmount[index] > 0)
        {
            currentTackleEquipped = index;
            tackleList[currentTackleEquipped].transform.Find("Selected Border").gameObject.SetActive(true);
            Debug.Log(tackleList[currentTackleEquipped].name + " tackle equipped!");
        }


    }

    public void InitializeBaitsAndTackles()
    {
        currentBaitEquipped = -1;
        currentTackleEquipped = -1;

        baitListAmount = new List<int> { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        tackleListAmount = new List<int> { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        Image[] baitImages = transform.GetChild(0).GetComponentsInChildren<Image>(true);
        Image[] tackleImages = transform.GetChild(1).GetComponentsInChildren<Image>(true);

        foreach (Image img in baitImages)
        {
            Debug.Log(img.name);
            if (img.name.Contains("White Border"))
                continue;
            img.color = Color.black;
        }
        foreach (Image img in tackleImages)
        {
            Debug.Log(img.name);
            if (img.name.Contains("White Border"))
                continue;
            img.color = Color.black;
        }

        foreach (TextMeshProUGUI baitAmount in baitAmountTexts)
        {
            baitAmount.text = "";
        }
    }

    public void RefreshBaitAndTackleVisuals()
    {
        GameObject baits = transform.GetChild(0).gameObject;
        GameObject tackles = transform.GetChild(1).gameObject;

        for (int i = 0; i < baitListAmount.Count; i++)
        {
            if (currentBaitEquipped == i)
            {
                baits.transform.GetChild(i).transform.Find("Selected Border").gameObject.SetActive(true);
            }

            if (baitListAmount[i] != -1)
            {
                baitAmountTexts[i].text = baitListAmount[i].ToString();

                foreach (Image img in baits.transform.GetChild(i).GetComponentsInChildren<Image>(true))
                {
                    img.color = Color.white;
                }
            }
        }

        for (int i = 0; i < tackleListAmount.Count; i++)
        {
            if (currentTackleEquipped == i)
            {
                tackles.transform.GetChild(i).transform.Find("Selected Border").gameObject.SetActive(true);
            }

            if (tackleListAmount[i] != -1)
            {
                foreach (Image img in tackles.transform.GetChild(i).GetComponentsInChildren<Image>(true))
                {
                    img.color = Color.white;
                }
            }
        }
    }

    public void BuyAllBaitsAndTackles()
    {
        AddBait(0, 10);
        AddBait(1, 1);

        AddTackle(0);
        AddTackle(1);
    }
}
