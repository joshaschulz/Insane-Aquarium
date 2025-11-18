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

    public int minutesUntilGrown;
    public int minutesUntilHungry;

    public int minutesUntilDead;

    public GameObject heartIcon;
    public GameObject hungerIcon;

    [Range(0f, 1f)]
    public float spawnHeight;

    public float baseSpeed;
    private float baseSpeedFactored;
    private float currentSpeed;
    private float hungrySpeed;

    public int fishCost;
    public Vector3 originalScale;

    private Vector2 target;

    public Vector2 spawnTank; //keeps track of fish's spawned tank
    public float minX, maxX, minY, maxY;

    public GameObject bloodEffectPrefab;
    public GameObject bloodOutlineEffectPrefab;
    public GameObject bubblesEffectPrefab;
    public GameObject radiationEffectPrefab;
    public GameObject radiationOutlineEffectPrefab;

    public List<GameObject> fishDiet;
    public List<GameObject> foodInScene;

    private Scr_TimeHandler Scr_TimeHandler;
    private Scr_UIElementsHandler Scr_UIElementsHandler;

    private float tickIntervalInMinutes;

    public int numberCaught;

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

    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;

        switch (gameObject.tag)
        {
            case "Goldfish":
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                fishCost = settings.fishCost_Goldfish;
                break;

            case "Betta Fish":
                minutesUntilGrown = settings.minutesUntilGrown_BettaFish;
                minutesUntilHungry = settings.minutesUntilHungry_BettaFish;
                minutesUntilDead = settings.minutesUntilDead_BettaFish;
                baseSpeed = settings.baseSpeed_BettaFish;
                fishCost = settings.fishCost_BettaFish;
                break;

            case "Piranha":
                minutesUntilGrown = settings.minutesUntilGrown_Piranha;
                minutesUntilHungry = settings.minutesUntilHungry_Piranha;
                minutesUntilDead = settings.minutesUntilDead_Piranha;
                baseSpeed = settings.baseSpeed_Piranha;
                fishCost = settings.fishCost_Piranha;
                break;

            case "Clownfish":
                minutesUntilGrown = settings.minutesUntilGrown_Clownfish;
                minutesUntilHungry = settings.minutesUntilHungry_Clownfish;
                minutesUntilDead = settings.minutesUntilDead_Clownfish;
                baseSpeed = settings.baseSpeed_Clownfish;
                fishCost = settings.fishCost_Clownfish;
                break;

            case "Blue Tang":
                minutesUntilGrown = settings.minutesUntilGrown_BlueTang;
                minutesUntilHungry = settings.minutesUntilHungry_BlueTang;
                minutesUntilDead = settings.minutesUntilDead_BlueTang;
                baseSpeed = settings.baseSpeed_BlueTang;
                fishCost = settings.fishCost_BlueTang;
                break;

            default:
                Debug.Log("Unknown tag on fish! Game settings set to default stats of goldfish (find me in Scr_Fish.ChangeGameSettings())");
                minutesUntilGrown = settings.minutesUntilGrown_Goldfish;
                minutesUntilHungry = settings.minutesUntilHungry_Goldfish;
                minutesUntilDead = settings.minutesUntilDead_Goldfish;
                baseSpeed = settings.baseSpeed_Goldfish;
                fishCost = settings.fishCost_Goldfish;
                break;
        }




    }

    public void OnTickEvent()
    {
        Debug.Log($"{gameObject.name} received a tick event!");

        HungerCounter();

        GrowCounter();

        FreakCounter();

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
