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

    private bool idle;
    private bool invoked;
    public bool isHungry;
    public bool dieCorRunning;

    public int fishId;

    public Vector2 target;
    public float MinX, MaxX, MinY, MaxY;
    public float speed;
    public float transitionLength;
    public float hungryTimer;
    public float dieTimer;

    public GameObject boundingBox;
    private Vector2 boundingBoxSize;

    private float startScaleX;

    void Start()
    {

        gameManager = Scr_GameManager.GMinstance;

        startScaleX = gameObject.transform.localScale.x;
        idle = false;
        invoked = false;
        isHungry = true;
        SetMinAndMax();


        //SetNewTarget();
        target = gameObject.transform.position;

        FindClosestPellet();
        StartCoroutine(Die());
    }

    void Update()
    {
        //Debug.Log(target);
        Debug.DrawLine(transform.position, target);

        //if food pellet exists, go to it
        if (closestPellet != null && isHungry)
        {
            MoveToPellet();
        }


        if (!(gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y) && !idle && !invoked)
            Move();
        else if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y && !idle && isHungry && closestPellet != null)
            MoveToPellet();
        else if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y && !idle)
            SetIdleState();



    }

    public void InvokeSetHungry()
    {
        Invoke("SetHungry", hungryTimer);
    }

    public void SetHungry()
    {
        isHungry = true;
        side.GetComponent<CircleCollider2D>().enabled = true;
        FindClosestPellet();

        if (closestPellet != null)
        {
            MoveToPellet();
        }
        else
            StartCoroutine(Die());
    }

    private void MoveToPellet()
    {
        target = closestPellet.transform.position;

        Move();
        SetInvokedFalse(); //seems inefficient
    }

    //like CalculateClosestPellet on game manager. need to do this here when hungry timer runs out
    public void FindClosestPellet()
    {
        if (gameManager.foodPelletList.Count > 0 && gameManager.foodPelletList != null)
        {
            Debug.Log("looking for closest pellet");
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
            closestPellet = closestFoodPellet;
        }
    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));

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

    public IEnumerator Die()
    {
        dieCorRunning = true;
        yield return new WaitForSeconds(dieTimer);
        if (isHungry == true)
        {
            Debug.Log(gameObject.name + " died due to hunger!");
            gameManager.fishList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
