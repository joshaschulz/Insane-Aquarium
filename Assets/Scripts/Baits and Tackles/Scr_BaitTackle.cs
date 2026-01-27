using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_BaitTackle : MonoBehaviour
{
    public List<Sprite> baitSprites;
    public List<Sprite> tackleSprites;

    public int currentBaitEquipped;
    public int currentTackleEquipped;

    public List<int> baitList;
    public List<int> tackleList;

    private void OnEnable()
    {
        RefreshBaitAndTackleVisuals();
    }
    public void AddBait(int index, int amount)
    {
        if (baitList[index] != -1)
            baitList[index] += amount;
        else
            baitList[index] += amount + 1; //it was -1 (represents locked) so have to add another 1
    }

    public void AddTackle(int index)
    {
        tackleList[index] = 1;
    }

    public void RefreshBaitAndTackleVisuals()
    {
        Image[] baitImages = transform.GetChild(0).GetComponentsInChildren<Image>(true);
        Image[] tackleImages = transform.GetChild(1).GetComponentsInChildren<Image>(true);

        foreach (Image img in baitImages)
        {
            // Decide here if the bait should be unlocked...
            bool unlocked = false;

            img.color = unlocked ? Color.white : Color.black;
        }
        foreach (Image img in tackleImages)
        {
            // Decide here if the tackle should be unlocked...
            bool unlocked = false;

            img.color = unlocked ? Color.white : Color.black;
        }
    }
}
