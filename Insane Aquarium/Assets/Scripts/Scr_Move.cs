using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Vector2 target;
    private float MinX, MaxX, MinY, MaxY;
    public float speed;

    public SpriteRenderer spriteRenderer;

    private Rect fishRect;
    private Vector2 fishRectSize;
    //private float scaleX, scaleY;

    void Start()
    {
        SetMinAndMax();


        SetNewTarget();


    }

    void Update()
    {
        //Debug.Log(target);
        Debug.DrawLine(transform.position, target);

        Move();

        if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y)
            SetNewTarget();

    }

    private void SetMinAndMax()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        fishRect = spriteRenderer.sprite.rect;
        fishRectSize = new Vector2(fishRect.width/Screen.width * Bounds.x / 4, fishRect.height / Screen.height * Bounds.y / 4);

        MinX = -Bounds.x + (fishRectSize.x * transform.localScale.x);
        MaxX = Bounds.x - (fishRectSize.x * transform.localScale.x);
        MinY = -Bounds.y + (fishRectSize.y * transform.localScale.y);
        MaxY = Bounds.y - (fishRectSize.y * transform.localScale.y);

    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));

    }

    private void Move()
    {
        spriteRenderer.flipX = target.x < transform.position.x;

        gameObject.transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }


}
