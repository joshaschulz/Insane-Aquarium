using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BaitTackle : MonoBehaviour
{
    public List<Sprite> baitSprites;
    public List<Sprite> tackleSprites;

    public int currentBaitEquipped;
    public int currentTackleEquipped;

    public List<int> baitList;
    public List<int> tackleList;


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
}
