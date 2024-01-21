using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FoodBehavior : MonoBehaviour
{
    public float fallSpeed;
    public float spinSpeed;
    public float spinAmount;
    int spinDirection;

    private void Start()
    {
        spinDirection = Random.Range(0, 2);
        if (spinDirection == 1)
            spinAmount = -spinAmount;
        InvokeRepeating("rotateFood", 0f, 1 / spinSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed / 1000, 0, Space.World);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void rotateFood()
    {
        transform.Rotate(0, 0, spinAmount);
    }

}
