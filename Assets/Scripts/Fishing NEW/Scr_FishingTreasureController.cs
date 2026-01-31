// ================================
// Scr_FishingMinigameChestController.cs
// put this on the same gameobject as Scr_FishingMinigameFishController
// ================================
using UnityEngine;
using UnityEngine.UI;

public class Scr_FishingMinigameChestController : MonoBehaviour
{
    private Scr_GameManager gameManager;

    [Header("hover grace")]
    [Tooltip("covers the brief gap when colliders flicker")]
    public float hoverGraceSeconds = 0.12f;

    [Tooltip("set this to your MinigameChest layer (chest collider)")]
    public LayerMask chestLayerMask;

    [Header("treasure chest (spawning)")]
    public GameObject[] treasureChestPrefabs;

    public GameObject[] rewardPrefabs;

    // one-time trigger settings
    [Range(0f, 1f)]
    public float spawnAtFishProgress = 0.25f;

    [Range(0f, 1f)]
    public float spawnChanceAtFishProgress = 0.50f;

    // kept for inspector compatibility (no longer used for spawning logic)
    [Range(0f, 1f)]
    public float chestSpawnChance = 0.25f;

    public float chestSpawnCheckInterval = 5.0f;

    public float chestSpawnTopPadding = 0.5f;

    public float chestStopViewportY = 0.65f;

    public float chestFallSpeed = 2.0f;

    [Header("treasure chest (hover progress)")]
    public float chestSecondsToFill = 1.0f;
    public float chestSecondsToDrain = 4.0f;

    [Header("ui (chest bar)")]
    public bool chestBarAlwaysVisible = true;
    public RectTransform chestProgressRoot;
    public RectTransform chestProgressFillRect;
    public Image chestProgressFillImage;
    public Color chestEmptyColor = Color.red;
    public Color chestFullColor = Color.green;

    [Header("chest bar follow (screen space camera)")]
    public Canvas uiCanvas;
    public RectTransform chestBarRoot;
    public Vector2 chestBarPixelOffset = new Vector2(0f, 40f);
    public Vector2 canvasOffset = new Vector2(0f, 80f);

    [Header("results")]
    public int chestCount = 0;

    [Header("cursor heaviness (via fishing line lag)")]
    public Scr_FishingLineToCursor fishingLine;
    public float cursorLagIncreasePerChest = 1.5f;
    public float minCursorFollowSpeed = 3.0f;

    private Scr_FishingMinigameFishController fishController;

    private Camera cam;

    public GameObject activeChest;


    public GameObject hookedCrateImage;

    private float chestT01 = 0f;
    private float chestFullFillHeight = 0f;

    private float lastChestHoverHitTime = -999f;
    private bool chestHovering = false;

    private bool chestRolledThisRun = false;
    public GameObject lastChest;


    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
        cam = Camera.main;

        fishController = GetComponent<Scr_FishingMinigameFishController>();

        if (chestProgressRoot != null)
            chestProgressRoot.gameObject.SetActive(chestBarAlwaysVisible);

        if (chestProgressFillRect != null)
            chestFullFillHeight = chestProgressFillRect.sizeDelta.y;

