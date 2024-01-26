using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CoinBehavior : MonoBehaviour
{
    private Scr_GameManager gameManager;

    private SpriteRenderer spriteRenderer;
    public Sprite Coin1;
    public Sprite Coin2;
    public Sprite Coin3;
    public Sprite Coin4;

    private int spinCounter = 1;

    public float groundBarrierPercentage;
    private Vector2 groundBarrier;
    public float fallSpeed;
    public float spinSpeed;

    private bool fadeOut = false;
    private float currentTimeforFade = 0f;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        spriteRenderer = GetComponent<SpriteRenderer>();

        spinCounter = Random.Range(1, 5);
        InvokeRepeating("rotateCoin", 0f, 1/spinSpeed);

        groundBarrier = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * groundBarrierPercentage));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed / 1000, 0);

        if (transform.position.y < groundBarrier.y && fadeOut == false)
        {
            fallSpeed = 0;
            fadeOut = true;
        }
        if (fadeOut == true)
        {
            currentTimeforFade += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(currentTimeforFade / gameManager.groundTimeUntilDespawn);

            Color currentColor = spriteRenderer.material.color;
            currentColor.a = alpha;
            spriteRenderer.material.color = currentColor;

            if (currentTimeforFade >= gameManager.groundTimeUntilDespawn)
            {
                Despawn();
            }
        }
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

    public void GetClicked()
    {
        gameManager.SetMoneyAmount(gameManager.GetMoneyAmount() + gameManager.goldCoinWorth);
        Destroy(gameObject);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
