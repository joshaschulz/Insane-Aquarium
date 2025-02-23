using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FoodBehavior : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    public List<Sprite> SpriteOptions;

    public float groundBarrierPercentage;
    private Vector2 groundBarrier;
    public float fallSpeed;
    public float spinSpeed;
    public float spinAmount;
    private int spinDirection;

    private bool fadeOut = false;
    private float currentTimeforFade = 0f;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Add multiple sprites in this list in the inspector to randomly assign one
        if (SpriteOptions.Count > 0)
        {
            int spriteIndex = Random.Range(0, SpriteOptions.Count);
            spriteRenderer.sprite = SpriteOptions[spriteIndex];
        }

        spinDirection = Random.Range(0, 2);
        if (spinDirection == 1)
            spinAmount = -spinAmount;
        InvokeRepeating("rotateFood", 0f, 1 / spinSpeed);

        groundBarrier = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height * groundBarrierPercentage));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -fallSpeed / 1000, 0, Space.World);

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

    void rotateFood()
    {
        transform.Rotate(0, 0, spinAmount);
    }

    public void Despawn()
    {
        gameManager.foodFishDictionary.Remove(gameObject);
        gameManager.RemoveFoodFromExistingFishFoodDiets(gameObject);
        Destroy(gameObject);
    }
}
