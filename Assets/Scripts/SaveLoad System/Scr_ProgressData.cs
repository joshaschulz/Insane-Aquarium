using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scr_ProgressData
{
    public int day;
    public int moneyAmount;
    public int loanAmount;
    public int baitEquipped; //index in bait list
    public int tackleEquipped; //index in tackle list

    public string[] fishNames;

    public int[] fishTanks; //[1, 0, 0, 1, 1, 1, ...] 0 for foreground tank, 1 for background tank 
    public int[] fishFoodAmounts; //[92, 182, 0]
    public int[] structureAmounts; //[4, 1, 0, 6]
    public int[] baitAmounts;
    public int[] tackleAmounts;
    public int[] skills; //*bools [1, 0, 0, 1, 0, 0, ...]
    public int[] fishpediaEntries; //*bools [1, 0, 0, 1, 0 ,0 ,1]
    public int[] fishpediaNumCaughtAmounts;

    public float[] fishHues;
    public float[] structuresPositions; //*vector3 [x0, y0, z0, x1, y1, z1, ...]

}
