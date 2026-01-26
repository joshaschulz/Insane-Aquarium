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
    public int[] fishIsLegendary;
    public int[] fishIsWild;
    public int[] fishIsRadiated;
    public int[] fishIsMutated;
    public int[] fishIsGrown;
    public float[] fishGrowCounts;
    public float[] fishFreakCounts;
    public int[] fishSpecies; //[1, 3, 5, 4, ...] index of game manager fish prefabs
    public float[] fishHues;

    public string[] exoticFishNames;
    public int[] exoticFishTanks;
    public int[] exoticFishSpecies;

    public int[] starfishLegCounts; //[4, 6, 1, ...] number of legs active in each starfish

    public string[] fishBagsNames;
    public int[] fishBagsIsLegendary;
    public int[] fishBagsIsWild;
    public int[] fishBagsIsRadiated;
    public int[] fishBagsIsMutated;
    public int[] fishBagsIsGrown;
    public float[] fishBagsGrowCounts;
    public float[] fishBagsFreakCounts;
    public int[] fishBagsSpecies; //[1, 3, 5, 4, ...] index of game manager fish prefabs
    public float[] fishBagsHues;

    public string[] fishBagsExoticFishNames;
    public int[] fishBagsExoticFishSpecies;

    public int[] fishBagsStarfishLegCounts; //[4, 6, 1, ...] number of legs active in each starfish

    public int[] fishFoodAmounts; //[92, 182, 0]
    public int[] structureAmounts; //[4, 1, 0, 6]
    public int[] structuresInScene; //[1, 2, 1, 3, ...] index of game manager structure prefabs
    public int[] structuresPlacedOnRight;
    public int[] baitsUnlocked; //[1, 0, 0, 1, ...] 1 if unlocked
    public int[] tacklesUnlocked;
    public int[] baitAmounts;
    public int[] tackleAmounts;
    public int[] skills; //*bools [1, 0, 0, 1, 0, 0, ...]
    public int[] fishpediaEntries; //*bools [1, 0, 0, 1, 0 ,0 ,1]
    public int[] fishpediaNumCaughtAmounts;

    public float[] structurePositions; //*vector3 [x0, y0, z0, x1, y1, z1, ...]


    public Scr_ProgressData (Scr_GameManager gameManager)
    {
        day = gameManager.currentDay;
        moneyAmount = gameManager.moneyAmount;
        loanAmount = gameManager.loanAmount;
        baitEquipped = 0; //PLACEHOLDER
        tackleEquipped = 0; //PLACEHOLDER

        int numFish = 0;
        int numExoticFish = 0;
        int numStarfish = 0;
        List<Scr_Fish> fishList = new List<Scr_Fish>();
        List<Scr_ExoticFish> exoticFishList = new List<Scr_ExoticFish>();
        List<Scr_Starfish> starfishList = new List<Scr_Starfish>();

        foreach (var kvp in gameManager.foodFishDictionary)
        {
            if (kvp.Key.GetComponent<Scr_Fish>() != null)
            {
                numFish++;
                fishList.Add(kvp.Key.GetComponent<Scr_Fish>());
            }
            else if (kvp.Key.GetComponent<Scr_ExoticFish>() != null)
            {
                numExoticFish++;
                exoticFishList.Add(kvp.Key.GetComponent<Scr_ExoticFish>());

                if (kvp.Key.GetComponent<Scr_Starfish>() != null)
                {
                    numStarfish++;
                    starfishList.Add(kvp.Key.GetComponent<Scr_Starfish>());
                }
            }
        }

        fishNames = new string[numFish];
        fishTanks = new int[numFish];
        fishIsLegendary = new int[numFish];
        fishIsWild = new int[numFish];
        fishIsRadiated = new int[numFish];
        fishIsMutated = new int[numFish];
        fishIsGrown = new int[numFish];
        fishGrowCounts = new float[numFish];
        fishFreakCounts = new float[numFish];
        fishSpecies = new int[numFish];
        fishHues = new float[numFish];
        fishpediaNumCaughtAmounts = new int[numFish];

        exoticFishNames = new string[numExoticFish];
        exoticFishTanks = new int[numExoticFish];
        exoticFishSpecies = new int[numExoticFish];

        starfishLegCounts = new int[numStarfish];

        for (int i = 0; i < fishList.Count; i++)
        {
            fishNames[i] = fishList[i].name;
            fishTanks[i] = gameManager.allTanks.IndexOf(fishList[i].spawnTank);
            fishIsLegendary[i] = fishList[i].legendary ? 1 : 0;
            fishIsWild[i] = fishList[i].wild ? 1 : 0;
            fishIsRadiated[i] = fishList[i].radiated ? 1 : 0;
            fishIsMutated[i] = fishList[i].mutated ? 1 : 0;
            fishIsGrown[i] = fishList[i].grown ? 1 : 0;
            fishGrowCounts[i] = fishList[i].growCount;
            fishFreakCounts[i] = fishList[i].freakCount;
            fishSpecies[i] = gameManager.allFishPrefabs.IndexOf(fishList[i].thisPrefab);
            fishHues[i] = fishList[i].GetComponent<Scr_FishHue>().GetHue();
            fishpediaNumCaughtAmounts[i] = fishList[i].numberCaught;
        }

        for (int i = 0; i < exoticFishList.Count; i++)
        {
            exoticFishNames[i] = "PLACEHOLDER";
            exoticFishSpecies[i] = 0; //PLACEHOLDER
        }

        for (int i = 0; i < starfishList.Count; i++)
        {
            exoticFishTanks[i] = gameManager.allTanks.IndexOf(starfishList[i].spawnTank); //PLACEHOLDER, should be in exotic fish list

            foreach (GameObject leg in starfishList[i].starfishLegs)
            {
                if (leg.activeSelf)
                    starfishLegCounts[i]++;
            }
        }

        fishFoodAmounts = new int[gameManager.allFishFoodPrefabs.Count];

        for (int i = 0; i < gameManager.allFishFoodPrefabs.Count; i++)
        {
            fishFoodAmounts[i] = gameManager.GetFishFoodAmount(gameManager.allFishFoodPrefabs[i]);
        }

        structureAmounts = new int[gameManager.allStructurePrefabs.Count];

        for (int i = 0; i < gameManager.allStructurePrefabs.Count; i++)
        {
            structureAmounts[i] = gameManager.GetStructureAmount(gameManager.allStructurePrefabs[i]);
        }

        baitsUnlocked = new int[0]; //PLACEHOLDER
        tacklesUnlocked = new int[0]; //PLACEHOLDER
        baitAmounts = new int[0]; //PLACEHOLDER
        tackleAmounts = new int[0]; //PLACEHOLDER

        skills = new int[25];

        int j = 0;
        foreach (var kvp in gameManager.skills.skillMap)
        {
            foreach (bool skill in kvp.Value)
            {
                skills[j] = skill ? 1 : 0;
                j++;
            }
        }

        fishpediaEntries = new int[gameManager.fishpedia.GetComponent<Scr_Fishpedia>().entries.Length];

        for (int i = 0; i < gameManager.fishpedia.GetComponent<Scr_Fishpedia>().entries.Length; i++)
        {
            fishpediaEntries[i] = gameManager.fishpedia.GetComponent<Scr_Fishpedia>().entries[i].activeSelf ? 1 : 0;
        }

        structuresInScene = new int[gameManager.structuresInScene.Count];
        structuresPlacedOnRight = new int[gameManager.structuresInScene.Count];
        structurePositions = new float[gameManager.structuresInScene.Count * 3];

        for (int i = 0; i < gameManager.structuresInScene.Count; i++)
        {
            structuresInScene[i] = gameManager.allStructurePrefabs.IndexOf(gameManager.structuresInScene[i].GetComponent<Scr_StructurePlacementRules>().thisPrefab);
            Debug.Log(gameManager.structuresInScene[i].name + " index: " + structuresInScene[i]);
            structuresPlacedOnRight[i] = gameManager.structuresInScene[i].GetComponent<Scr_StructurePlacementRules>().placedOnRight ? 1 : 0;
        }

        int k = 0;
        for (int i = 0; i < gameManager.structuresInScene.Count * 3; i+= 3)
        {
            structurePositions[i] = gameManager.structuresInScene[k].transform.position.x;
            structurePositions[i + 1] = gameManager.structuresInScene[k].transform.position.y;
            structurePositions[i + 2] = gameManager.structuresInScene[k].transform.position.z;
            k++;
        }

        int bagsActive = 0;
        for (int i = 0; i < gameManager.allBagButtons.Count; i++)
        {
            if (gameManager.allBagButtons[i].activeSelf)
                bagsActive++;
        }

        /*
    public string[] fishBagsNames;
    public int[] fishBagsIsLegendary;
    public int[] fishBagsIsWild;
    public int[] fishBagsIsRadiated;
    public int[] fishBagsIsMutated;
    public int[] fishBagsIsGrown;
    public float[] fishBagsGrowCounts;
    public float[] fishBagsFreakCounts;
    public int[] fishBagsSpecies; //[1, 3, 5, 4, ...] index of game manager fish prefabs
    public float[] fishBagsHues;

    public string[] fishBagsExoticFishNames;
    public int[] fishBagsExoticFishSpecies;

    public int[] fishBagsStarfishLegCounts; //[4, 6, 1, ...] number of legs active in each starfish
         */

        fishBagsNames = new string[bagsActive];
        fishBagsIsLegendary = new int[bagsActive];
        fishBagsIsWild = new int[bagsActive];
        fishBagsIsRadiated = new int[bagsActive];
        fishBagsIsMutated = new int[bagsActive];
        fishBagsIsGrown = new int[bagsActive];
        fishBagsGrowCounts = new float[bagsActive];
        fishBagsFreakCounts = new float[bagsActive];
        fishBagsSpecies = new int[bagsActive];
        fishBagsHues = new float[bagsActive];

        fishBagsExoticFishNames = new string[bagsActive];
        fishBagsExoticFishSpecies = new int[bagsActive];
        fishBagsStarfishLegCounts = new int[bagsActive];

        for (int i = 0; i < bagsActive; i++)
        {
            if (gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>() != null)
            {
                fishBagsNames[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().name;
                fishBagsIsLegendary[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().legendary ? 1 : 0;
                fishBagsIsWild[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().wild ? 1 : 0;
                fishBagsIsRadiated[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().radiated ? 1 : 0;
                fishBagsIsMutated[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().mutated ? 1 : 0;
                fishBagsIsGrown[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().grown ? 1 : 0;
                fishBagsGrowCounts[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().growCount;
                fishBagsFreakCounts[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().freakCount;
                fishBagsSpecies[i] = gameManager.allFishPrefabs.IndexOf(gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Fish>().thisPrefab);
                fishBagsHues[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_FishHue>().GetHue();
                fishBagsExoticFishSpecies[i] = -1;
            }
            else if (gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_ExoticFish>() != null)
            {
                fishBagsExoticFishNames[i] = gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_ExoticFish>().name;
                fishBagsExoticFishSpecies[i] = gameManager.allExoticFishPrefabs.IndexOf(gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Starfish>().thisPrefab); //PLACEHOLDER STARFISH
                fishBagsSpecies[i] = -1;

                if (gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Starfish>() != null)
                {
                    foreach (GameObject leg in gameManager.allBagButtons[i].transform.GetChild(0).gameObject.GetComponent<Scr_Starfish>().starfishLegs)
                    {
                        if (leg.activeSelf)
                            fishBagsStarfishLegCounts[i]++;
                    }
                }

            }
        }

    }
}
