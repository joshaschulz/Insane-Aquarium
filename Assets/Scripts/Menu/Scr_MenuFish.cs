using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    private bool isIdle;
    private int isForward = 1;

    private Collider2D myCollider;
    private Animator sideAnimator;

    private int clicks = 0;

    private float randomScale;

    public void Initialize(float startY)
    {
        sideAnimator = transform.GetChild(0).GetComponent<Animator>();

        baseY = startY;
        bobOffset = Random.Range(0f, 1000f);

        randomScale = Random.Range(0.4f, 1.1f);

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

        transform.GetChild(0).GetComponent<SortingGroup>().sortingOrder = (int)(10 * randomScale);
        transform.GetChild(1).GetComponent<SortingGroup>().sortingOrder = (int)(10 * randomScale);


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

        if (isIdle)
            return;

        transform.position += Vector3.right * finalSpeed * Time.unscaledDeltaTime * isForward;

        float bob = Mathf.Sin((Time.unscaledTime + bobOffset) * bobFrequency) * bobAmplitude;
        Vector3 p = transform.position;
        p.y = baseY + bob;
        transform.position = p;
    }

    private void MoveAgain()
    {
        isIdle = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        finalSpeed *= 1.5f;
    }

    private void StopMoving()
    {
        isIdle = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        Invoke(nameof(MoveAgain), 2f);
    }
    void CheckMouseClick()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == myCollider)
            {
                if (!isIdle)
                {
                    transform.GetChild(0).gameObject.transform.localScale = new Vector3(transform.GetChild(0).gameObject.transform.localScale.x * -1, transform.GetChild(0).gameObject.transform.localScale.y, transform.GetChild(0).gameObject.transform.localScale.z);
                    isForward = -isForward;
                    StopMoving();

                    Scr_GameManager gameManager = FindObjectOfType<Scr_GameManager>();

                    List <AudioClip> bubblesSFX = new List<AudioClip> { gameManager.SFX_Bubbles1, gameManager.SFX_Bubbles2 };
                    List<float> bubblesVolumes = new List<float> { 7f, 0.5f };
                    List<float> bubblesLowerPitches = new List<float> { 0.9f, 0.6f };
                    List<float> bubblesUpperPitches = new List<float> { 1.1f, 0.8f };
                    gameManager.PlayRandomSoundEffect(bubblesSFX, bubblesVolumes, bubblesLowerPitches, bubblesUpperPitches);

                    gameManager.SpawnParticles(gameManager.bubblesEffectPrefab, transform.position, transform.rotation, null);
                    return;
                }
            }
        }
    }


}
