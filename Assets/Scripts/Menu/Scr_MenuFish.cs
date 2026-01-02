using UnityEngine;

public class Scr_MenuFish : MonoBehaviour
{
    [Header("Base movement")]
    public float baseSpeed = 2.0f;

    [Header("Normal speed range")]
    public float minSpeedMultiplier = 0.5f;
    public float maxSpeedMultiplier = 1.5f;

    [Header("Rare fast fish")]
    [Range(0f, 1f)]
    public float zoomyChance = 0.10f;
    public float zoomySpeedMultiplier = 2.5f;
    public float zoomyScaleMultiplier = 1.15f;
    public float zoomyBobMultiplier = 1.4f;

    [Header("Baby fish")]
    [Range(0f, 1f)]
    public float babyChance = 0.35f;     // tune this (35% feels good)
    public float babyScaleMultiplier = 0.5f;
    public float babySpeedMultiplier = 1.2f;

    [Header("Bob")]
    public float bobAmplitude = 0.06f;
    public float bobFrequency = 1.2f;

    private float finalSpeed;
    private float baseY;
    private float bobOffset;

    public void Initialize(float startY)
    {
        baseY = startY;
        bobOffset = Random.Range(0f, 1000f);

        bool isBaby = Random.value < babyChance;
        bool isZoomy = Random.value < zoomyChance;

        float speedMult = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);

        // baby adjustments
        if (isBaby)
        {
            transform.localScale *= babyScaleMultiplier;
            speedMult *= babySpeedMultiplier;
        }

        // zoomy overrides
        if (isZoomy)
        {
            speedMult *= zoomySpeedMultiplier;
            transform.localScale *= zoomyScaleMultiplier;
            bobAmplitude *= zoomyBobMultiplier;
        }

        finalSpeed = baseSpeed * speedMult;

        // ensure facing right
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = false;
    }

    void Update()
    {
        transform.position += Vector3.right * finalSpeed * Time.unscaledDeltaTime;

        float bob = Mathf.Sin((Time.unscaledTime + bobOffset) * bobFrequency) * bobAmplitude;
        Vector3 p = transform.position;
        p.y = baseY + bob;
        transform.position = p;
    }
}
