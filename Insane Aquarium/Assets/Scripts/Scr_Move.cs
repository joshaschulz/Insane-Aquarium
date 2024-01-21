using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Move : MonoBehaviour
{
    public GameObject side;
    public GameObject front;

    private bool idle;
    private bool invoked;

    Vector2 target;
    private float MinX, MaxX, MinY, MaxY;
    public float speed;
    public float transitionLength;

    public GameObject boundingBox;
    private Vector2 boundingBoxSize;

    private float startScaleX;

    void Start()
    {
        startScaleX = gameObject.transform.localScale.x;
        idle = false;
        invoked = false;
        SetMinAndMax();


        //SetNewTarget();
        target = gameObject.transform.position;

    }

    void Update()
    {
        //Debug.Log(target);
        Debug.DrawLine(transform.position, target);

        //Move();

        if (!(gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y) && !idle && !invoked)
            Move();
        else if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y && !idle)
            SetIdleState();

    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
        idle = true;

        if ((target.x < transform.position.x && gameObject.transform.localScale.x > 0) || (target.x > transform.position.x && gameObject.transform.localScale.x < 0))
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

    private void SetIdleState()
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

    private void SetInvokedFalse()
    {
        invoked = false;
        front.SetActive(false);
        side.SetActive(true);
        idle = false;

        if (target.x < side.transform.position.x && side.transform.localScale.x > 0)
            side.transform.localScale = new Vector2(startScaleX * -1, gameObject.transform.localScale.y);
        else if(target.x > side.transform.position.x && side.transform.localScale.x < 0)
            side.transform.localScale = new Vector2(startScaleX, gameObject.transform.localScale.y);
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
