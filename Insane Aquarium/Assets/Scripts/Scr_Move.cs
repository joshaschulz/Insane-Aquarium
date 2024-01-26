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
    public float MinX, MaxX, MinY, MaxY;
    public float speed;
    public float transitionLength;
    public float coinDropTimer;
    public Color hungryColor;


    public GameObject boundingBox;
    private Vector2 boundingBoxSize;

    private float startScaleX;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        gameManager.fishList.Add(gameObject);
        fishId = gameManager.fishIdCounter;

        startScaleX = side.transform.localScale.x;
        idle = false;
        invoked = false;
        SetMinAndMax();

        target = gameObject.transform.position;

        InvokeRepeating("HungerCounter", 0, 1);
        InvokeRepeating("DropCoin", coinDropTimer, coinDropTimer);
    }

    void Update()
    {
        Debug.DrawLine(transform.position, target);

        // If fish is hungry and food exists, go to it
        if (FindClosestPellet() != null && IsHungry)
        {
            MoveToPellet();
        }

        // Bool which decides if fish is at its target
        atTarget = gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y;

        // If fish is not at its target and not idle and ready to move, move.
        if (!atTarget && !idle && !invoked)
            Move();
        
        // If fish is at its target and is hungry and food exists, go to it
        else if (atTarget && !idle && IsHungry && FindClosestPellet() != null)
            MoveToPellet();
        // If fish is at its target, set it to idle
        else if (atTarget && !idle)
            SetIdleState();

    }

    public void HungerCounter()
    {
        Debug.Log(gameObject.name + "'s hunger counter is at : " + HungerCount);

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
        ChangeFishColor(transform, hungryColor);
    }
    public void SetNotHungry()
    {
        IsHungry = false;
        HungerCount = 0;
        side.GetComponent<CircleCollider2D>().enabled = false;
        ChangeFishColor(transform, Color.white);
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


    public void ChangeFishColor(Transform _fishParentObject, Color _colorToChange) // Checks ALL children, except for those named "Bounding Box"
    {
        foreach (Transform child in _fishParentObject)
        {
            if (child.gameObject.GetComponent<SpriteRenderer>() != null && child.name != "Bounding Box")
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = _colorToChange;
            }

            if (child.childCount > 0)
            {
                ChangeFishColor(child, _colorToChange);
            }
        }
    }
    

    private void MoveToPellet()
    {
        if (FindClosestPellet() != null)
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
        }

    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));

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

    private void DropCoin()
    {
        Instantiate(gameManager.goldCoin, new Vector2(transform.position.x, transform.position.y - boundingBoxSize.y/2), Quaternion.identity);
        gameManager.PlaySoundEffect(gameManager.SFX_DropCoin, 0.3f, 0.8f, 1.2f);
    }

    private void SetInvokedFalse()
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
            Invoke("SetInvokedFalse", transitionLength);
            side.SetActive(false);
            front.SetActive(true);
        }
        else
            SetInvokedFalse();
    }

    private void Move()
    {
        gameObject.transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void SetMinAndMax()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        boundingBoxSize = new Vector2(boundingBox.transform.localScale.x * gameObject.transform.localScale.x, boundingBox.transform.localScale.y * gameObject.transform.localScale.y);

        MinX = -Bounds.x + (0.5f * boundingBoxSize.x * transform.localScale.x);
        MaxX = Bounds.x - (0.5f * boundingBoxSize.x * transform.localScale.x);
        MinY = -Bounds.y + (0.5f * boundingBoxSize.y * transform.localScale.y);
        MaxY = Bounds.y - (0.5f * boundingBoxSize.y * transform.localScale.y);

    }

}
