using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_LineConnector : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private LineRenderer line;

    public bool fishCaught = false;


    void Awake()
    {
        line = GetComponent<LineRenderer>();

        pointA = transform;

        line.material.renderQueue = 5000; // Render after everything else
    }

    void Update()
    {
        if (fishCaught && pointA && pointB)
        {
            line.SetPosition(0, pointA.position);
            line.SetPosition(1, pointB.position);
        }
    }
}
