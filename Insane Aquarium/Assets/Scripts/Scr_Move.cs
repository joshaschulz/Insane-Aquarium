using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Move : MonoBehaviour
{
    public GameObject side;
    public GameObject front;

    public bool idle;

    Vector2 target;
    private float MinX, MaxX, MinY, MaxY;
    public float speed;

    //public SpriteRenderer spriteRenderer;

    public GameObject boundingBox;
    private Vector2 boundingBoxSize;
    //private float scaleX, scaleY;

    private float startScaleX;

    void Start()
    {
        startScaleX = gameObject.transform.localScale.x;
        idle = false;
        SetMinAndMax();


        SetNewTarget();


    }

    void Update()
    {
        //Debug.Log(target);
        Debug.DrawLine(transform.position, target);

        //Move();

        if (!(gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y))
            Move();

        if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y && !idle)
            SetNewTarget();

    }

    private void SetMinAndMax()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        /*fishRect = spriteRenderer.sprite.rect;
        fishRectSize = new Vector2(fishRect.width/Screen.width * Bounds.x / 4, fishRect.height / Screen.height * Bounds.y / 4);
        fishRectSize = new Vector2(fishRect.width / Screen.width * Bounds.x / 4, fishRect.height / Screen.height * Bounds.y / 4);*/

        boundingBoxSize = new Vector2(boundingBox.transform.localScale.x * gameObject.transform.localScale.x, boundingBox.transform.localScale.y * gameObject.transform.localScale.y);

        MinX = -Bounds.x + (0.5f * boundingBoxSize.x * transform.localScale.x);
        MaxX = Bounds.x - (0.5f * boundingBoxSize.x * transform.localScale.x);
        MinY = -Bounds.y + (0.5f * boundingBoxSize.y * transform.localScale.y);
        MaxY = Bounds.y - (0.5f * boundingBoxSize.y * transform.localScale.y);

    }

    private void SetNewTarget()
    {
        int randInt = Random.Range(0, 2);
        //Debug.Log(randInt);
        if (randInt == 1)
        {
            target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
            if (!side.activeSelf)
            {
                side.SetActive(true);
                front.SetActive(false);
            }
            idle = false;
        }
        else
        {
            if (side.activeSelf)
            {
                side.SetActive(false);
                front.SetActive(true);
            }
            Invoke("SetNewTarget", 2);
            idle = true;
        }

    }

    private void Move()
    {
        if (target.x < transform.position.x)
            gameObject.transform.localScale = new Vector2(startScaleX * -1, gameObject.transform.localScale.y);
        else
            gameObject.transform.localScale = new Vector2(startScaleX, gameObject.transform.localScale.y);


        gameObject.transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

}
