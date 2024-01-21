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

    private void OnColisionEnter2D(Collider2D collision)
    {
        Debug.Log("Hello");
        // Check if the collider belongs to a food object
        if (collision.CompareTag("Fish"))
        {
            gameManager.foodPelletList.Remove(gameObject);
            Destroy(gameObject);
        }

    }
}
