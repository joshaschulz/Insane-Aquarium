using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_StructureBehavior : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public float fallSpeed;

    public float groundBarrierPercentage;
    private Vector2 groundBarrier;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        groundBarrier = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * groundBarrierPercentage));

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!(fallSpeed == 0))
            transform.Translate(0, -fallSpeed * Time.deltaTime, 0, Space.World);
        else
            fallSpeed = 0;

    }
}
