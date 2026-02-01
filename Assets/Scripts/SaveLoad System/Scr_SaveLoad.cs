using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_SaveLoad : MonoBehaviour
{
    public Scr_GameManager gameManager;

    //public TextMeshProUGUI businessName;

    public void SaveBusiness()
    {
        gameManager.notifications.Show("Game Saved!", false);
        Scr_SaveSystem.SaveBusiness(gameManager, gameManager.businessName);
    }

    public void LoadBusiness()
    {
        Scr_ProgressData progress = Scr_SaveSystem.LoadBusiness(gameManager.businessName);

        gameManager.currentDay = progress.day;
        gameManager.moneyAmount = progress.moneyAmount;
        gameManager.loanAmount = progress.loanAmount;
        gameManager.baitAndTackle.currentBaitEquipped = progress.baitEquipped;
        gameManager.baitAndTackle.currentTackleEquipped = progress.tackleEquipped;
        gameManager.SetOscarState(progress.oscarState);
        gameManager.currentDifficulty = progress.currentDifficulty;
        gameManager.tutorials.tutorialCompleted = progress.tutorialsCompleted == 1 ? true : false;

        gameManager.UpdateSceneTexts();
        gameManager.UpdateGameSettingsFromDifficulty();

        for (int i = 0; i < progress.fishSpecies.Length; i++)
        {
            GameObject newFish = gameManager.SpawnFish(gameManager.allFishPrefabs[progress.fishSpecies[i]], gameManager.allTanks[progress.fishTanks[i]]);
            Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();

            newFishScript.name = progress.fishNames[i];
            newFishScript.legendary = progress.fishIsLegendary[i] == 1 ? true : false;
            newFishScript.wild = progress.fishIsWild[i] == 1 ? true : false;
            newFishScript.radiated = progress.fishIsRadiated[i] == 1 ? true : false;
            newFishScript.mutated = progress.fishIsMutated[i] == 1 ? true : false;
            newFishScript.grown = progress.fishIsGrown[i] == 1 ? true : false;
            newFishScript.generation = progress.fishGenerations[i];
            newFishScript.growCount = progress.fishGrowCounts[i];
            newFishScript.freakCount = progress.fishFreakCounts[i];
            newFishScript.hungerCount = progress.fishHungerCounts[i];
            newFishScript.poopCount = progress.fishPoopCounts[i];
            newFishScript.GetComponent<Scr_FishHue>().SetHue(progress.fishHues[i]);
            //fishpediaNumCaughtAmounts PALCEHOLDER

            gameManager.UpdateSpawnedLoadingFish(newFish);

        }

        int j = 0;
        for (int i = 0; i < progress.exoticFishSpecies.Length; i++)
        {
            GameObject newFish = gameManager.SpawnStarFish(gameManager.allExoticFishPrefabs[progress.exoticFishSpecies[i]], gameManager.allTanks[progress.exoticFishTanks[i]]);
            Scr_Starfish newFishScript = newFish.GetComponent<Scr_Starfish>();

            newFishScript.name = progress.exoticFishNames[i];

            if (newFish.GetComponent<Scr_Starfish>() != null)
            {
                Scr_Starfish starfishScript = newFish.GetComponent<Scr_Starfish>();

                int legsToDisable = 4 - progress.starfishLegCounts[j];

                for (int k = 0; k < legsToDisable; k++)
                {
                    starfishScript.starfishLegs[k].SetActive(false);
                }

                j++;
            }
        }

        for (int i = 0; i < progress.fishFoodAmounts.Length; i++)
        {
            gameManager.SetFishFoodAmount(gameManager.allFishFoodPrefabs[i], progress.fishFoodAmounts[i]);
        }

        for (int i = 0; i < progress.structureAmounts.Length; i++)
        {
            gameManager.SetStructureAmount(gameManager.allStructurePrefabs[i], progress.structureAmounts[i]);
        }

        for (int i = 0; i < progress.baitAmounts.Length; i++)
        {
            gameManager.baitAndTackle.baitListAmount[i] = progress.baitAmounts[i];
            gameManager.baitAndTackle.tackleListAmount[i] = progress.tackleAmounts[i];
        }

        int index = 0;

        foreach (var kvp in gameManager.skills.skillMap)
        {
            bool[] skillRow = kvp.Value; // length 5

            for (int i = 0; i < skillRow.Length; i++)
            {
                skillRow[i] = progress.skills[index] == 1;
                index++;
            }
        }

        gameManager.skills.UpdateSkillsOnLoad();
        gameManager.UpdateSkillObjects();

        for (int i = 0; i < progress.fishpediaButtons.Length; i++)
        {
            gameManager.fishpedia.GetComponent<Scr_Fishpedia>().buttons[i].gameObject.SetActive(progress.fishpediaButtons[i] == 1);
        }

        for (int i = 0; i < progress.structuresInScene.Length; i++)
        {
            int posIndex = i * 3;
            Vector3 pos = new Vector3(progress.structurePositions[posIndex], progress.structurePositions[posIndex + 1], progress.structurePositions[posIndex + 2]);
            Debug.Log("structure: " + progress.structuresInScene[i]);
            gameManager.DropStructure(gameManager.allStructurePrefabs[progress.structuresInScene[i]], pos, progress.structuresPlacedOnRight[i] == 1, true);
        }

        for (int i = 0; i < progress.fishBagsSpecies.Length; i++)
        {
            if (progress.fishBagsSpecies[i] != -1) //fish in that bag slot is not exotic
            {
                GameObject newFish = gameManager.SpawnLoadingFish(gameManager.allFishPrefabs[progress.fishBagsSpecies[i]], gameManager.fishWaitingArea);
                Scr_Fish newFishScript = newFish.GetComponent<Scr_Fish>();

                newFishScript.name = progress.fishBagsNames[i];
                newFishScript.legendary = progress.fishBagsIsLegendary[i] == 1;
                newFishScript.wild = progress.fishBagsIsWild[i] == 1;
                newFishScript.radiated = progress.fishBagsIsRadiated[i] == 1;
                newFishScript.mutated = progress.fishBagsIsMutated[i] == 1;
                newFishScript.grown = progress.fishBagsIsGrown[i] == 1;
                newFishScript.generation = progress.fishBagsGenerations[i];
                newFishScript.growCount = progress.fishGrowCounts[i];
                newFishScript.freakCount = progress.fishBagsFreakCounts[i];
                newFishScript.hungerCount = progress.fishHungerCounts[i];
                newFishScript.poopCount = progress.fishBagsPoopCounts[i];
                newFishScript.GetComponent<Scr_FishHue>().SetHue(progress.fishBagsHues[i]);

                gameManager.BagLoadingFish(newFish);

                //gameManager.BagAFish(newFish);
            }
            else
            {
                GameObject newFish = gameManager.SpawnTempStarfish(gameManager.allExoticFishPrefabs[progress.fishBagsExoticFishSpecies[i]], gameManager.fishWaitingArea);
                Scr_Starfish newFishScript = newFish.GetComponent<Scr_Starfish>(); //PLACEHOLDER

                newFishScript.name = progress.fishBagsExoticFishNames[i];

                if (newFish.GetComponent<Scr_Starfish>() != null)
                {
                    Scr_Starfish starfishScript = newFish.GetComponent<Scr_Starfish>();

                    int legsToDisable = 4 - progress.fishBagsStarfishLegCounts[j];

                    for (int k = 0; k < legsToDisable; k++)
                    {
                        starfishScript.starfishLegs[k].SetActive(false);
                    }
                }

                gameManager.BagAStarfish(newFish);
            }
        }


    }
}
