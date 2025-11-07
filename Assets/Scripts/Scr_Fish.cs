using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Scr_Fish : MonoBehaviour
{

    // Fish drops in

    // Fish starts a hungry timer
    // When timer gets below hungry threshhold, fish turns green and searches for food
    // When timer gets below death threshhold, fish dies

    // When the fish finds a food, it goes towards it
    // When the fish collision hits food, it deletes the food and resets the hungry timer

    // When fish not hungry, fish either moves to a random location or waits for a moment

    // If the fish wants to move in the opposite direction, it must first look forward

    // After the fish eats, it looks forward

    [HideInInspector]
    public GameObject thisPrefab;

    private Scr_GameManager gameManager;
    private GameObject sideContainer;
    private GameObject frontContainer;
    private Animator frontAnimator;

    private float hungerCount = 0;
    private bool isHungry = false;

    public float growCount = 0;
    public bool grown = false;

    public bool radiated = false;

    public float freakCount = 0;
    public bool canFreak;
    private GameObject fishToFreakOn;
    //public float SecondsUntilHungry;
    //public float SecondsUntilDead;

    //public int tickEventsUntilHungry;
    //public int tickEventsUntilDead;

    public int minutesUntilGrown;

    public int minutesUntilHungry;
    public int minutesUntilDead;

    public Color hungryColor;
    private GameObject heartIcon;
    private GameObject hungerIcon;

    [Range(0f, 1f)]
    public float spawnHeight;

    public float baseSpeed;
    private float currentSpeed;
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

    public void OnTickEvent()
    {
        Debug.Log($"{gameObject.name} received a tick event!");

        HungerCounter();

        GrowCounter();

        FreakCounter();

        //Scr_UIElementsHandler.UpdateTankWater();


        // Your fish behavior here, e.g., update hunger status.
    }

    public void Awake()
    {
        // These are the gameobjects that hold the front and side images of the fish and their animators. The side one also has the mouth collision circle
        sideContainer = transform.GetChild(0).gameObject;
        frontContainer = transform.GetChild(1).gameObject;
        heartIcon = transform.GetChild(2).gameObject;
        hungerIcon = transform.GetChild(3).gameObject;
        if (!sideContainer.name.Contains("Side Container"))
            Debug.Log(gameObject.name + "'s first child's name does not contain 'Side Container'.");
        if (!frontContainer.name.Contains("Front Container"))
            Debug.Log(gameObject.name + "'s second child's name does not contain 'Front Container'.");
        if (!heartIcon.name.Contains("Fish Heart"))
            Debug.Log(gameObject.name + "'s third child's name does not contain 'Fish Heart'.");
        if (!hungerIcon.name.Contains("Fish Hunger"))
            Debug.Log(gameObject.name + "'s fourth child's name does not contain 'Fish Hunger'.");

    }

    // Start is called before the first frame update
    public void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        originalScale = gameObject.transform.localScale;

        // Each fish has a different max range they can travel, based on their size
        SetMinAndMax();

        // Play the spawn animation
        frontAnimator = frontContainer.GetComponent<Animator>();
        frontAnimator.Play("Fish Spawn");

        // Start not targeting anything
        SetTarget(gameObject.transform.position);

        // Start the hungry timer
        //InvokeRepeating("HungerCounter", 0, 1);

        // Start selecting between Idle and Moving after the drop in animation has played
        Invoke("IdleOrMove", frontAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (radiated)
        {
            gameManager.SpawnParticles(radiationOutlineEffectPrefab, transform.position, transform.rotation, transform);
            gameManager.SpawnParticles(radiationEffectPrefab, transform.position, transform.rotation, transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //don't do anything if fish is still in spawn animation
        if (IsAnimationPlaying(frontAnimator, "Fish Spawn"))
        {
            return;
        }


        // 3 possibilities: Fish is hungry. Fish is idle. Fish is moving.
        if (isHungry)
        {
            // Find a food to eat
            GameObject food = FindClosestFood();

            // If there is no food, keep going towards same target
            if (food == null)
            {
                if (target != new Vector2(transform.position.x, transform.position.y))
                {
                    // If target is left of the fish, then set the fish left, otherwise set the fish right
                    int tempFlip = (target.x < transform.position.x) ? -1 : 1;
                    sideContainer.transform.localScale = new Vector2(tempFlip, sideContainer.transform.localScale.y);

                    transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
                    return;
                }
                else
                {
                    FaceForward();
                    return;
                }
            }

            if (!sideContainer.activeSelf)
            {
                FaceSideways();
            }

            // Move towards it
            transform.position = Vector2.MoveTowards(transform.position, food.transform.position, baseSpeed * Time.deltaTime);

            // If pellet is left of the fish, then set the fish left, otherwise set the fish right
            int leftOrRight = (food.transform.position.x < transform.position.x) ? -1 : 1;
            sideContainer.transform.localScale = new Vector2(leftOrRight, sideContainer.transform.localScale.y);
        }
        else if (canFreak)//go freakmode
        {

            if (!sideContainer.activeSelf)
            {
                FaceSideways();
            }

            transform.position = Vector2.MoveTowards(transform.position, fishToFreakOn.transform.position, currentSpeed * Time.deltaTime);

            int leftOrRight = (fishToFreakOn.transform.position.x < transform.position.x) ? -1 : 1;
            sideContainer.transform.localScale = new Vector2(leftOrRight, sideContainer.transform.localScale.y);

        }
        // If the fish has a target that is not itself
        else if (target != new Vector2(transform.position.x, transform.position.y))
        {
            // Move towards target
            transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
        }
        // The fish has reached its destination
        else
        {
            // Set to idle until next call of IdleOrMove
            FaceForward();
        }

    }

    IEnumerator DelayedFreakCheck()
    {
        yield return new WaitForEndOfFrame(); // wait for all fish to tick

        FindClosestMate();

        canFreak = (fishToFreakOn != null);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        GameObject collisionObj = collision.gameObject;

        if (collisionObj.CompareTag(gameObject.tag))
        {
            Scr_Fish collisionObjScr = collisionObj.GetComponent<Scr_Fish>();

            if (canFreak && collisionObjScr.canFreak)
            {
                freakCount = 0;
                canFreak = false;
                collisionObjScr.canFreak = false;
                collisionObjScr.freakCount = 0;

                heartIcon.SetActive(false);
                collisionObjScr.heartIcon.SetActive(false);


                FaceForward();
                collisionObjScr.FaceForward();

                SetTarget(transform.position);
                collisionObjScr.SetTarget(collisionObj.transform.position);

                if (gameObject.GetInstanceID() < collisionObj.GetInstanceID()) //only the smaller ordered fish in the scene runs this
                {
                    GameObject babyFish = gameManager.SpawnBabyFish(thisPrefab, gameObject.transform);
                    
                    float hueForBabyFish = GetComponent<Scr_FishHue>().GetHue();
                    if (radiated)
                    {
                        int sign = (Random.Range(0,2) == 0) ? -1 : 1;
                        hueForBabyFish += sign * gameManager.radiationHueShift;
                    }
                    babyFish.GetComponent<Scr_FishHue>().SetHue(hueForBabyFish);
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
            FaceForward();

            // When fish eats, it idles until its next call of IdleOrMove
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

    private void IdleOrMove()
    {
        if (Random.Range(0, 2) == 0)
        {
            // Chose to idle
            FaceForward();

            // Idle at the current position
            SetTarget(transform.position);
        }
        else
        {
            FaceSideways();

            // Chose to move to a new target
            SetTarget(Random.Range(minX, maxX), Random.Range(minY, maxY));

            // If target is left of the fish, then set the fish left, otherwise set the fish right
            int leftOrRight = (target.x < transform.position.x) ? -1 : 1;
            sideContainer.transform.localScale = new Vector2(leftOrRight, sideContainer.transform.localScale.y);

            // Select a random speed
            float speedFactor = Random.Range(0.5f, 1f);
            currentSpeed = baseSpeed * speedFactor;

        }

        Invoke("IdleOrMove", 4);
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
            }
        }
    }

    public void FreakCounter()
    {
        if (grown)
        {
            if (freakCount != 100)
            {
                if (!isHungry)
                {
                    freakCount = Mathf.Min(100, freakCount + tickIntervalInMinutes);
                }
                else if (isHungry)
                {
                    freakCount = Mathf.Max(0, freakCount - tickIntervalInMinutes);
                }

            }

            if (freakCount == 100)
            {
                StartCoroutine(DelayedFreakCheck());
            }
            else
            {
                canFreak = false;
            }

            Debug.Log(gameObject.name + freakCount);
        }

    }

    public void SetHungry()
    {
        isHungry = true;
        //sideContainer.GetComponent<BoxCollider2D>().enabled = true;
        //gameManager.ChangeColor(gameObject, hungryColor);
        hungerIcon.SetActive(true);
    }
    public void SetNotHungry()
    {
        isHungry = false;
        hungerCount = 0;
        //sideContainer.GetComponent<BoxCollider2D>().enabled = false;
        //gameManager.ChangeColor(gameObject, Color.white);
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

    public void FindClosestMate()
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
                heartIcon.SetActive(true);
                fishToFreakOn = closestMate;
            }
            else
            {
                heartIcon.SetActive(false);
                fishToFreakOn = null;
            }

        }
    }

    public void SetTarget(float _Xcoord, float _Ycoord)
    {
        target = new Vector2(_Xcoord, _Ycoord);
    }
    public void SetTarget(Vector2 _position)
    {
        target = _position;
    }
    public void FaceSideways()
    {
        frontContainer.SetActive(false);
        sideContainer.SetActive(true);
    }
    public void FaceForward()
    {
        frontContainer.SetActive(true);
        sideContainer.SetActive(false);
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

    bool IsAnimationPlaying(Animator anim, string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime < 1f;
    }
}
