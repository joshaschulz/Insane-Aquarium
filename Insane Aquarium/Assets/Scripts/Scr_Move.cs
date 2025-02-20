using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Move : MonoBehaviour
{
    public static Scr_Move Moveinstance;
    private Scr_GameManager gameManager;

    private GameObject sideContainer;
    private GameObject frontContainer;

    private Animator sideAnimator;
    private Animator frontAnimator;

    public GameObject closestPellet;
    public GameObject bloodEffectPrefab;
    public GameObject bubblesEffectPrefab;

    private bool idle;
    private bool invoked;
    private bool atTarget;

    private int HungerCount = 0;
    public bool IsHungry;
    public float SecondsUntilHungry;
    public float SecondsUntilDead;

    public int fishId;
    public int fishCost;

    public Vector2 target;
    public float minX, maxX, minY, maxY;
    public float speed;
    public float transitionLength;
    public Color hungryColor;



    public GameObject boundingBox;
    public Vector2 boundingBoxSize;

    private float startScaleX;


    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        

        // These are the gameobjects that hold the front and side images of the fish and their animators. The side one also has the mouth collision circle
        sideContainer = transform.GetChild(0).gameObject;
        frontContainer = transform.GetChild(1).gameObject;
        if (!sideContainer.name.Contains("Side Container"))
            Debug.Log(gameObject.name + "'s first child's name does not contain 'Side Container'.");
        if (!frontContainer.name.Contains("Front Container"))
            Debug.Log(gameObject.name + "'s second child's name does not contain 'Front Container'.");


        //play the spawn animation
        frontAnimator = frontContainer.GetComponent<Animator>();
        sideAnimator = sideContainer.GetComponent<Animator>();
        frontAnimator.Play("Fish Spawn");

        fishId = gameManager.fishIdCounter;

        startScaleX = sideContainer.transform.localScale.x;
        idle = false;
        invoked = false;
        SetMinAndMax();

        target = gameObject.transform.position;

        InvokeRepeating("HungerCounter", 0, 1);
    }

    void Update()
    {
        // If the fish has finished dropping into the tank
        if (!frontAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fish Spawn"))
        {
            Debug.DrawLine(transform.position, target);

            // If fish is hungry and food exists, go to it
            if (IsHungry && FindClosestPellet() != null)
                SetPelletTarget();

            // Bool which decides if fish is at its target
            atTarget = gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y;

            // If fish is not at its target and not idle and ready to move, move.
            if (!atTarget && !idle && !invoked)
                Move();

            // If fish is at its target and is hungry and food exists, go to it
            else if (atTarget && !idle && IsHungry && FindClosestPellet() != null)
                SetPelletTarget();
            // If fish is at its target, set it to idle
            else if (atTarget && !idle)
                SetIdleState();
        }
    }

    public void HungerCounter()
    {
        //Debug.Log(gameObject.name + "'s hunger counter is at : " + HungerCount);

        if (HungerCount == SecondsUntilHungry)
        {
            SetHungry();
        }
        if (HungerCount == SecondsUntilDead)
        {
            Die();
        }
        HungerCount++;
    }
    public void SetHungry()
    {
        IsHungry = true;
        sideContainer.GetComponent<CircleCollider2D>().enabled = true;
        gameManager.ChangeColor(gameObject, hungryColor);
    }
    public void SetNotHungry()
    {
        IsHungry = false;
        HungerCount = 0;
        sideContainer.GetComponent<CircleCollider2D>().enabled = false;
        gameManager.ChangeColor(gameObject, Color.white);
    }
    public void Die()
    {
        Debug.Log(gameObject.name + " died due to hunger!");
        gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);

        gameManager.SpawnParticles(bloodEffectPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
    
    private void SetPelletTarget()
    {
        //if (FindClosestPellet() != null)
        target = FindClosestPellet().transform.position;

        TransitionAnimation();
    }

    public GameObject FindClosestPellet() // Returns the closest pellet to the fish or NULL if no pellets exist.
    {
        // If there are food pellets...
        if (gameManager.foodList.Count > 0)
        {
            GameObject closestFoodPellet = gameManager.foodList[0];
            float minDistance = float.MaxValue;
            for (int j = 0; j < gameManager.foodList.Count; j++)
            {
                float distance = Vector2.Distance(transform.position, gameManager.foodList[j].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFoodPellet = gameManager.foodList[j];

                    closestPellet = closestFoodPellet;
                }
            }
            return closestFoodPellet;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colliderFood = collision.gameObject;

        // Check if the collider belongs to a food object
        if (colliderFood.CompareTag("Food") && IsHungry)
        {
            SetNotHungry();
            SetIdleState();

            gameManager.PlaySoundEffect(gameManager.SFX_FishEat, 0.7f, 0.8f, 1.2f);
            gameManager.foodList.Remove(colliderFood);
            Destroy(colliderFood);

            List<AudioClip> bubblesSFX = new List<AudioClip>{ gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
            List<float> bubblesVolumes = new List<float> { 3f, 0.2f };
            List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
            List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
            gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);

            gameManager.SpawnParticles(bubblesEffectPrefab, transform.position, transform.rotation);
        }

    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

        TransitionAnimation();
    }

    public void SetIdleState()
    {
        int randInt = Random.Range(0, 2);

        if (randInt == 0)
        {
            if (sideContainer.activeSelf)
            {
                sideContainer.SetActive(false);
                frontContainer.SetActive(true);
            }
            idle = true;
            Invoke("SetIdleState", 2);
        }
        else
        {
            SetNewTarget();
        }
    }

    private void StartMoving()
    {
        invoked = false;
        frontContainer.SetActive(false);
        sideContainer.SetActive(true);
        idle = false;

        if (target.x < sideContainer.transform.position.x && sideContainer.transform.localScale.x > 0)
            sideContainer.transform.localScale = new Vector2(startScaleX * -1, sideContainer.transform.localScale.y);
        else if (target.x > sideContainer.transform.position.x && sideContainer.transform.localScale.x < 0)
            sideContainer.transform.localScale = new Vector2(startScaleX, sideContainer.transform.localScale.y);
    }

    public void TransitionAnimation()
    {
        idle = true;

        if ((target.x < sideContainer.transform.position.x && sideContainer.transform.localScale.x > 0) || (target.x > sideContainer.transform.position.x && sideContainer.transform.localScale.x < 0))
        {
            //play transition
            invoked = true;
            Invoke("StartMoving", transitionLength);
            sideContainer.SetActive(false);
            frontContainer.SetActive(true);
        }
        else
            StartMoving();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

    }

    private void SetMinAndMax()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        boundingBoxSize = new Vector2(boundingBox.transform.localScale.x * gameObject.transform.localScale.x, boundingBox.transform.localScale.y * gameObject.transform.localScale.y);

        minX = -Bounds.x + (0.5f * boundingBoxSize.x * transform.localScale.x);
        maxX = Bounds.x - (0.5f * boundingBoxSize.x * transform.localScale.x);
        minY = -Bounds.y + (0.5f * boundingBoxSize.y * transform.localScale.y);
        maxY = Bounds.y - (0.5f * boundingBoxSize.y * transform.localScale.y);

    }

}