        SetChestFillHeight(chestT01);
        SetChestFillColor(chestT01);
    }

    private void OnEnable()
    {
        chestRolledThisRun = false;

        if (activeChest != null)
        {
            Destroy(activeChest);
            activeChest = null;
        }

        chestCount = 0;
        chestT01 = 0f;
        chestHovering = false;
        lastChestHoverHitTime = -999f;

        if (!chestBarAlwaysVisible && chestProgressRoot != null)
            chestProgressRoot.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cam == null) cam = Camera.main;

        TrySpawnChestAtFishProgress();
        UpdateChestFall();
        UpdateChestHoverWithGrace();
        UpdateChestProgress();
        UpdateChestBarFollow();

        SetChestFillHeight(chestT01);
        SetChestFillColor(chestT01);
    }

    private void TrySpawnChestAtFishProgress()
    {
        if (activeChest != null) return;
        if (chestRolledThisRun) return;
        if (treasureChestPrefabs == null || treasureChestPrefabs.Length == 0) return;
        if (fishController == null) return;
        if (cam == null) return;
        if (!gameManager.tutorials.tutorialCompleted) return;

        float fishT = fishController.GetFishProgress01();
        if (fishT < spawnAtFishProgress) return;

        chestRolledThisRun = true;

        if (Random.value > spawnChanceAtFishProgress) return;

        // -------------------------
        // NEW: X range comes from the same bounds the fish uses
        // -------------------------
        float minX, maxX;

        if (fishController.minigameBoundsCollider != null)
        {
            Bounds fishBounds = fishController.minigameBoundsCollider.bounds;
            minX = fishBounds.min.x;
            maxX = fishBounds.max.x;

            // OPTIONAL (recommended): intersect with camera X so chest stays on screen
            float halfW = cam.orthographicSize * cam.aspect;
            float camMinX = cam.transform.position.x - halfW;
            float camMaxX = cam.transform.position.x + halfW;

            minX = Mathf.Max(minX, camMinX);
            maxX = Mathf.Min(maxX, camMaxX);

            // safety if the intersection got inverted
            if (maxX <= minX)
            {
                minX = fishBounds.min.x;
                maxX = fishBounds.max.x;
            }
        }
        else
        {
            // fallback to camera range if bounds missing
            float halfW = cam.orthographicSize * cam.aspect;
            minX = cam.transform.position.x - halfW;
            maxX = cam.transform.position.x + halfW;
        }

        float camTopY = cam.transform.position.y + cam.orthographicSize;
        float spawnY = camTopY + chestSpawnTopPadding;

        float x = Random.Range(minX, maxX);

        GameObject prefab = (gameManager.skills.currentFishingSkills[2]) ? treasureChestPrefabs[1] : treasureChestPrefabs[0];

        lastChest = prefab;

        activeChest = Instantiate(prefab, new Vector3(x, spawnY, 0f), Quaternion.identity);

        if (chestProgressRoot != null)
            chestProgressRoot.gameObject.SetActive(true);
    }

    public void ClearActiveChestAndUI()
    {
        if (activeChest != null)
        {
            Destroy(activeChest);
            activeChest = null;
        }

        chestT01 = 0f;
        chestHovering = false;
        lastChestHoverHitTime = -999f;

        if (chestProgressRoot != null)
            chestProgressRoot.gameObject.SetActive(chestBarAlwaysVisible);
    }

    private void UpdateChestFall()
    {
        if (activeChest == null || cam == null) return;

        float z = Mathf.Abs(cam.transform.position.z);
        Vector3 stopWorld = cam.ViewportToWorldPoint(new Vector3(0.5f, chestStopViewportY, z));
        float stopY = stopWorld.y;

        Vector3 p = activeChest.transform.position;
        if (p.y > stopY)
        {
            p.y -= chestFallSpeed * Time.deltaTime;
            if (p.y < stopY) p.y = stopY;
            activeChest.transform.position = p;
        }
    }

    private void UpdateChestHoverWithGrace()
    {
        if (cam == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, chestLayerMask);

        if (hit != null)
        {
            if (activeChest != null && hit.transform.IsChildOf(activeChest.transform))
            {
                chestHovering = true;
                lastChestHoverHitTime = Time.time;
                return;
            }

            if (activeChest != null && hit.gameObject == activeChest)
            {
                chestHovering = true;
                lastChestHoverHitTime = Time.time;
                return;
            }
        }

        if (Time.time - lastChestHoverHitTime > hoverGraceSeconds)
            chestHovering = false;
    }

    private void UpdateChestProgress()
    {
        if (chestHovering && activeChest != null)
        {
            float fillRate = 1f / Mathf.Max(0.01f, chestSecondsToFill);
            chestT01 += fillRate * Time.deltaTime;
        }
        else
        {
            float drainRate = 1f / Mathf.Max(0.01f, chestSecondsToDrain);
            chestT01 -= drainRate * Time.deltaTime;
        }

        chestT01 = Mathf.Clamp01(chestT01);

        if (activeChest != null && chestT01 >= 1f)
        {
            chestCount += 1;

            Destroy(activeChest);
            activeChest = null;

            chestT01 = 0f;

            if (fishingLine != null && fishingLine.useCursorLag)
            {
                if (gameManager.baitAndTackle.currentTackleEquipped == 1) //if ducky bobber is equipped, no weight for the crates
                {
                    fishingLine.cursorFollowSpeed = Mathf.Max(minCursorFollowSpeed, fishingLine.cursorFollowSpeed);
                }
                else
                {
                    fishingLine.cursorFollowSpeed = Mathf.Max(minCursorFollowSpeed, fishingLine.cursorFollowSpeed - cursorLagIncreasePerChest);
                }


                hookedCrateImage.GetComponent<SpriteRenderer>().sprite = treasureChestPrefabs[lastChest.name.Contains("Golden") ? 1 : 0].GetComponentInChildren<SpriteRenderer>().sprite;

                hookedCrateImage.SetActive(true);
                gameManager.PlaySoundEffect(gameManager.SFX_catchCrate, 0.8f);
            }

            if (!chestBarAlwaysVisible && chestProgressRoot != null)
                chestProgressRoot.gameObject.SetActive(false);
        }
        else
        {
            if (!chestBarAlwaysVisible && chestProgressRoot != null)
                chestProgressRoot.gameObject.SetActive(activeChest != null);
        }
    }

    private void UpdateChestBarFollow()
    {
        if (uiCanvas == null || chestBarRoot == null) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        if (activeChest == null)
        {
            chestBarRoot.gameObject.SetActive(false);
            return;
        }

        chestBarRoot.gameObject.SetActive(true);

        //Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, activeChest.transform.position);
        Vector2 screenPoint = cam.WorldToScreenPoint(activeChest.transform.position);
        //screenPoint += chestBarPixelOffset;
        screenPoint += canvasOffset;

        RectTransform canvasRect = uiCanvas.transform as RectTransform;

        Vector2 localPoint;
        bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            uiCanvas.worldCamera,
            out localPoint
        );

        if (!ok) return;

        chestBarRoot.anchoredPosition = localPoint;
    }

    private void SetChestFillHeight(float t)
    {
        if (chestProgressFillRect == null) return;

        if (chestFullFillHeight <= 0f)
            chestFullFillHeight = chestProgressFillRect.sizeDelta.y;

        float h = chestFullFillHeight * Mathf.Clamp01(t);
        chestProgressFillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    private void SetChestFillColor(float t)
    {
        if (chestProgressFillImage == null) return;
        chestProgressFillImage.color = Color.Lerp(chestEmptyColor, chestFullColor, Mathf.Clamp01(t));
    }
}
