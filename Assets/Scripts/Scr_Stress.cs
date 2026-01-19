using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Stress : MonoBehaviour
{

    private Scr_GameManager gameManager;
    private Scr_Fish fishScr;

    public int minComfortableSameSpeciesPopulation = 0;
    public int maxComfortableTankPopulation = 10;
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


        // If the total population of the tank is over the comfortable limit...
        if (!maxIsInfinite && fishInTank.Count > maxComfortableTankPopulation)
        {
            activeStressFactors.Add(StressFactor.Crowded);
        }


        // If the population of this species is under the comfortable limit...
        List<GameObject> sameSpeciesInTank = new List<GameObject>();
        foreach (GameObject otherFish in fishInTank)
        {
            if (otherFish.CompareTag(fishScr.tag))
            {
                sameSpeciesInTank.Add(otherFish);
            }
        }
        if (sameSpeciesInTank.Count < minComfortableSameSpeciesPopulation)
        {
            activeStressFactors.Add(StressFactor.Lonely);
        }


        // If the fish is hungry...
        if (fishScr.isHungry)
        {
            activeStressFactors.Add(StressFactor.Hungry);
        }


        // If a predator is nearby...
        foreach (GameObject otherFish in fishInTank)
        {
            // If an exotic fish ever eats a fish, add it here...
            if (otherFish.GetComponent<Scr_Fish>())
            {
                if (otherFish.GetComponent<Scr_Fish>().fishDiet.Contains(fishScr.thisPrefab))
                {
                    activeStressFactors.Add(StressFactor.PredatorNearby);
                    break;
                }
            }

        }


        // If the tank's poop level is over the limit...
        if (gameManager.GetPoopLevel(new Vector2(fishScr.spawnTank.transform.position.x, fishScr.spawnTank.transform.position.y)) > comfortablePoopLevel)
        {
            activeStressFactors.Add(StressFactor.DirtyTank);
        }


        fishScr.isStressed = activeStressFactors.Count > 0;
    }

}
