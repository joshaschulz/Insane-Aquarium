using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Vector2 target;
    private float MinX, MaxX, MinY, MaxY;

    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 Bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        MinX = -Bounds.x;
        MaxX = Bounds.x;
        MinY = -Bounds.y;
        MaxY = Bounds.y;

        SetNewTarget();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(target);

        Move();

        if (gameObject.transform.position.x == target.x && gameObject.transform.position.y == target.y)
            SetNewTarget();

    }

    private void SetNewTarget()
    {
        target = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));

    }

    private void Move()
    {
        spriteRenderer.flipX = target.x < transform.position.x;

        gameObject.transform.position = Vector2.MoveTowards(transform.position, target, 1 * Time.deltaTime);

    }
}
