using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FoodBehavior : MonoBehaviour
{
    public float fallSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed/1000, 0);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
