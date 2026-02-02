using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Scr_Fish : MonoBehaviour
{

    public Scr_FishAnimation fishAnimation;

    //[HideInInspector]
    public GameObject thisPrefab;
    public string fishDescription;

    private Scr_GameManager gameManager;
    private Scr_Stress stressScr;

    public int minComfortableSameSpeciesPopulation = 0;
    public int maxComfortableTankPopulation = 10;
    public bool maxIsInfinite = true;
    public bool comfortableWithPredators = false;
    public int comfortablePoopLevel = 75;

    public int gameManagerFishPrefabsIndex;

    private int maxCapacityOriginal;

    public GameObject sideCrown;
    public GameObject frontCrown;

    public bool legendary;
    public bool wild;
    public bool mutated;

    public float hungerCount = 0;
    public bool isHungry = false;

    public float growCount = 0;
    public bool grown = false;

    public bool radiated = false;

    public float freakCount = 0;
    public bool isFreaky;

    public bool isStressed;

    public float poopCount = 0;

    public int minutesUntilGrown;
    public int minutesUntilHungry;
    public int minutesUntilPoop;
    public int minutesUntilDead;
    public int minutesUntilFreaky;

    public GameObject heartIcon;
    public GameObject hungerIcon;

    [Range(0f, 1f)]
    //public float spawnHeight;

    public float baseSpeed;
    private float baseSpeedFactored;
    private float currentSpeed;
    private float hungrySpeed;

    public int baseFishCost; //amount to buy from fishy guy
    public int fishValue; //amount the fish sells for
    public int originalfishValue;
    public Vector3 originalScale;

    //public sizeWhenBagged;

    private Vector2 target;

    public Transform spawnTank; //keeps track of fish's spawned tank
    public Scr_Tank currentTankScript;
    public float minX, maxX, minY, maxY;

    public GameObject bloodEffectPrefab;
    public GameObject bloodOutlineEffectPrefab;
    public GameObject bubblesEffectPrefab;
    public GameObject radiationEffectPrefab;
    public GameObject radiationOutlineEffectPrefab;
    public GameObject poopEffectPrefab;
    public GameObject poopOutlineEffectPrefab;

    public GameObject closestFood;

    public List<GameObject> fishDiet;
    public List<GameObject> foodInScene;

    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    private float tickIntervalInMinutes;

    public int numberCaught;

    public int generation = 1;

    public string name;
    string[] prefixes = {
    "Mr.", "Mrs.", "Miss", "Master", "Sir", "Lady",
    "Captain", "Prince", "Princess", "King", "Queen",
    "Lord", "Duke", "Duchess", "Dr.", "Professor",
    "Count", "Countess", "Monsieur",
    "Madame", "Baby", "Little", "Tiny", "Chief",
    "Admiral", "Bishop","General", "Mayor",
};

    string[] nameOptions = {
    "Bubbles", "Finley", "Splash", "Nemo", "Dory",
    "Goldie", "Gillbert", "Pearl", "Marina", "Coral",
    "Azure", "Rainbow", "Sunny", "Pebbles", "Blue",
    "Flash", "Finn", "Glimmer", "Twinkle", "Skye",
    "Shadow", "Sapphire", "Copper", "Stripe", "Dot",
    "Misty", "Echo", "Flipper", "Jet", "Spark",
    "Wave", "Drift", "Frost", "Mystic", "Goby",
    "Sprinkles", "Tide", "Salty", "Crystal", "Jelly",
    "Blinky", "Pip", "Ziggy", "Ruby", "Luna",
    "Star", "Bubblegum", "Marbles", "Chroma", "Glitter",
    "Sushi", "Taffy", "Cherry", "Minty", "Poppy",
    "Coco", "Mango", "Tiki", "Orbit", "Pixel",
    "Nimbus", "Shade", "Cosmo", "Rogue", "Sprout",
    "Zephyr", "Swoosh", "Ripple", "Lagoon", "Tango",
    "Phoenix", "Orbit", "Blaze", "Rascal", "Nimbus",
    "Sparkle", "Twist", "Dash", "Whisper", "Galaxy",
    "Pebble", "Puddle", "Fizz", "Mochi", "Sundae",
    "Waffle", "Pickle", "Noodle", "Mermy", "Fable",
    "Quill", "Pipette", "Biscuit", "Gizmo", "Banjo",
    "Cricket", "Echo", "Flora", "Zeppelin", "Nova"
};

    private void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;
        stressScr = GetComponent<Scr_Stress>();

        maxCapacityOriginal = maxComfortableTankPopulation;

        ChangeGameSettings();

        originalfishValue = fishValue;

        SetMinAndMax();
    }
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

        Debug.Log(legendary);
        Debug.Log(thisPrefab);

        if (legendary)
        {
            gameManager.legendaryCountBySpecies[gameObject.tag] += 1;
            frontCrown.SetActive(true);
            sideCrown.SetActive(true);

            UpdateLegendaryAura();
        }

        Debug.Log(gameManager.legendaryCountBySpecies[gameObject.tag]);

        UpdateSkills();

        currentTankScript = gameManager.GetTankPos(gameObject.transform).GetComponent<Scr_Tank>();
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks.
        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.RemoveListener(OnTickEvent);
        }

        if (legendary)
        {
            gameManager.legendaryCountBySpecies[gameObject.tag] -= 1;
            UpdateLegendaryAura();
        }
    }


    // Start is called before the first frame update
    public void Start()
    {
        //ChangeGameSettings();
        if (name == "")
            name = GenerateRandomName();

        originalScale = gameObject.transform.localScale;
        hungrySpeed = baseSpeed * 1.5f;


        // Start not targeting anything
        SetTarget(gameObject.transform.position);

        // Start selecting between Idle and Moving after the drop in animation has played
        Invoke("IdleOrMove", fishAnimation.frontAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (radiated)
        {
            //gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
            //gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetComponent<Scr_TimeHandler>().timePaused)
            return;

        currentSpeed = baseSpeedFactored * gameManager.GetFastForwardSettingFactor();

        //don't do anything if fish is still in spawn animation
        if (fishAnimation.IsAnimationPlaying(fishAnimation.frontAnimator, "Fish Spawn"))
        {
            return;
        }


        // 3 possibilities: Fish is hungry. Fish is idle. Fish is moving.
        if (isHungry)
        {
            // Find a food to eat
            closestFood = FindClosestFood();

            if (closestFood != null)
            {
                currentSpeed = hungrySpeed * gameManager.GetFastForwardSettingFactor();
                SetTarget(closestFood.transform.position);
            }
        }
        else if (isFreaky)//go freakmode
        {
            if (!radiated || gameManager.skills.currentResearchSkills[3]) //radiated fish can't breed unless have designer guppies skill
            {
                // Find a fish to freak
                GameObject mate = FindClosestMate();

                if (mate != null)
                {
                    SetTarget(mate.transform.position);
                }
            }

        }
        transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        // If fish has reached its target...
        if (target == new Vector2(transform.position.x, transform.position.y) && fishAnimation.GetState() == Scr_FishAnimation.FishState.Move)
        {
            IdleOrMove();
        }

    }

    public void OnTickEvent()
    {
        //Debug.Log($"{gameObject.name} received a tick event!");

        CheckIfStressed();

        HungerCounter();

        GrowCounter();

        FreakCounter();

        PoopCounter();

        UpdateFishInfoPanel();

        if ((isHungry && FindClosestFood() != null))
        {
            CancelInvoke("IdleOrMove");
        }

        if (!radiated || gameManager.skills.currentResearchSkills[3]) //radiated fish can't breed unless have designer guppies skill
        {
            if ((isFreaky && FindClosestMate() != null))
            {
                CancelInvoke("IdleOrMove");
            }
        }

        // Your fish behavior here, e.g., update hunger status.
    }

    public void CheckIfStressed()
    {
        if (currentTankScript.speciesStressDict[thisPrefab].Count > 0)
            SetStressed();
        else
            SetNotStressed();
    }

    public void UpdateSkills()
    {
        /*
        freakuency; gamesettings
        purebred; gamesettings
        schoolSpirit;
        freeRange; gamesettings*/

        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        Scr_PlayerSkills skills = gameManager.skills;

        switch (gameObject.tag)
        {
            case "Goldfish":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_Goldfish) : settings.minutesUntilFreaky_Goldfish;
                fishValue = settings.fishValue_Goldfish;
                break;

            case "Betta Fish":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_BettaFish) : settings.minutesUntilFreaky_BettaFish;
                fishValue = settings.fishValue_BettaFish;
                break;

            case "Piranha":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_Piranha) : settings.minutesUntilFreaky_Piranha;
                fishValue = settings.fishValue_Piranha;
                break;

            case "Clownfish":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_Clownfish) : settings.minutesUntilFreaky_Clownfish;
                fishValue = settings.fishValue_Clownfish;
                break;

            case "Blue Tang":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_BlueTang) : settings.minutesUntilFreaky_BlueTang;
                fishValue = settings.fishValue_BlueTang;
                break;

            case "Tetra":
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_Tetra) : settings.minutesUntilFreaky_Tetra;
                fishValue = settings.fishValue_Tetra;
                break;

            default:
                Debug.LogWarning("Unknown tag on fish! Game settings set to default stats of goldfish (find me in Scr_Fish.ChangeGameSettings())");
                minutesUntilFreaky = (skills.currentFishkeepingSkills[1]) ? (int)(0.75 * settings.minutesUntilFreaky_Goldfish) : settings.minutesUntilFreaky_Goldfish;
                fishValue = settings.fishValue_Goldfish;
                break;
        }

        if (skills.currentFishkeepingSkills[2] && !wild) //purebred
            fishValue *= 2;

        Debug.Log("normal: " + fishValue);

        if (wild && skills.currentFishingSkills[3]) //free range
            fishValue *= 5;

        Debug.Log("wild: " + fishValue);

        if (mutated)
            fishValue = (int)(1.5 * fishValue);

        if (legendary)
            fishValue *= 5;

        Debug.Log("legendary: " + fishValue);


        if (!legendary && gameManager.legendaryCountBySpecies[gameObject.tag] > 0) //boost price if legendary fish exists
            fishValue = (int)(fishValue * (1f + 0.2f * gameManager.legendaryCountBySpecies[gameObject.tag]));

        Debug.Log("legendary aura: " + fishValue);


        if (skills.currentFishkeepingSkills[3])
        {
            maxComfortableTankPopulation = (int)(maxCapacityOriginal * 1.5);
        }

    }

    public void UpdateLegendaryAura()
    {
        foreach (var kvp in gameManager.foodFishDictionary)
        {
            if (kvp.Key.GetComponent<Scr_Fish>() != null)
            {
                if (kvp.Key.CompareTag(gameObject.tag))
                    kvp.Key.GetComponent<Scr_Fish>().UpdateSkills();
            }
        }
    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;

        switch (gameObject.tag)
        {
            case "Goldfish":
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilFreaky = settings.minutesUntilFreaky_Goldfish;
                minutesUntilPoop = settings.minutesUntilPoop_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                baseFishCost = settings.baseFishCost_Goldfish;
                fishValue = settings.fishValue_Goldfish;
                break;

            case "Betta Fish":
                minutesUntilGrown = settings.minutesUntilGrown_BettaFish;
                minutesUntilHungry = settings.minutesUntilHungry_BettaFish;
                minutesUntilFreaky = settings.minutesUntilFreaky_BettaFish;
                minutesUntilPoop = settings.minutesUntilPoop_BettaFish;
                minutesUntilDead = settings.minutesUntilDead_BettaFish;
                baseSpeed = settings.baseSpeed_BettaFish;
                baseFishCost = settings.baseFishCost_BettaFish;
                fishValue = settings.fishValue_BettaFish;
                break;

            case "Piranha":
                minutesUntilGrown = settings.minutesUntilGrown_Piranha;
                minutesUntilHungry = settings.minutesUntilHungry_Piranha;
                minutesUntilFreaky = settings.minutesUntilFreaky_Piranha;
                minutesUntilPoop = settings.minutesUntilPoop_Piranha;
                minutesUntilDead = settings.minutesUntilDead_Piranha;
                baseSpeed = settings.baseSpeed_Piranha;
                baseFishCost = settings.baseFishCost_Piranha;
                fishValue = settings.fishValue_Piranha;
                break;

            case "Clownfish":
                minutesUntilGrown = settings.minutesUntilGrown_Clownfish;
                minutesUntilHungry = settings.minutesUntilHungry_Clownfish;
                minutesUntilFreaky = settings.minutesUntilFreaky_Clownfish;
                minutesUntilPoop = settings.minutesUntilPoop_Clownfish;
                minutesUntilDead = settings.minutesUntilDead_Clownfish;
                baseSpeed = settings.baseSpeed_Clownfish;
                baseFishCost = settings.baseFishCost_Clownfish;
                fishValue = settings.fishValue_Clownfish;
                break;

            case "Blue Tang":
                minutesUntilGrown = settings.minutesUntilGrown_BlueTang;
                minutesUntilHungry = settings.minutesUntilHungry_BlueTang;
                minutesUntilFreaky = settings.minutesUntilFreaky_BlueTang;
                minutesUntilPoop = settings.minutesUntilPoop_BlueTang;
                minutesUntilDead = settings.minutesUntilDead_BlueTang;
                baseSpeed = settings.baseSpeed_BlueTang;
                baseFishCost = settings.baseFishCost_BlueTang;
                fishValue = settings.fishValue_BlueTang;
                break;

            case "Tetra":
                minutesUntilGrown = settings.minutesUntilGrown_Tetra;
                minutesUntilHungry = settings.minutesUntilHungry_Tetra;
                minutesUntilFreaky = settings.minutesUntilFreaky_Tetra;
                minutesUntilPoop = settings.minutesUntilPoop_Tetra;
                minutesUntilDead = settings.minutesUntilDead_Tetra;
                baseSpeed = settings.baseSpeed_Tetra;
                baseFishCost = settings.baseFishCost_Tetra;
                fishValue = settings.fishValue_Tetra;
                break;

            default:
                Debug.LogWarning("Unknown tag on fish! Game settings set to default stats of goldfish (find me in Scr_Fish.ChangeGameSettings())");
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilFreaky = settings.minutesUntilFreaky_Goldfish;
                minutesUntilPoop = settings.minutesUntilPoop_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                baseFishCost = settings.baseFishCost_Goldfish;
                fishValue = settings.fishValue_Goldfish;
                break;
        }

    }

    public string GenerateRandomName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Length)];
        string nameOption = nameOptions[Random.Range(0, nameOptions.Length)];
        return prefix + " " + nameOption;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target);

    }

   
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collisionObj = collision.gameObject;

        if (!isHungry)
        {
            if (!isFreaky)
                return;
            else
            {
                if (!radiated || gameManager.skills.currentResearchSkills[3]) //radiated fish can't breed, they can if have Designer Guppies skill
                {
                    if (!collisionObj.CompareTag(gameObject.tag))
                        return;
                    else
                    {
                        if (!collisionObj.GetComponent<Scr_Fish>().isFreaky)
                            return;
                        else
                        {
                            Scr_Fish collisionObjScr = collisionObj.GetComponent<Scr_Fish>();

                            if (isFreaky && collisionObjScr.isFreaky)
                            {
                                UpdateFishInfoPanel();
                                freakCount = 0;
                                isFreaky = false;
                                collisionObjScr.isFreaky = false;
                                collisionObjScr.freakCount = 0;

                                //heartIcon.SetActive(false);
                                //collisionObjScr.heartIcon.SetActive(false);

                                gameManager.FreakyFishReset(gameObject);
                                IdleOrMove();
                                collisionObjScr.IdleOrMove();

                                if (gameObject.GetInstanceID() < collisionObj.GetInstanceID()) //only the smaller ordered fish in the scene runs this
                                {
                                    gameManager.SpawnBabyFish(thisPrefab, gameObject);

                                    if (gameManager.skills.currentResearchSkills[4] && radiated)
                                    {
                                        if (Random.Range(0, 4) == 0)
                                        {
                                            gameManager.SpawnBabyFish(thisPrefab, gameObject);

                                            if (Random.Range(0, 4) == 0)
                                            {
                                                gameManager.SpawnBabyFish(thisPrefab, gameObject);

                                            }
                                        }
                                    }

                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (collisionObj.GetComponent<Scr_Fish>() != null)
                if (!fishDiet.Contains(collisionObj.GetComponent<Scr_Fish>().thisPrefab))
                    return;
                else
                {

                }
        }


        // Make sure these are not null
        if (collisionObj == null)
            return;

        if (closestFood == null)
            return;

        if (!gameManager.foodFishDictionary.TryGetValue(collisionObj, out GameObject collisionObjPrefab))
        {
            return;
        }

        if (!gameManager.foodFishDictionary.TryGetValue(closestFood, out GameObject closestFoodPrefab))
        {
            return;
        }

        Debug.Log("CLOSEST FOOD PREFAB: " + closestFoodPrefab);

        Debug.Log("closest food prefab: " + closestFoodPrefab);
        Debug.Log("collision obj prefab: " + collisionObjPrefab);
        Debug.Log("Are they the same? " + (closestFoodPrefab == collisionObjPrefab));

        if (closestFoodPrefab != collisionObjPrefab)
        {
            return;
        }

        Debug.Log("Got to here");


        if (isHungry)
        {
            Debug.Log("Got to here even!");
            SetNotHungry();

            //if the info panel is active and showing this fish, update it
            UpdateFishInfoPanel();

            //SetTarget(transform.position);
            IdleOrMove();

            gameManager.PlaySoundEffect(gameManager.SFX_FishEat, 0.7f, 0.8f, 1.2f);

            if (collisionObj.GetComponent<Scr_FoodBehavior>() != null)
            {
                if (collisionObj.GetComponent<Scr_FoodBehavior>().radiated && !radiated)
                {
                    radiated = true;
                    SetRadiatedHue();
                    //gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
                    //gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
                }
            }

            // If the food is a fish, make it run Die(), so sound/blood effects happen
            if (collisionObj.GetComponent<Scr_Fish>() != null || collisionObj.GetComponent<Scr_Starfish>() != null)
            {
                if (collisionObj.GetComponent<Scr_Fish>() != null)
                {
                    if (collisionObj.GetComponent<Scr_Fish>().radiated && !radiated)
                    {
                        radiated = true;
                        SetRadiatedHue();
                        //gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
                        //gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
                    }

                    collisionObj.GetComponent<Scr_Fish>().Die();

                }
                else if (collisionObj.GetComponent<Scr_Starfish>() != null)
                {
                    collisionObj.GetComponent<Scr_Starfish>().Die();
                    IdleOrMove();
                }



            }
            else
            {
                collisionObj.GetComponent<Scr_FoodBehavior>().Despawn();
            }

            List<AudioClip> bubblesSFX = new List<AudioClip> { gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
            List<float> bubblesVolumes = new List<float> { 3f, 0.2f };
            List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
            List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
            gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);

            gameManager.SpawnParticles(bubblesEffectPrefab, transform.position, transform.rotation, null);
        }

    }

    public void IdleOrMove()
    {
        CancelInvoke("IdleOrMove");

        // Select a random speed
        float speedFactor = Random.Range(0.5f, 1.5f);
        baseSpeedFactored = baseSpeed * speedFactor;

        if (Random.Range(0, 2) == 0)
        {
            // Chose to idle
            SetTarget(transform.position);
            Invoke("IdleOrMove", 2 / gameManager.GetFastForwardSettingFactor());
        }
        else
        {
            SetTarget(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

    }


    public void HungerCounter()
    {
        hungerCount += tickIntervalInMinutes;

        if (hungerCount >= minutesUntilHungry && !isHungry)
        {
            SetHungry();
        }
        else if (hungerCount - minutesUntilHungry >= minutesUntilDead)
        {
            Die();
        }
    }

    public void GrowCounter()
    {
        if (!grown)
        {
            growCount += tickIntervalInMinutes;
            if (growCount >= minutesUntilGrown)
            {
                gameManager.MakeFishBigger(gameObject);
                grown = true;
                gameManager.PlaySoundEffect(gameManager.SFX_FishGrow, 1f);
            }
        }
    }

    public void FreakCounter()
    {
        if (grown)
        {
            if (!isStressed)
            {
                if (!isHungry)
                {
                    freakCount = Mathf.Min(minutesUntilFreaky, freakCount + tickIntervalInMinutes);
                }
                else if (isHungry)
                {
                    freakCount = Mathf.Max(0, freakCount - tickIntervalInMinutes);
                }
                isFreaky = (freakCount == minutesUntilFreaky);
                //heartIcon.SetActive(isFreaky);
            }
        }

    }
    public void SetStressed()
    {
        isStressed = true;
    }
    public void SetNotStressed()
    {
        isStressed = false;
    }

    public void SetHungry()
    {
        isHungry = true;
        ToggleHungryColor();
        //hungerIcon.SetActive(true);

        Scr_AdvancedFishFeeder[] advFeeders = FindObjectsOfType<Scr_AdvancedFishFeeder>();

        foreach (Scr_AdvancedFishFeeder advFeeder in advFeeders)
        {
            advFeeder.SpawnFoodBurst();
        }
    }
    public void SetNotHungry()
    {
        isHungry = false;
        hungerCount = 0;
        ToggleHungryColor();
        //hungerIcon.SetActive(false);
    }

    public void SetRadiatedHue()
    {
        Scr_FishHue fishHue = GetComponent<Scr_FishHue>();

        float hue = fishHue.GetHue();

        int sign = (Random.Range(0, 2) == 0) ? -1 : 1;
        hue += sign * gameManager.radiationHueShift;

        fishHue.SetHue(hue);

        if (fishHue.GetHue() != 1)
        {
            mutated = true;
            UpdateSkills();
        }
    }

    public void ToggleHungryColor()
    {
        Color hungryColor = new Color(0.2f, 1f, 0.2f); // subtle green tint

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = isHungry ? hungryColor : Color.white;
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " died!");

        gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);
        gameManager.SpawnParticles(bloodOutlineEffectPrefab, transform.position, transform.rotation, null);
        gameManager.SpawnParticles(bloodEffectPrefab, transform.position, transform.rotation, null);

        gameManager.foodFishDictionary.Remove(gameObject);
        gameManager.RemoveFoodFromExistingFishDiets(gameObject);

        if (gameManager.infoPanel != null)
            gameManager.infoPanel.HideIfFish(this); //hide the fish info ui panel if it's showing this fish

        currentTankScript.RemoveFish(thisPrefab);

        Destroy(gameObject);

    }
    public void PoopCounter()
    {
        poopCount += tickIntervalInMinutes;

        if (poopCount >= minutesUntilPoop)
        {
            Poop();
        }
    }
    private void Poop()
    {
        poopCount = 0;

        gameManager.UpdatePoopLevel(new Vector2(spawnTank.position.x, spawnTank.position.y), 1);

        int poopSoundSeed = Random.Range(0, 2);
        /*if (poopSoundSeed == 0)
            gameManager.PlaySoundEffect(gameManager.SFX_Fart1, 0.2f);
        else
            gameManager.PlaySoundEffect(gameManager.SFX_Fart2, 0.2f);
        */

        gameManager.SpawnParticles(poopOutlineEffectPrefab, transform.position, transform.rotation, transform);
        gameManager.SpawnParticles(poopEffectPrefab, transform.position, transform.rotation, transform);
    }

    public GameObject FindClosestFood()
    {
        if (foodInScene == null || foodInScene.Count == 0)
            return null;

        GameObject closestStarfishLeg = null;
        float minLegDist = float.MaxValue;

        GameObject closestOtherFood = null;
        float minOtherDist = float.MaxValue;

        foreach (GameObject food in foodInScene)
        {
            if (food == null || !food.activeInHierarchy)
                continue;

            float dist = Vector2.Distance(transform.position, food.transform.position);

            // ⭐ Identify starfish leg using parent root
            Scr_Starfish starfishRoot = food.GetComponentInParent<Scr_Starfish>();

            if (starfishRoot != null) // It's a starfish leg
            {
                if (dist < minLegDist)
                {
                    minLegDist = dist;
                    closestStarfishLeg = food;
                }
            }
            else // normal food
            {
                if (dist < minOtherDist)
                {
                    minOtherDist = dist;
                    closestOtherFood = food;
                }
            }
        }

        // Priority:
        if (closestStarfishLeg != null && IsWithinBoundsOfTank(closestStarfishLeg))
            return closestStarfishLeg;

        if (closestOtherFood != null && IsWithinBoundsOfTank(closestOtherFood))
            return closestOtherFood;

        return null;
    }


    public GameObject FindClosestMate()
    {
        if (gameManager.foodFishDictionary.Count > 0)
        {
            GameObject closestMate = null;
            float minDistance = float.MaxValue;

            foreach (GameObject foodFishKey in gameManager.foodFishDictionary.Keys)
            {
                if (foodFishKey.GetComponent<Scr_Fish>())
                {
                    Scr_Fish foodFishKeyScr = foodFishKey.GetComponent<Scr_Fish>();

                    if (foodFishKey != gameObject && foodFishKeyScr.CompareTag(gameObject.tag) && foodFishKeyScr.isFreaky)
                    {
                        float distance = Vector2.Distance(transform.position, foodFishKey.transform.position);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestMate = foodFishKey;
                        }
                    }
                }

            }

            if (closestMate && IsWithinBoundsOfTank(closestMate))
            {
                return closestMate;
            }
            else
            {
                return null;
            }

        }
        return null;
    }

    public void SetTarget(float _Xcoord, float _Ycoord)
    {
        target = new Vector2(_Xcoord, _Ycoord);

        if (target == new Vector2(transform.position.x, transform.position.y))
        {
            fishAnimation.SetState(Scr_FishAnimation.FishState.Idle);
        }
        else
        {
            fishAnimation.SetState(Scr_FishAnimation.FishState.Move);
        }

        fishAnimation.FaceDirection(target);
    }
    public void SetTarget(Vector2 _position)
    {
        target = _position;

        if (target == new Vector2(transform.position.x, transform.position.y))
        {
            fishAnimation.SetState(Scr_FishAnimation.FishState.Idle);
        }
        else
        {
            fishAnimation.SetState(Scr_FishAnimation.FishState.Move);
        }

        fishAnimation.FaceDirection(target);
    }

    private void SetMinAndMax() //set the min and max of where fish can travel
    {
        currentTankScript = gameManager.GetTankPos(gameObject.transform).GetComponent<Scr_Tank>();
        spawnTank = currentTankScript.gameObject.transform;

        GameObject tankFishSwimBounds = currentTankScript.fishSwimBounds;

        BoxCollider2D box = tankFishSwimBounds.GetComponent<BoxCollider2D>();
        Bounds bounds = box.bounds;

        minX = bounds.min.x;
        maxX = bounds.max.x;
        minY = bounds.min.y;
        maxY = bounds.max.y;

    }
    private void SetMinAndMax(Vector2 _spawnTank) //set the min and max of where fish can travel
    {
        currentTankScript = gameManager.GetTankPos(gameObject.transform).GetComponent<Scr_Tank>();
        spawnTank = currentTankScript.gameObject.transform;

        GameObject tankFishSwimBounds = currentTankScript.fishSwimBounds;

        BoxCollider2D box = tankFishSwimBounds.GetComponent<BoxCollider2D>();
        Bounds bounds = box.bounds;

        minX = bounds.min.x;
        maxX = bounds.max.x;
        minY = bounds.min.y;
        maxY = bounds.max.y;

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

    public void UpdateFishInfoPanel()
    {
        if (gameManager.infoPanel.gameObject.activeSelf && gameManager.infoPanel.currentFish == this)
        {
            gameManager.infoPanel.Show(this);
        }
    }

}
