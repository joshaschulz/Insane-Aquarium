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

    private bool hovered;
    private int isForward = 1;

    private Collider2D myCollider;
    private Animator sideAnimator;

    private int clicks = 0;

    public void Initialize(float startY)
    {
        sideAnimator = transform.GetChild(0).GetComponent<Animator>();

        baseY = startY;
        bobOffset = Random.Range(0f, 1000f);

        float randomScale = Random.Range(0.4f, 1.1f);

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

        transform.localScale *= randomScale;

        finalSpeed = baseSpeed * speedMult * randomScale;

        sideAnimator.speed = finalSpeed;

        // ensure facing right
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = false;
    }

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            CheckMouseClick();
        }

        if (hovered)
            return;

        transform.position += Vector3.right * finalSpeed * Time.unscaledDeltaTime * isForward;

        float bob = Mathf.Sin((Time.unscaledTime + bobOffset) * bobFrequency) * bobAmplitude;
        Vector3 p = transform.position;
        p.y = baseY + bob;
        transform.position = p;
    }
    /*
    private void OnMouseEnter()
    {
        if (!hovered)
        {
            hovered = true;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            Invoke(nameof(MoveAgain), Random.Range(2f, 4f));
        }

    }
    */
    private void MoveAgain()
    {
        hovered = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    private void StopMoving()
    {
        hovered = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    void CheckMouseClick()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == myCollider)
            {
                clicks++;

                if (clicks == 1)
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    isForward = -1;
                    return;
                }
                else if (clicks == 2)
                {
                    StopMoving();
                    Invoke(nameof(MoveAgain), Random.Range(2f, 4f));
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    isForward = 1;
                    return;
                }
                else
                {
                    return;
                }

            }
        }
    }


}
