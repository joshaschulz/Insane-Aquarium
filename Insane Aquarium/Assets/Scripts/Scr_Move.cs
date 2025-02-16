using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Move : MonoBehaviour
{
    public static Scr_Move Moveinstance;
    private Scr_GameManager gameManager;

    public GameObject side;
    public GameObject front;

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
    //public float coinDropTimer;
    public Color hungryColor;

    //failed movement attempts
    /*public float smoothSpeed;
    public float maxSpeed;
    public Vector2 velocity = Vector2.zero;
    public bool moving = false;*/


    public GameObject boundingBox;
    public Vector2 boundingBoxSize;

    private float startScaleX;

    private Animator animatorSpawn;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        //play the spawn animation
        animatorSpawn = front.GetComponent<Animator>();
        animatorSpawn.Play("Fish Spawn");

        gameManager.fishList.Add(gameObject);
        fishId = gameManager.fishIdCounter;

        startScaleX = side.transform.localScale.x;
        idle = false;
        invoked = false;
        SetMinAndMax();

        target = gameObject.transform.position;

        InvokeRepeating("HungerCounter", 0, 1);
        //InvokeRepeating("DropCoin", coinDropTimer, coinDropTimer);
    }

    void Update()
    {
        if (!animatorSpawn.GetCurrentAnimatorStateInfo(0).IsName("Fish Spawn"))
        {
            Debug.DrawLine(transform.position, target);

            // If fish is hungry and food exists, go to it
            if (IsHungry)
                if (FindClosestPellet() != null)
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
        side.GetComponent<CircleCollider2D>().enabled = true;
        gameManager.ChangeColor(gameObject, hungryColor);
    }

    public void SetNotHungry()
    {
        IsHungry = false;
        HungerCount = 0;
        side.GetComponent<CircleCollider2D>().enabled = false;
        gameManager.ChangeColor(gameObject, Color.white);
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " died due to hunger!");
        gameManager.PlaySoundEffect(gameManager.SFX_FishDeath, 1, 0.5f, 1.5f);

        // Instantiate blood particles, then un-child it before destroying the fish, then the particle system after 5 seconds
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, transform.rotation);
        ParticleSystem bloodParticleSystem = bloodEffect.GetComponent<ParticleSystem>();
        bloodParticleSystem.Play();
        Destroy(bloodEffect, 5);

        gameManager.fishList.Remove(gameObject);
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
        if (gameManager.foodPelletList.Count > 0)
        {
            GameObject closestFoodPellet = gameManager.foodPelletList[0];
            float minDistance = float.MaxValue;
            for (int j = 0; j < gameManager.foodPelletList.Count; j++)
            {
                float distance = Vector2.Distance(transform.position, gameManager.foodPelletList[j].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFoodPellet = gameManager.foodPelletList[j];

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
            gameManager.foodPelletList.Remove(colliderFood);
            Destroy(colliderFood);

            // Instantiate bubbles particles, then un-child it, then destroy the particle system after 5 seconds
            List<AudioClip> bubblesSFX = new List<AudioClip>{ gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
            List<float> bubblesVolumes = new List<float> { 3f, 0.2f };
            List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
            List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
            gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);
            GameObject bubblesEffect = Instantiate(bubblesEffectPrefab, transform.position, transform.rotation);
            ParticleSystem bubblesParticleSystem = bubblesEffect.GetComponent<ParticleSystem>();
            bubblesParticleSystem.Play();
            Destroy(bubblesEffect, 5);
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
            if (side.activeSelf)
            {
                side.SetActive(false);
                front.SetActive(true);
            }
            idle = true;
            Invoke("SetIdleState", 2);
        }
        else
        {
            SetNewTarget();
        }
    }

    /*
    private void DropCoin()
    {
        Instantiate(gameManager.goldCoin, new Vector2(transform.position.x, transform.position.y - boundingBoxSize.y/2), Quaternion.identity);
        gameManager.PlaySoundEffect(gameManager.SFX_DropCoin, 0.3f, 0.8f, 1.2f);
    }
    */

    private void StartMoving()
    {
        invoked = false;
        front.SetActive(false);
        side.SetActive(true);
        idle = false;

        if (target.x < side.transform.position.x && side.transform.localScale.x > 0)
            side.transform.localScale = new Vector2(startScaleX * -1, side.transform.localScale.y);
        else if (target.x > side.transform.position.x && side.transform.localScale.x < 0)
            side.transform.localScale = new Vector2(startScaleX, side.transform.localScale.y);
    }

    public void TransitionAnimation()
    {
        idle = true;

        if ((target.x < side.transform.position.x && side.transform.localScale.x > 0) || (target.x > side.transform.position.x && side.transform.localScale.x < 0))
        {
            //play transition
            invoked = true;
            Invoke("StartMoving", transitionLength);
            side.SetActive(false);
            front.SetActive(true);
        }
        else
            StartMoving();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        
        
        /*if (FindClosestPellet() != null)
        {
            if (target == new Vector2(closestPellet.transform.position.x, closestPellet.transform.position.y))
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, smoothSpeed * Time.deltaTime);*/



        /*
        if (closestPellet != null && IsHungry)

        if (target == new Vector2(closestPellet.transform.position.x, closestPellet.transform.position.y))
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        else
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, smoothSpeed * Time.deltaTime);*/

        //failed movement attempts
        /* float step = smoothSpeed * Time.deltaTime;
         transform.position = Vector2.Lerp(transform.position, target, step);*/

        /*if (closestPellet == null)
            transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, smoothSpeed * Time.smoothDeltaTime);
        else
            transform.position = Vector2.SmoothDamp(transform.position, closestPellet.transform.position, ref velocity,  Time.smoothDeltaTime);*/

        //StartCoroutine(LerpPosition(target, 3));

        /*float dist = Vector2.Distance(side.GetComponent<CircleCollider2D>().transform.position, target);
        dist = Mathf.Clamp(Mathf.Abs(dist), 0f, 1.5f);
        Debug.Log(Time.deltaTime);

        transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, smoothSpeed * Time.deltaTime * dist);*/

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
