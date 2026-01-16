using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Stress : MonoBehaviour
{

    private Scr_GameManager gameManager;
    private Scr_Fish fishScr;

    public int minComfortablePopulation = 0;
    public int maxComfortablePopulation = 10;
    public bool maxIsInfinite = true;
    public bool comfortableWithPredators = false;
    public int comfortablePoopLevel = 75;

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
        gameManager = Scr_GameManager.GMinstance;
        fishScr = GetComponent<Scr_Fish>();
    }

    public void CheckStress()
    {
        activeStressFactors.Clear();


        List<GameObject> fishInTank = gameManager.GetAllFishInTank(fishScr.spawnTank);


        if (!maxIsInfinite && fishInTank.Count > maxComfortablePopulation)
        {
            activeStressFactors.Add(StressFactor.Crowded);
        }

        if (fishInTank.Count < minComfortablePopulation)
        {
            activeStressFactors.Add(StressFactor.Lonely);
        }

        if (fishScr.isHungry)
        {
            activeStressFactors.Add(StressFactor.Hungry);
        }

        foreach (GameObject otherFish in fishInTank)
        {
            // If an exotic fish ever eats a fish, add it here...
            if (otherFish.GetComponent<Scr_Fish>().fishDiet.Contains(fishScr.thisPrefab))
            {
                activeStressFactors.Add(StressFactor.PredatorNearby);
                break;
            }
        }

        if (gameManager.GetPoopLevel(new Vector2(fishScr.spawnTank.transform.position.x, fishScr.spawnTank.transform.position.y)) > comfortablePoopLevel)
        {
            activeStressFactors.Add(StressFactor.DirtyTank);
        }

        fishScr.isStressed = activeStressFactors.Count > 0;
    }

}
