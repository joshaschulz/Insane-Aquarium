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
    public int price;
    public bool radiated;
    public float fallSpeed;
    public float spinSpeed;
    public float spinAmount;
    private int spinDirection;
    public List<GameObject> particleEffectPrefabs;

    private bool fadeOut = false;
    private float currentTimeforFade = 0f;

    // -------- NEW: projectile phase controls ----------
    [Header("Shot Water Blend Settings")]
    public bool isShot = false;               // are we currently in shot mode?
    public float shotSpeed = 10f;            // initial speed of the shot
    public float shotAngleDegrees = 45f;     // angle above horizontal
    public float shotGravity = -20f;         // gravity during shot phase
    public float waterDrag = 5f;             // how quickly horizontal speed dies
    public float maxShotTime = 0.7f;         // how long it behaves like a cannonball

    private Vector2 shotVelocity;            // current velocity during shot
    private float shotTime = 0f;             // how long we've been in shot mode
    private float normalFallSpeed;           // backup of original fallSpeed
    private bool hasLanded = false;


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

        if (particleEffectPrefabs.Count > 0)
        {
            foreach (GameObject _particleEffectPrefab in particleEffectPrefabs)
            {
                gameManager.SpawnParticles(_particleEffectPrefab, transform.position, transform.rotation, transform);
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // -------- MOVEMENT SECTION --------
        if (!hasLanded)
        {
            if (isShot)
            {
                float dt = Time.fixedDeltaTime;
                shotTime += dt;

                // --- Vertical ---
                if (shotTime < maxShotTime)
                {
                    // pure projectile / cannon phase
                    shotVelocity.y += shotGravity * dt;
                }
                else
                {
                    // gently steer vertical speed toward -normalFallSpeed
                    float verticalBlendStrength = 3f; // tweak if needed
                    float targetVy = -normalFallSpeed;
                    shotVelocity.y = Mathf.Lerp(shotVelocity.y, targetVy, verticalBlendStrength * dt);
                }

                // --- Horizontal ---
                // Smoothly bleed off sideways motion toward 0
                shotVelocity.x = Mathf.Lerp(shotVelocity.x, 0f, waterDrag * dt);

                // Move by current velocity
                transform.position += (Vector3)(shotVelocity * dt);

                // When we’re basically at the normal sink behavior, hand off
                if (shotTime >= maxShotTime &&
                    Mathf.Abs(shotVelocity.y + normalFallSpeed) < 0.05f &&
                    Mathf.Abs(shotVelocity.x) < 0.05f)
                {
                    isShot = false;
                    fallSpeed = normalFallSpeed;
                    shotVelocity = Vector2.zero;
                }
            }
            else
            {
                // original straight-down falling in water
                transform.Translate(0, -fallSpeed * Time.fixedDeltaTime, 0, Space.World);
            }

            // Check if we hit the "ground"
            if (transform.position.y < groundBarrier.y && fadeOut == false)
            {
                hasLanded = true;
                fallSpeed = 0f;
                shotVelocity = Vector2.zero;
                isShot = false;

                fadeOut = true;
            }
        }

        // -------- FADE-OUT SECTION (unchanged) --------
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
        gameManager.RemoveFoodFromExistingFishDiets(gameObject);
        Destroy(gameObject);
    }

    public void Shot(bool flipped)
    {
        // remember whatever the current sinking speed is
        normalFallSpeed = fallSpeed;

        // enter shot mode
        isShot = true;
        hasLanded = false;
        fadeOut = false;
        currentTimeforFade = 0f;

        // reset timers and speeds
        shotTime = 0f;
        fallSpeed = 0f;  // don't use straight fall during cannon phase

        // compute initial velocity: left + up
        float rad = shotAngleDegrees * Mathf.Deg2Rad;


        shotVelocity.x = -Mathf.Cos(rad) * shotSpeed * ((!flipped) ? 1 : -1); // left
        shotVelocity.y = Mathf.Sin(rad) * shotSpeed;  // up
    }
}
