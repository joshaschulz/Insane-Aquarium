using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CoinBehavior : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public Sprite Coin1;
    public Sprite Coin2;
    public Sprite Coin3;
    public Sprite Coin4;

    private int spinCounter = 1;

    public float fallSpeed;
    public float spinSpeed;

    private void Start()
    {
        spinCounter = Random.Range(1, 5);
        InvokeRepeating("rotateCoin", 0f, 1/spinSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed / 1000, 0);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void rotateCoin()
    {
        if (spinCounter == 1)
        {
            spriteRenderer.sprite = Coin1;
            spinCounter++;
        }
        else if (spinCounter == 2)
        {
            spriteRenderer.sprite = Coin2;
            spinCounter++;
        }
        else if (spinCounter == 3)
        {
            spriteRenderer.sprite = Coin3;
            spinCounter++;
        }
        else if (spinCounter == 4)
        {
            spriteRenderer.sprite = Coin4;
            spinCounter = 1;
        }
    }
}
