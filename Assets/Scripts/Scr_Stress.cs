using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Stress : MonoBehaviour
{

    private Scr_GameManager gameManager;

    public enum StressFactor
    {
        Crowded,
        Lonely,
        Hungry,
        PredatorNearby,
        DirtyTank
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
    }

    public HashSet<StressFactor> CheckStress(GameObject tank, GameObject fishPrefab)
    {
        HashSet<StressFactor> activeStressFactors = new HashSet<StressFactor>();

        Scr_Fish fishPrefabScr = fishPrefab.GetComponent<Scr_Fish>();
        Scr_Tank tankScr = tank.GetComponent<Scr_Tank>();

        int totalFish = 0;
        foreach (var kvp in tankScr.fishCountBySpeciesDict)
        {
            totalFish += kvp.Value;
        }

        // If the total population of the tank is over the comfortable limit...
        if (!fishPrefabScr.maxIsInfinite && totalFish > ((gameManager.skills.currentFishkeepingSkills[3]) ? fishPrefabScr.maxComfortableTankPopulation * 1.5f : fishPrefabScr.maxComfortableTankPopulation))
        {
            activeStressFactors.Add(StressFactor.Crowded);
        }


        if (tankScr.fishCountBySpeciesDict[fishPrefab] < fishPrefabScr.minComfortableSameSpeciesPopulation)
        {
            activeStressFactors.Add(StressFactor.Lonely);
        }

        foreach (var kvp in tankScr.fishCountBySpeciesDict)
        {
            if (kvp.Key.GetComponent<Scr_Fish>().fishDiet.Contains(fishPrefab) && kvp.Value > 0)
            {
                activeStressFactors.Add(StressFactor.PredatorNearby);
                break;
            }
        }


        // If the tank's poop level is over the limit...
        if (gameManager.GetPoopLevel(new Vector2(tank.transform.position.x, tank.transform.position.y)) > fishPrefabScr.comfortablePoopLevel)
        {
            activeStressFactors.Add(StressFactor.DirtyTank);
        }

        return activeStressFactors;
    }

}
