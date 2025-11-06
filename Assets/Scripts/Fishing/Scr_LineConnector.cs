using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_LineConnector : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public Transform ColliderLeft;
    public Transform ColliderRight;

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
        pointA = transform;

        if (pointA && pointB)
        {
            Vector2 start = pointA.position;
            Vector2 dir = (pointB.position - pointA.position).normalized;
            float dist = Vector2.Distance(pointA.position, pointB.position);

            RaycastHit2D hit = Physics2D.Raycast(start, dir, dist);

            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.name);
                if (hit.collider.name == ("Toilet Collider Left"))
                {
                    line.positionCount = 3;
                    line.SetPosition(0, pointA.position);
                    line.SetPosition(1, ColliderLeft.position);
                    line.SetPosition(2, pointB.position);
                }
                else if (hit.collider.name == ("Toilet Collider Right"))
                {
                    line.positionCount = 3;
                    line.SetPosition(0, pointA.position);
                    line.SetPosition(1, ColliderRight.position);
                    line.SetPosition(2, pointB.position);
                }
                else
                {
                    line.positionCount = 2;
                    line.SetPosition(0, pointA.position);
                    line.SetPosition(1, pointB.position);
                }
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, pointA.position);
                line.SetPosition(1, pointB.position);
            }


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
