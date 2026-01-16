using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Stress : MonoBehaviour
{

    private Scr_Fish fishScr;

    public enum StressFactor
    {
        Crowded,
        Lonely,
        Hungry,
        PredatorNearby,
        DirtyTank
    }


    public HashSet<StressFactor> activeStressFactors = new HashSet<StressFactor>();

    // Start is called before the first frame update
    void Start()
    {
        fishScr = GetComponent<Scr_Fish>();
    }

    public void CheckStress()
    {
        activeStressFactors.Clear();

        if (fishScr.isHungry)
        {
            activeStressFactors.Add(StressFactor.Hungry);
        }

        // Determine more stress factors


        fishScr.isStressed = activeStressFactors.Count > 0;
    }

}
