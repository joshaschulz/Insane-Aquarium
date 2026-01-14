using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_FishingMinigameFishController : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Scr_Fishpedia fishpedia;

    [Header("bounds")]
    public Collider2D minigameBoundsCollider;

    [Tooltip("set this to your MinigameFish layer (both front + side colliders)")]
    public LayerMask fishLayerMask;

    [Header("fish spawning")]
    [Tooltip("add multiple fish prefabs here. one will be instantiated for this minigame.")]
    public GameObject[] fishPrefabs;

    [Tooltip("weights aligned 1-to-1 with fishPrefabs. example: [60, 30, 10]. must be same length.")]
    public float[] fishSpawnWeights;

    [Tooltip("random spawn area. if set, fish spawns at a random point inside this collider's bounds.")]
    public Collider2D fishSpawnBoundsCollider;

    [Tooltip("optional: fixed spawn point. if assigned, this overrides random spawn bounds.")]
    public Transform fishSpawnPoint;

    [Tooltip("if true, spawns a fish whenever this controller is enabled.")]
    public bool spawnOnEnable = true;

    [Tooltip("if true, destroy any existing spawned fish when spawning a new one.")]
    public bool destroyExistingFishOnSpawn = true;

    [Header("progress behavior (fish)")]
    [Range(0f, 1f)]
    public float startingProgress01 = 0.20f;   // ✅ NEW: start at 20%

    public float secondsToFill = 1.5f;
    public float secondsToDrain = 6.0f;

    [Tooltip("covers the brief gap when fish swaps front + side colliders)")]
    public float hoverGraceSeconds = 0.12f;

    [Header("ui (fish bar)")]
    public RectTransform progressRoot;
    public RectTransform progressFillRect;
    public Image progressFillImage;
    public Color fishEmptyColor = Color.red;
    public Color fishFullColor = Color.green;

    [Header("hook aim source")]
    [Tooltip("drag your Scr_FishingLineToCursor here. hover checks use the line tip, not the mouse.")]
    public Scr_FishingLineToCursor fishingLine;

    private Camera cam;

    public GameObject hookedCrateImage;
    public GameObject fishHookImage;

    private float fishT01 = 0f;
    private float fishFullFillHeight = 0f;

    private Transform currentFishRoot;
    private float lastFishHoverHitTime = -999f;

    private Scr_FishMinigameMovement cachedFishMovement;
    private GameObject spawnedFishInstance;

    [Header("win panel")]
    public Scr_FishingMinigamePanel winPanel;
    private bool hasWonThisRun = false;

    private void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;

        cam = Camera.main;

        if (progressRoot != null)
            progressRoot.gameObject.SetActive(true);

        if (progressFillRect != null)
            fishFullFillHeight = progressFillRect.sizeDelta.y;

        fishT01 = Mathf.Clamp01(startingProgress01);
        SetFishFillHeight(fishT01);
        SetFishFillColor(fishT01);
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        fishingLine.cursorFollowSpeed = fishingLine.baseCursorFollowSpeed;
        fishHookImage.SetActive(true);

        if (spawnOnEnable)
        {
            SpawnOneFish();
        }
        else
        {
            TryCacheCurrentFishAndApplyBounds();
        }

    }

    private void LateUpdate()
    {
        if (cam == null) cam = Camera.main;

        if (cachedFishMovement == null)
        {
            if (spawnOnEnable)
                SpawnOneFish();
            else
                TryCacheCurrentFishAndApplyBounds();
        }

        UpdateFishHoverTargetWithGrace();
        UpdateFishProgress();

        SetFishFillHeight(fishT01);
        SetFishFillColor(fishT01);
    }

    private Vector2 GetAimWorldPoint()
    {
        if (fishingLine != null)
            return fishingLine.GetTipWorldPosition();

        if (cam == null) cam = Camera.main;
        return cam != null ? (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) : Vector2.zero;
    }

    // -------------------------
    // spawning
    // -------------------------
    private void SpawnOneFish()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            TryCacheCurrentFishAndApplyBounds();
            return;
        }

        if (!HasValidWeights())
        {
            Debug.LogWarning("fishSpawnWeights must be the same length as fishPrefabs and have a positive total. Falling back to uniform random.");
        }

        if (destroyExistingFishOnSpawn && spawnedFishInstance != null)
        {
            Destroy(spawnedFishInstance);
            spawnedFishInstance = null;
        }

        hasWonThisRun = false;

        fishT01 = Mathf.Clamp01(startingProgress01);

        currentFishRoot = null;
        lastFishHoverHitTime = -999f;

        Vector3 spawnPos = GetSpawnPosition();

        GameObject prefab = PickWeightedFishPrefab();
        spawnedFishInstance = Instantiate(prefab, spawnPos, Quaternion.identity);

        cachedFishMovement = spawnedFishInstance.GetComponentInChildren<Scr_FishMinigameMovement>();
        if (cachedFishMovement != null && minigameBoundsCollider != null)
        {
            cachedFishMovement.SetBounds(minigameBoundsCollider.bounds);
            cachedFishMovement.IdleOrMoveNormal();
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (fishSpawnPoint != null)
            return fishSpawnPoint.position;

        if (fishSpawnBoundsCollider != null)
        {
            Bounds b = fishSpawnBoundsCollider.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            return new Vector3(x, y, 0f);
        }

        return transform.position;
    }

    private bool HasValidWeights()
    {
        if (fishSpawnWeights == null) return false;
        if (fishSpawnWeights.Length != fishPrefabs.Length) return false;

        float total = 0f;
        for (int i = 0; i < fishSpawnWeights.Length; i++)
            total += Mathf.Max(0f, fishSpawnWeights[i]);

        return total > 0.0001f;
    }

    private GameObject PickWeightedFishPrefab()
    {
        if (!HasValidWeights())
            return fishPrefabs[Random.Range(0, fishPrefabs.Length)];

        float total = 0f;
        for (int i = 0; i < fishSpawnWeights.Length; i++)
            total += Mathf.Max(0f, fishSpawnWeights[i]);

        float r = Random.value * total;

        float cumulative = 0f;
        for (int i = 0; i < fishSpawnWeights.Length; i++)
        {
            cumulative += Mathf.Max(0f, fishSpawnWeights[i]);
            if (r <= cumulative)
                return fishPrefabs[i];
        }

        return fishPrefabs[fishPrefabs.Length - 1];
    }

    private void TryCacheCurrentFishAndApplyBounds()
    {
        cachedFishMovement = FindObjectOfType<Scr_FishMinigameMovement>();

        if (cachedFishMovement != null && minigameBoundsCollider != null)
        {
            cachedFishMovement.SetBounds(minigameBoundsCollider.bounds);
            cachedFishMovement.IdleOrMoveNormal();
        }
    }

    // -------------------------
    // hover + progress
    // -------------------------
    private void UpdateFishHoverTargetWithGrace()
    {
        Vector2 aimWorld = GetAimWorldPoint();
        Collider2D hit = Physics2D.OverlapPoint(aimWorld, fishLayerMask);

        if (hit != null)
        {
            Scr_FishMinigameMovement mover = hit.GetComponentInParent<Scr_FishMinigameMovement>();
            if (mover != null)
            {
                if (!mover.hooked)
                    mover.GoFast();

                currentFishRoot = mover.transform;
                lastFishHoverHitTime = Time.time;
                return;
            }
        }

        if (Time.time - lastFishHoverHitTime > hoverGraceSeconds)
            currentFishRoot = null;
    }

    private void UpdateFishProgress()
    {
        bool hoveringFish = (currentFishRoot != null);

        bool minigameStarted = (cachedFishMovement != null && cachedFishMovement.hooked);

        if (hoveringFish)
        {
            float fillRate = 1f / Mathf.Max(0.01f, secondsToFill);
            fishT01 += fillRate * Time.deltaTime;
        }
        else
        {
            if (minigameStarted)
            {
                float drainRate = 1f / Mathf.Max(0.01f, secondsToDrain);
                fishT01 -= drainRate * Time.deltaTime;
            }
        }

        fishT01 = Mathf.Clamp01(fishT01);

        if (!hasWonThisRun && fishT01 >= 1f)
        {
            hasWonThisRun = true;

            if (cachedFishMovement != null)
            {
                if (winPanel != null)
                    winPanel.ShowWithFish(cachedFishMovement.gameObject);
            }
            hookedCrateImage.SetActive(false);
            fishHookImage.SetActive(false);

            Cursor.visible = true;

            // NEW FISHPEDIA CODE
            // Enabling fishpedia buttons upon catching a fish
            foreach (var prefab in gameManager.fishPrefabs)
            {
                if (prefab.CompareTag(cachedFishMovement.tag))
                {
                    prefab.GetComponent<Scr_Fish>().numberCaught++;
                    int buttonIndex = 0;
                    foreach (Button button in fishpedia.buttons)
                    {
                        if (cachedFishMovement.name.Contains(button.name))
                        {
                            fishpedia.EnableEntryButton(buttonIndex);
                            fishpedia.entries[buttonIndex].transform.GetChild(2).Find("# Caught Numbers").GetComponent<TextMeshProUGUI>().text = prefab.GetComponent<Scr_Fish>().numberCaught.ToString();
                        }
                        buttonIndex++;
                    }
                }
            }


        }
        else if (!hasWonThisRun && fishT01 <= 0f)
        {
            hasWonThisRun = false;

            if (cachedFishMovement != null)
            {
                // kick out to main menu...
                gameManager.PlaySoundEffect(gameManager.SFX_Flush, 0.3f);
                gameManager.ClickButton(winPanel.flushFishButton);
            }
            hookedCrateImage.SetActive(false);
            fishHookImage.SetActive(false);
            
            Cursor.visible = true;
        }

    }

    public void ForceWinUIAndStop()
    {
        fishT01 = 1f;
        currentFishRoot = null;
        lastFishHoverHitTime = -999f;

        SetFishFillHeight(1f);
        SetFishFillColor(1f);

        enabled = false;
    }

    private void SetFishFillHeight(float t)
    {
        if (progressFillRect == null) return;

        if (fishFullFillHeight <= 0f)
            fishFullFillHeight = progressFillRect.sizeDelta.y;

        float h = fishFullFillHeight * Mathf.Clamp01(t);
        progressFillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }

    private void SetFishFillColor(float t)
    {
        if (progressFillImage == null) return;
        progressFillImage.color = Color.Lerp(fishEmptyColor, fishFullColor, Mathf.Clamp01(t));
    }

    public float GetFishProgress01()
    {
        return fishT01;
    }

    public void BagFish()
    {
        gameManager.BagFishingFish(spawnedFishInstance);
        ResetFishingMinigame();
        DespawnFishingMinigame();
    }

    public void FlushFish()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_Flush, 0.3f);
        ResetFishingMinigame();
        DespawnFishingMinigame();
    }

    public void ResetFishingMinigame()
    {
        enabled = true;
        winPanel.lineConnectorRoot.SetActive(true);
        Cursor.visible = true;
    }

    public void DespawnFishingMinigame()
    {
        Destroy(spawnedFishInstance);
        spawnedFishInstance = null;

        if (winPanel.winChest != null)
        {
            Destroy(winPanel.winChest);
            winPanel.winChest = null;
        }
            
    }

    public void PlayReelDropSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_Click, 1);
    }

}
