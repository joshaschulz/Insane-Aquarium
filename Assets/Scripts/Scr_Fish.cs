using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Scr_Fish : MonoBehaviour
{
    public Scr_FishAnimation fishAnimation;

    [HideInInspector]
    public GameObject thisPrefab;
    public string fishDescription;

    private Scr_GameManager gameManager;

    private float hungerCount = 0;
    private bool isHungry = false;

    public float growCount = 0;
    public bool grown = false;

    public bool radiated = false;

    public float freakCount = 0;
    public bool isFreaky;

    public float poopCount = 0;

    public int minutesUntilGrown;
    public int minutesUntilHungry;
    public int minutesUntilPoop;
    public int minutesUntilDead;

    public GameObject heartIcon;
    public GameObject hungerIcon;

    [Range(0f, 1f)]
    public float spawnHeight;

    public float baseSpeed;
    private float baseSpeedFactored;
    private float currentSpeed;
    private float hungrySpeed;

    public int baseFishCost; //amount to buy from fishy guy
    public Vector3 originalScale;

    private Vector2 target;

    public Vector2 spawnTank; //keeps track of fish's spawned tank
    public float minX, maxX, minY, maxY;

    public GameObject bloodEffectPrefab;
    public GameObject bloodOutlineEffectPrefab;
    public GameObject bubblesEffectPrefab;
    public GameObject radiationEffectPrefab;
    public GameObject radiationOutlineEffectPrefab;
    public GameObject poopEffectPrefab;

    public List<GameObject> fishDiet;
    public List<GameObject> foodInScene;

    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    private float tickIntervalInMinutes;

    public int numberCaught;

    public int generation = 1;
    public int totalMutations = 0;

    //fish stats (0-5)
    public int priceModifier;
    public int appeal;
    public int hungerCapacity;
    public int freakuency;
    public int poopInterval;

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

        ChangeGameSettings();

        originalScale = gameObject.transform.localScale;
        hungrySpeed = baseSpeed * 1.5f;

        // Each fish has a different max range they can travel, based on their size
        SetMinAndMax();

        // Start not targeting anything
        SetTarget(gameObject.transform.position);

        // Start selecting between Idle and Moving after the drop in animation has played
        Invoke("IdleOrMove", fishAnimation.frontAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (radiated)
        {
            gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
            gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
        }

    }

    public void GenerateRandomStats()
    {
        name = GenerateRandomName();
        // reset everything to 0
        priceModifier = 0;
        appeal = 0;
        hungerCapacity = 0;
        freakuency = 0;
        poopInterval = 0;

        // put the 5 points into a random stat each time
        for (int i = 0; i < 5; i++)
        {
            int roll = Random.Range(0, 5); // 0 to 4

            switch (roll)
            {
                case 0: priceModifier++; break;
                case 1: appeal++; break;
                case 2: hungerCapacity++; break;
                case 3: freakuency++; break;
                case 4: poopInterval++; break;
            }
        }
    }

    public void GenerateStatsFromParents(Scr_Fish parent1, Scr_Fish parent2)
    {
        name = GenerateNameFromParents(parent1, parent2);
        //name = GenerateRandomName();
        priceModifier = Random.Range(0, 2) == 0 ? parent1.priceModifier : parent2.priceModifier;
        appeal = Random.Range(0, 2) == 0 ? parent1.appeal : parent2.appeal;
        hungerCapacity = Random.Range(0, 2) == 0 ? parent1.hungerCapacity : parent2.hungerCapacity;
        freakuency = Random.Range(0, 2) == 0 ? parent1.freakuency : parent2.freakuency;
        poopInterval = Random.Range(0, 2) == 0 ? parent1.poopInterval : parent2.poopInterval;

        // Determine mutation chance
        float mutationChance = 0f;

        if (parent1.radiated) mutationChance += 0.5f;
        if (parent2.radiated) mutationChance += 0.5f;

        // Clamp so 2 radiated parents = 100% chance
        mutationChance = Mathf.Min(mutationChance, 1f);

        // Roll mutation
        if (Random.value < mutationChance)
        {
            ApplyMutation();
            totalMutations = Mathf.Max(parent1.totalMutations, parent2.totalMutations) + 1;
        }

        generation = Mathf.Max(parent1.generation, parent2.generation) + 1;
    }

    public void ApplyMutation()
    {
        int[] stats = { priceModifier, appeal, hungerCapacity, freakuency, poopInterval };
        int roll = Random.Range(0, 5);

        if (stats[roll] < 5)
        {
            stats[roll]++;
        }

        // write back the mutated value
        priceModifier = stats[0];
        appeal = stats[1];
        hungerCapacity = stats[2];
        freakuency = stats[3];
        poopInterval = stats[4];
    }

    public string GenerateRandomName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Length)];
        string suffix = suffixes[Random.Range(0, suffixes.Length)];
        return prefix + suffix.Substring(0, 1).ToUpper() + suffix.Substring(1);
    }

    public string GenerateNameFromParents(Scr_Fish parent1, Scr_Fish parent2)
    {
        // Extract prefix/suffix from parent1
        string n1 = parent1.name;
        int n1Split = -1;
        for (int i = 1; i < n1.Length; i++)
        {
            if (char.IsUpper(n1[i]))
            {
                n1Split = i;
                break;
            }
        }
        string p1Prefix = n1Split > 0 ? n1.Substring(0, n1Split) : n1;
        string p1Suffix = n1Split > 0 ? n1.Substring(n1Split) : "";

        // Extract prefix/suffix from parent2
        string n2 = parent2.name;
        int n2Split = -1;
        for (int i = 1; i < n2.Length; i++)
        {
            if (char.IsUpper(n2[i]))
            {
                n2Split = i;
                break;
            }
        }
        string p2Prefix = n2Split > 0 ? n2.Substring(0, n2Split) : n2;
        string p2Suffix = n2Split > 0 ? n2.Substring(n2Split) : "";

        // Safety check
        if (p1Prefix == "" || p1Suffix == "")
            Debug.LogError("Invalid name format for parent1: " + n1);
        if (p2Prefix == "" || p2Suffix == "")
            Debug.LogError("Invalid name format for parent2: " + n2);

        // Randomly pick from parents
        int choice1 = Random.Range(0, 2); //if 0, picked from parent1. if 1, picked from parent2
        string finalPrefix = (choice1 == 0) ? p1Prefix : p2Prefix;
        string finalSuffix = (choice1 == 0) ? p2Suffix : p1Suffix;

        return finalPrefix + finalSuffix;
    }

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;

        switch (gameObject.tag)
        {
            case "Goldfish":
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilPoop = settings.minutesUntilPoop_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                baseFishCost = settings.baseFishCost_Goldfish;
                break;

            case "Betta Fish":
                minutesUntilGrown = settings.minutesUntilGrown_BettaFish;
                minutesUntilHungry = settings.minutesUntilHungry_BettaFish;
                minutesUntilPoop = settings.minutesUntilPoop_BettaFish;
                minutesUntilDead = settings.minutesUntilDead_BettaFish;
                baseSpeed = settings.baseSpeed_BettaFish;
                baseFishCost = settings.baseFishCost_BettaFish;
                break;

            case "Piranha":
                minutesUntilGrown = settings.minutesUntilGrown_Piranha;
                minutesUntilHungry = settings.minutesUntilHungry_Piranha;
                minutesUntilPoop = settings.minutesUntilPoop_Piranha;
                minutesUntilDead = settings.minutesUntilDead_Piranha;
                baseSpeed = settings.baseSpeed_Piranha;
                baseFishCost = settings.baseFishCost_Piranha;
                break;

            case "Clownfish":
                minutesUntilGrown = settings.minutesUntilGrown_Clownfish;
                minutesUntilHungry = settings.minutesUntilHungry_Clownfish;
                minutesUntilPoop = settings.minutesUntilPoop_Clownfish;
                minutesUntilDead = settings.minutesUntilDead_Clownfish;
                baseSpeed = settings.baseSpeed_Clownfish;
                baseFishCost = settings.baseFishCost_Clownfish;
                break;

            case "Blue Tang":
                minutesUntilGrown = settings.minutesUntilGrown_BlueTang;
                minutesUntilHungry = settings.minutesUntilHungry_BlueTang;
                minutesUntilPoop = settings.minutesUntilPoop_BlueTang;
                minutesUntilDead = settings.minutesUntilDead_BlueTang;
                baseSpeed = settings.baseSpeed_BlueTang;
                baseFishCost = settings.baseFishCost_BlueTang;
                break;

            default:
                Debug.LogWarning("Unknown tag on fish! Game settings set to default stats of goldfish (find me in Scr_Fish.ChangeGameSettings())");
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilPoop = settings.minutesUntilPoop_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                baseFishCost = settings.baseFishCost_Goldfish;
                break;
        }

        Debug.Log("BASE FISH COST:" + baseFishCost);


    }

    public void OnTickEvent()
    {
        Debug.Log($"{gameObject.name} received a tick event!");

        HungerCounter();

        GrowCounter();

        FreakCounter();

        PoopCounter();

        if ((isHungry && FindClosestFood() != null) || (isFreaky && FindClosestMate() != null))
        {
            CancelInvoke("IdleOrMove");
        }

        // Your fish behavior here, e.g., update hunger status.
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target);

    }

    // Update is called once per frame
    void Update()
    {
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
            GameObject food = FindClosestFood();

            if (food != null)
            {
                currentSpeed = hungrySpeed * gameManager.GetFastForwardSettingFactor();
                SetTarget(food.transform.position);
            }
        }
        else if (isFreaky)//go freakmode
        {
            // Find a fish to freak
            GameObject mate = FindClosestMate();

            if (mate != null)
            {
                SetTarget(mate.transform.position);
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        // If fish has reached its target...
        if (target == new Vector2(transform.position.x, transform.position.y) && fishAnimation.GetState() == Scr_FishAnimation.FishState.Move)
        {
            IdleOrMove();
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {

        GameObject collisionObj = collision.gameObject;

        if (collisionObj.CompareTag(gameObject.tag))
        {
            Scr_Fish collisionObjScr = collisionObj.GetComponent<Scr_Fish>();

            if (isFreaky && collisionObjScr.isFreaky)
            {

                freakCount = 0;
                isFreaky = false;
                collisionObjScr.isFreaky = false;
                collisionObjScr.freakCount = 0;

                heartIcon.SetActive(false);
                collisionObjScr.heartIcon.SetActive(false);

                gameManager.FreakyFishReset(gameObject);
                IdleOrMove();
                collisionObjScr.IdleOrMove();

                if (gameObject.GetInstanceID() < collisionObj.GetInstanceID()) //only the smaller ordered fish in the scene runs this
                {
                    GameObject babyFish = gameManager.SpawnBabyFish(thisPrefab, gameObject);
                    Scr_Fish babyFishScr = babyFish.GetComponent<Scr_Fish>();
                    babyFishScr.GenerateStatsFromParents(this, collisionObjScr);
                }
                return;
            }
        }

        if (!foodInScene.Contains(collisionObj))
        {
            return;
        }

        if (isHungry)
        {
            SetNotHungry();
            SetTarget(transform.position);

            gameManager.PlaySoundEffect(gameManager.SFX_FishEat, 0.7f, 0.8f, 1.2f);

            if (collisionObj.GetComponent<Scr_FoodBehavior>() != null)
            {
                if (collisionObj.GetComponent<Scr_FoodBehavior>().radiated && !radiated)
                {
                    radiated = true;
                    gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
                    gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
                }
            }

            // If the food is a fish, make it run Die(), so sound/blood effects happen
            if (collisionObj.GetComponent<Scr_Fish>() != null)
            {
                if (collisionObj.GetComponent<Scr_Fish>().radiated && !radiated)
                {
                    radiated = true;
                    gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
                    gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
                }

                collisionObj.GetComponent<Scr_Fish>().Die();
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
        else if (hungerCount >= minutesUntilDead)
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
            if (!isHungry)
            {
                freakCount = Mathf.Min(100, freakCount + tickIntervalInMinutes);
            }
            else if (isHungry)
            {
                freakCount = Mathf.Max(0, freakCount - tickIntervalInMinutes);
            }
            isFreaky = (freakCount == 100);
            heartIcon.SetActive(isFreaky);

            Debug.Log(gameObject.name + freakCount);
        }

    }

    public void SetHungry()
    {
        isHungry = true;
        hungerIcon.SetActive(true);
    }
    public void SetNotHungry()
    {
        isHungry = false;
        hungerCount = 0;
        hungerIcon.SetActive(false);
    }
    public void Die()
    {
        Debug.Log(gameObject.name + " died!");

        gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);
        gameManager.SpawnParticles(bloodOutlineEffectPrefab, transform.position, transform.rotation, null);
        gameManager.SpawnParticles(bloodEffectPrefab, transform.position, transform.rotation, null);

        gameManager.foodFishDictionary.Remove(gameObject);
        gameManager.RemoveFoodFromExistingFishDiets(gameObject);


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
        Debug.Log("Poop!");
        poopCount = 0;

        int poopSoundSeed = Random.Range(0, 2);
        if (poopSoundSeed == 0)
            gameManager.PlaySoundEffect(gameManager.SFX_Fart1, 0.2f);
        else
            gameManager.PlaySoundEffect(gameManager.SFX_Fart2, 0.2f);

        gameManager.SpawnParticles(poopEffectPrefab, transform.position, transform.rotation, transform);
    }
    public GameObject FindClosestFood() // Returns the closest edible food to the fish or NULL if no edible food exist.
    {
        // If there are food objects in the scene that this fish can eat...
        if (foodInScene.Count > 0)
        {
            GameObject closestEdibleFood = foodInScene[0];
            float minDistance = float.MaxValue;
            for (int j = 0; j < foodInScene.Count; j++)
            {

                float distance = Vector2.Distance(transform.position, foodInScene[j].transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEdibleFood = foodInScene[j];
                }
            }

            //if the food is within the tank that the fish is contained in
            if (IsWithinBoundsOfTank(closestEdibleFood))
            {
                return closestEdibleFood;
            }
        }
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

                    if (foodFishKey != gameObject && foodFishKeyScr.CompareTag(gameObject.tag) && foodFishKeyScr.freakCount == 100)
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
