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


    private Scr_GameManager gameManager;
    private GameObject sideContainer;
    private GameObject frontContainer;
    private Animator frontAnimator;



    private int HungerCount = 0;
    private bool IsHungry = false;
    public float SecondsUntilHungry;
    public float SecondsUntilDead;
    public Color hungryColor;

    public float baseSpeed;
    private float currentSpeed;
    public int fishCost;

    private Vector2 target;

    private float minX, maxX, minY, maxY;
    public GameObject boundingBox;
    private Vector2 boundingBoxSize;

    public GameObject bloodEffectPrefab;
    public GameObject bubblesEffectPrefab;

    public List<GameObject> fishDiet;
    public List<GameObject> foodInScene;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        // Each fish has a different max range they can travel, based on their size
        SetMinAndMax();

        // These are the gameobjects that hold the front and side images of the fish and their animators. The side one also has the mouth collision circle
        sideContainer = transform.GetChild(0).gameObject;
        frontContainer = transform.GetChild(1).gameObject;
        if (!sideContainer.name.Contains("Side Container"))
            Debug.Log(gameObject.name + "'s first child's name does not contain 'Side Container'.");
        if (!frontContainer.name.Contains("Front Container"))
            Debug.Log(gameObject.name + "'s second child's name does not contain 'Front Container'.");

        // Play the spawn animation
        frontAnimator = frontContainer.GetComponent<Animator>();
        frontAnimator.Play("Fish Spawn");

        // Start not targeting anything
        target = gameObject.transform.position;

        // Start the hungry timer
        InvokeRepeating("HungerCounter", 0, 1);

        // Start selecting between Idle and Moving after the drop in animation has played
        Invoke("IdleOrMove", frontAnimator.GetCurrentAnimatorStateInfo(0).length);

    }

    // Update is called once per frame
    void Update()
    {
        // 3 possibilities: Fish is hungry. Fish is idle. Fish is moving.
        if (IsHungry)
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
                    frontContainer.SetActive(true);
                    sideContainer.SetActive(false);
                    return;
                }
            }

            if (!sideContainer.activeSelf)
            {
                frontContainer.SetActive(false);
                sideContainer.SetActive(true);
            }

            // Move towards it
            transform.position = Vector2.MoveTowards(transform.position, food.transform.position, baseSpeed * Time.deltaTime);

            // If pellet is left of the fish, then set the fish left, otherwise set the fish right
            int leftOrRight = (food.transform.position.x < transform.position.x) ? -1 : 1;
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
            frontContainer.SetActive(true);
            sideContainer.SetActive(false);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject colliderFood = collision.gameObject;

        if (!foodInScene.Contains(colliderFood))
        {
            return;
        }

        if (IsHungry)
        {
            SetNotHungry();
            frontContainer.SetActive(true);
            sideContainer.SetActive(false);

            // When fish eats, it idles until its next call of IdleOrMove
            target = new Vector2(transform.position.x, transform.position.y);

            gameManager.PlaySoundEffect(gameManager.SFX_FishEat, 0.7f, 0.8f, 1.2f);

            // If the food is a fish, make it run Die(), so sound/blood effects happen
            if (colliderFood.GetComponent<Scr_Fish>() != null)
            {
                colliderFood.GetComponent<Scr_Fish>().Die();
            }
            else
            {
                colliderFood.GetComponent<Scr_FoodBehavior>().Despawn();
            }

            List<AudioClip> bubblesSFX = new List<AudioClip> { gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
            List<float> bubblesVolumes = new List<float> { 3f, 0.2f };
            List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
            List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
            gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);

            gameManager.SpawnParticles(bubblesEffectPrefab, transform.position, transform.rotation);
        }

    }

    private void IdleOrMove()
    {
        if (Random.Range(0, 2) == 0)
        {
            // Chose to idle
            frontContainer.SetActive(true);
            sideContainer.SetActive(false);

            // Idle at the current position
            target = transform.position;
        }
        else
        {
            frontContainer.SetActive(false);
            sideContainer.SetActive(true);

            // Chose to move to a new target
            target = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

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

        if (HungerCount == SecondsUntilHungry)
        {
            SetHungry();
        }
        else if (HungerCount == SecondsUntilDead)
        {
            Die();
        }
        HungerCount++;
    }

    public void SetHungry()
    {
        IsHungry = true;
        //sideContainer.GetComponent<BoxCollider2D>().enabled = true;
        gameManager.ChangeColor(gameObject, hungryColor);
    }
    public void SetNotHungry()
    {
        IsHungry = false;
        HungerCount = 0;
        //sideContainer.GetComponent<BoxCollider2D>().enabled = false;
        gameManager.ChangeColor(gameObject, Color.white);
    }
    public void Die()
    {
        Debug.Log(gameObject.name + " died!");

        gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);
        gameManager.SpawnParticles(bloodEffectPrefab, transform.position, transform.rotation);

        gameManager.RemoveThisFishOrFoodFromOtherFishFoodLists(gameObject);
        gameManager.fishDictionary.Remove(gameObject);

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
    
    private void SetMinAndMax()
    {
        //get the size of the bounding box on the fish
        //use that bounding box size and where the camera is (which tank it's in) to determine the max range the fish can travel

        boundingBoxSize = new Vector2(boundingBox.transform.localScale.x * gameObject.transform.localScale.x, boundingBox.transform.localScale.y * gameObject.transform.localScale.y);

        Vector2 spawnPosition = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);

        float screenWidthWorld = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeightWorld = Camera.main.orthographicSize * 2;

        minX = spawnPosition.x - screenWidthWorld / 2 + boundingBoxSize.x / 2;
        maxX = spawnPosition.x + screenWidthWorld / 2 - boundingBoxSize.x / 2;
        minY = spawnPosition.y - screenHeightWorld / 2 + boundingBoxSize.y / 2;
        maxY = spawnPosition.y + screenHeightWorld / 2 - boundingBoxSize.y / 2;

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
