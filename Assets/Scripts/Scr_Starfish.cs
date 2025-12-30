using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Scr_Starfish : MonoBehaviour
{
    public Scr_ImmobileFishAnimation fishAnimation;

    [HideInInspector]
    public GameObject thisPrefab;
    public string fishDescription;

    private Scr_GameManager gameManager;

    [Range(0f, 1f)]
    public float spawnHeight;

    public int baseFishCost; //amount to buy from fishy guy
    public Vector3 originalScale;

    public Vector2 spawnTank; //keeps track of fish's spawned tank
    public float minX, maxX, minY, maxY;

    public GameObject bloodEffectPrefab;
    public GameObject bloodOutlineEffectPrefab;
    public GameObject bubblesEffectPrefab;

    public GameObject frontBody;
    public GameObject backBody;

    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    private float tickIntervalInMinutes;

    public int numberCaught;

    public GameObject[] starfishLegs;

    public string name;
    string[] prefixes = {
    "Blue", "Red", "Gold", "Silver", "Pearl", "Shadow", "Moon", "Star", "Bubble", "Coral",
    "Swift", "Tiny", "Glitter", "Gloom", "Frost", "Storm", "Pink", "Aqua", "Lime", "Emerald"
       };

    string[] suffixes = {
    "fin", "tail", "gill", "whisper", "scale", "flash", "spark", "glow",
    "shimmer", "drifter", "swimmer", "dancer", "dart", "stripe", "snap"
    };

    private void OnEnable()
    {
        // Optionally, get a reference to the TickHandler (assuming there's only one or it’s a singleton)
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();
        Scr_UIElementsHandler = FindObjectOfType<Scr_UIElementsHandler>();


        tickIntervalInMinutes = Scr_TimeHandler.tickInterval / 60;

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks.
        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.RemoveListener(OnTickEvent);
        }
    }


    // Start is called before the first frame update
    public void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        //ChangeGameSettings();

        originalScale = gameObject.transform.localScale;

        // Each fish has a different max range they can travel, based on their size
        SetMinAndMax();


    }

    public string GenerateRandomName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Length)];
        string suffix = suffixes[Random.Range(0, suffixes.Length)];
        return prefix + suffix.Substring(0, 1).ToUpper() + suffix.Substring(1);
    }

    public void OnTickEvent()
    {
        // 1 in 10 chance to regenerate leg
        if (Random.Range(0, 10) == 0)
        {
            // find all inactive legs
            List<GameObject> inactiveLegs = new List<GameObject>();
            foreach (GameObject leg in starfishLegs)
            {
                if (!leg.activeSelf)
                    inactiveLegs.Add(leg);
            }

            // none to enable
            if (inactiveLegs.Count == 0)
                return;

            // choose random inactive leg and activate it
            int index = Random.Range(0, inactiveLegs.Count);
            inactiveLegs[index].SetActive(true);

            gameManager.AddRegeneratedStarfishLegToFishDiets(this, inactiveLegs[index]);
            gameManager.foodFishDictionary.Add(inactiveLegs[index], thisPrefab);
        }

        if (Random.Range(0, 10) == 0) //swap faces
        {
            // swap active state
            bool frontIsActive = frontBody.activeSelf;

            frontBody.SetActive(!frontIsActive);
            backBody.SetActive(frontIsActive);

            if (!frontIsActive)
                gameManager.PlaySoundEffect(gameManager.SFX_StarfishFlop, 1, 1.5f, 2f);
            else
                gameManager.PlaySoundEffect(gameManager.SFX_StarfishFlop, 1, 0.5f, 1f);

            List<AudioClip> bubblesSFX = new List<AudioClip> { gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
            List<float> bubblesVolumes = new List<float> { 3f, 0.2f };
            List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
            List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
            gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);

            gameManager.SpawnParticles(bubblesEffectPrefab, transform.position, transform.rotation, null);
        }



        // Debug.Log("Leg grew back: " + inactiveLegs[index].name);
    }

    // Update is called once per frame
    void Update()
    {
        //don't do anything if fish is still in spawn animation
        if (fishAnimation.IsAnimationPlaying(fishAnimation.frontAnimator, "Fish Spawn"))
        {
            return;
        }

    }

    public void Die()
    {
        if (gameObject.CompareTag("Starfish")) //if this gameobject is a starfish, eat its legs
        {
            List<GameObject> available = new List<GameObject>();

            foreach (GameObject leg in starfishLegs)
            {
                if (leg.activeSelf)
                {
                    available.Add(leg);
                }
            }

            if (available.Count > 0)
            {
                int randomIndex = Random.Range(0, available.Count);
                GameObject randomLeg = available[randomIndex];

                gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);
                gameManager.SpawnParticles(bloodOutlineEffectPrefab, transform.position, transform.rotation, null);
                gameManager.SpawnParticles(bloodEffectPrefab, transform.position, transform.rotation, null);



                gameManager.foodFishDictionary.Remove(randomLeg);
                gameManager.RemoveFoodFromExistingFishDiets(randomLeg);

                available[randomIndex].SetActive(false);

            }

        }
    }


    private void SetMinAndMax() //set the min and max of where fish can travel
    {
        spawnTank = gameManager.GetTankPos(gameObject.transform);

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        minX = spawnTank.x - screenWidthWorld / 2;
        maxX = spawnTank.x + screenWidthWorld / 2;
        minY = spawnTank.y - screenHeightWorld / 2;
        maxY = spawnTank.y + screenHeightWorld / 2;

    }
    private void SetMinAndMax(Vector2 _spawnTank) //set the min and max of where fish can travel
    {
        spawnTank = _spawnTank;

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        minX = spawnTank.x - screenWidthWorld / 2;
        maxX = spawnTank.x + screenWidthWorld / 2;
        minY = spawnTank.y - screenHeightWorld / 2;
        maxY = spawnTank.y + screenHeightWorld / 2;

    }

    private bool IsWithinBoundsOfTank(GameObject obj) //used to determine if a certain object is within the bounds of the tank a fish is contained in
    {
        if (obj.transform.position.x >= minX && obj.transform.position.x <= maxX && obj.transform.position.y >= minY && obj.transform.position.y <= maxY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
