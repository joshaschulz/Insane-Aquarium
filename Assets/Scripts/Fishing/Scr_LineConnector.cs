using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_LineConnector : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public LineRenderer line;
    public Scr_Fishing fishingRod;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        pointA = transform;

        line.material.renderQueue = 5000; // Render after everything else
    }

    void Update()
    {
        line.enabled = fishingRod.toiletFish;

        if (pointA && pointB)
        {
            line.SetPosition(0, pointA.position);
            line.SetPosition(1, pointB.position);
        }

        /*if (fishingRod.StopFishing)
        {
            line.enabled = false;
        }
        else if (pointA && pointB)
        {
            line.SetPosition(0, pointA.position);
            line.SetPosition(1, pointB.position);
        }*/
    }
}
