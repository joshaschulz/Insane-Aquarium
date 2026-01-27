using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_BaitTackle : MonoBehaviour
{
    public List<GameObject> baitList;
    public List<GameObject> tackleList;

    public List<int> baitCosts;
    public List<int> tackleCosts;

    public List<int> baitListAmount;
    public List<int> tackleListAmount;

    public int currentBaitEquipped;
    public int currentTackleEquipped;

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
    }

    public void AddTackle(int index)
    {
        tackleListAmount[index] = 1;
    }

    public void InitializeBaitsAndTackles()
    {
        baitListAmount = new List<int> { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        tackleListAmount = new List<int> { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        Image[] baitImages = transform.GetChild(0).GetComponentsInChildren<Image>(true);
        Image[] tackleImages = transform.GetChild(1).GetComponentsInChildren<Image>(true);

        foreach (Image img in baitImages)
        {
            img.color = Color.black;
        }
        foreach (Image img in tackleImages)
        {
            img.color = Color.black;
        }
    }

    public void RefreshBaitAndTackleVisuals()
    {
        GameObject baits = transform.GetChild(0).gameObject;
        GameObject tackles = transform.GetChild(1).gameObject;

        for (int i = 0; i < baitListAmount.Count; i++)
        {
            if (baitListAmount[i] != -1)
            {
                foreach (Image img in baits.transform.GetChild(i).GetComponentsInChildren<Image>(true))
                {
                    img.color = Color.white;
                }
            }
        }

        for (int i = 0; i < tackleListAmount.Count; i++)
        {
            if (tackleListAmount[i] != -1)
            {
                foreach (Image img in tackles.transform.GetChild(i).GetComponentsInChildren<Image>(true))
                {
                    img.color = Color.white;
                }
            }
        }
    }
}
