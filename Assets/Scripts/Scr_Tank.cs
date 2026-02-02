using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Tank : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_TimeHandler timeHandler;

    public GameObject tankBounds;
    public GameObject fishSwimBounds;

    public Dictionary<GameObject, HashSet<Scr_Stress.StressFactor>> speciesStressDict;
    public Dictionary<GameObject, int> fishCountBySpeciesDict;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        speciesStressDict = new Dictionary<GameObject, HashSet<Scr_Stress.StressFactor>>(gameManager.fishPrefabs.Length);
        fishCountBySpeciesDict = new Dictionary<GameObject, int>(gameManager.fishPrefabs.Length);

        InitializeDictionaries();

        timeHandler = FindObjectOfType<Scr_TimeHandler>();


        if (timeHandler != null)
        {
            timeHandler.tickEvent.AddListener(OnTickEvent);
        }

    }


    public void OnTickEvent()
    {
        //Debug.Log($"GOLDFISH IN {gameObject.name}: " + fishCountBySpeciesDict[gameManager.fishPrefabs[3]]);

        CheckSpeciesStress();
    }

    void CheckSpeciesStress()
    {
        foreach (var kvp in fishCountBySpeciesDict)
        {
            if (kvp.Value > 0)
            {
                Debug.Log(kvp.Key.name + " is in " + gameObject.name);
                speciesStressDict[kvp.Key] = gameManager.GetComponent<Scr_Stress>().CheckStress(gameObject, kvp.Key);
            }
        }
    }

    public void InitializeDictionaries()
    {
        foreach (GameObject fishPrefab in gameManager.fishPrefabs)
        {
            speciesStressDict.Add(fishPrefab, new HashSet<Scr_Stress.StressFactor>());
            fishCountBySpeciesDict.Add(fishPrefab, 0);
        }
    }

    public void ResetDictionaries()
    {
        foreach (var kvp in speciesStressDict)
        {
            speciesStressDict[kvp.Key] = new HashSet<Scr_Stress.StressFactor>();
        }

        foreach (var kvp in fishCountBySpeciesDict)
        {
            fishCountBySpeciesDict[kvp.Key] = 0;
        }
    }

    public void PopulateFishCountDict()
    {
        List<GameObject> fishInTank = gameManager.GetAllFishInTank(gameObject.transform);

        foreach (GameObject fish in fishInTank)
        {
            if (fish.GetComponent<Scr_Fish>() != null)
            {
                fishCountBySpeciesDict[fish.GetComponent<Scr_Fish>().thisPrefab]++;
            }
        }
    }

    public void AddFish(GameObject fishPrefab)
    {
        if (fishPrefab.GetComponent<Scr_Fish>() != null)
        {
            fishCountBySpeciesDict[fishPrefab]++;
        }
    }

    public void RemoveFish(GameObject fishPrefab)
    {
        if (fishPrefab.GetComponent<Scr_Fish>() != null)
        {
            fishCountBySpeciesDict[fishPrefab]--;
        }
    }
}
