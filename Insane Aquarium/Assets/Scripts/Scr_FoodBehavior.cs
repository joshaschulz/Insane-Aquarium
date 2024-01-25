using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FoodBehavior : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public float groundBarrierPercentage;
    private Vector2 groundBarrier;
    public float fallSpeed;
    public float spinSpeed;
    public float spinAmount;
    int spinDirection;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        spinDirection = Random.Range(0, 2);
        if (spinDirection == 1)
            spinAmount = -spinAmount;
        InvokeRepeating("rotateFood", 0f, 1 / spinSpeed);

        groundBarrier = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * groundBarrierPercentage));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed / 1000, 0, Space.World);

        if (transform.position.y < groundBarrier.y)
        {
            gameManager.foodPelletList.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void rotateFood()
    {
        transform.Rotate(0, 0, spinAmount);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject colliderFish = collision.gameObject;
        // Check if the collider belongs to a food object
        if (colliderFish.tag == "Fish" && colliderFish.transform.parent.gameObject.GetComponent<Scr_Move>().isHungry)
        {
            gameManager.foodPelletList.Remove(gameObject);

            Scr_Move fishScrMove = colliderFish.transform.parent.gameObject.GetComponent<Scr_Move>();

            //parent because collider is on side container and scr_move is on goldfish
            fishScrMove.isHungry = false;
            fishScrMove.side.GetComponent<CircleCollider2D>().enabled = false;
            fishScrMove.InvokeSetHungry();
            fishScrMove.SetIdleState();

            // Josh Code - Turn Fish Back to normal color - Other bit of code in the move script
            fishScrMove.ChangeFishColor(fishScrMove.transform, Color.white);

            if (fishScrMove.dieCorRunning)
            {
                StopCoroutine(fishScrMove.Die());
                fishScrMove.dieCorRunning = false;
            }

            Destroy(gameObject);


        }

    }
}
