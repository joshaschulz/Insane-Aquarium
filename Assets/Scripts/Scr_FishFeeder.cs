using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishFeeder : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro speedText;

    [Header("references")]
    public Scr_GameManager gameManager;   // assign in inspector
    public Scr_TankBounds Scr_TankBounds;     // assign in inspector OR auto-find in Awake
    public Scr_TimeHandler Scr_TimeHandler;   // assign in inspector OR auto-find on gameManager
    public Transform spawnPosition;

    [Header("feeding settings")]
    [Tooltip("Food prefab this feeder will drop.")]
    public GameObject currentFoodToDrop;
    public GameObject buttonThatWouldFlash; //______________________________________________________________________________________________________________________________________________DO THIS

    [Tooltip("How many ticks between feeding events.")]
    public int feedIntervalTicks = 30;

    [Tooltip("How many pellets to drop each time it feeds.")]
    public int amountPerFeed = 1;

    [Tooltip("Preset speeds in ticks. Right-click with empty hand cycles these.")]
    public int[] availableFeedIntervals = new int[] { 1, 2, 3, 5, 10 };

    private int currentSpeedIndex = 2; // default index into availableFeedIntervals (e.g., 3 ticks)
    private int tickCounter = 0;

    [Header("Tank visuals")]
    public GameObject[] foodSprites;   // assign all 20 sprite holders in inspector

    private bool pausedBecauseEmpty = false;
    public int startSpeedIndex = 0; // 0 = paused by default


    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        UpdateSpeedText();
    }

    private void Awake()
    {

        // auto-find tankBounds if not set
        if (Scr_TankBounds == null && transform.parent != null)
        {
            Scr_TankBounds = transform.parent.GetComponentInChildren<Scr_TankBounds>();
        }

        if (Scr_TankBounds == null)
        {
            Debug.LogWarning($"{name} (FishFeeder): no Scr_TankBounds assigned/found.");
        }

        ApplyStartSpeed();
    }

    private void LateUpdate()
    {
        KeepSpeedTextUpright();
    }

    private void OnEnable()
    {
        // Optionally, get a reference to the TickHandler (assuming there's only one or it’s a singleton)
        Scr_TimeHandler = FindObjectOfType<Scr_TimeHandler>();

        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.AddListener(OnTickEvent);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks.
        if (Scr_TimeHandler != null)
        {
            Scr_TimeHandler.tickEvent.RemoveListener(OnTickEvent);
        }

    }

    // =========================
    // TICK EVENT HANDLER
    // =========================
    public void OnTickEvent()
    {
        if (feedIntervalTicks <= 0) return; // 0 = paused
        if (currentFoodToDrop == null) return;
        if (pausedBecauseEmpty) return;

        tickCounter++;

        if (tickCounter >= feedIntervalTicks)
        {
            tickCounter = 0;
            SpawnFoodBurst();
        }
    }

    private void SpawnFoodBurst()
    {
        for (int i = 0; i < amountPerFeed; i++)
        {
            bool spawned = SpawnOneFood();
            if (!spawned)
            {
                // ran out mid-burst; stop trying this tick
                break;
            }
        }
    }

    private bool SpawnOneFood()
    {
        if (gameManager == null)
        {
            Debug.LogWarning($"{name} (FishFeeder): gameManager not assigned.");
            return false;
        }

        if (currentFoodToDrop == null)
        {
            Debug.LogWarning($"{name} (FishFeeder): currentFoodToDrop is null.");
            return false;
        }

        int amountLeft = gameManager.GetFishFoodAmount(currentFoodToDrop);

        // OUT OF FOOD: play error once, then pause automatic feeding
        if (amountLeft <= 0)
        {
            if (!pausedBecauseEmpty)
            {
                // call your existing manager function ONCE so it plays the error + flashes red
                bool flipped = (transform.localScale.x == -1);
                gameManager.DropFoodFromFeeder(currentFoodToDrop, spawnPosition.position, buttonThatWouldFlash, flipped);

                pausedBecauseEmpty = true;
            }

            UpdateFeederFillVisual(0);
            return false;
        }

        // HAVE FOOD: ensure not paused and fire normally
        pausedBecauseEmpty = false;

        bool flippedSpawn = (transform.localScale.x == -1);
        gameManager.DropFoodFromFeeder(currentFoodToDrop, spawnPosition.position, buttonThatWouldFlash, flippedSpawn);

        // Update visual using the new amount after dropping
        UpdateFeederFillVisual(gameManager.GetFishFoodAmount(currentFoodToDrop));
        return true;
    }

    private Vector2 GetRandomTopOfTankPosition()
    {
        if (Scr_TankBounds == null)
        {
            Debug.LogWarning($"{name} (FishFeeder): tankBounds is null, using feeder position.");
            return transform.position;
        }

        Bounds b = Scr_TankBounds.GetBounds();

        float minX = b.min.x;
        float maxX = b.max.x;
        float x = Random.Range(minX, maxX);
        float y = b.max.y;   // top of the tank

        return new Vector2(x, y);
    }

    // =========================
    // Called by ClickDetection
    // =========================

    public void SetFoodType(GameObject foodPrefab, GameObject foodButtonSelected)
    {
        pausedBecauseEmpty = false;

        currentFoodToDrop = foodPrefab;
        buttonThatWouldFlash = foodButtonSelected;

        UpdateFeederTypeVisual(foodPrefab);
        UpdateFeederFillVisual(gameManager.GetFishFoodAmount(currentFoodToDrop));
        Debug.Log($"{name} (FishFeeder): set food type to {(foodPrefab != null ? foodPrefab.name : "NONE")}");
    }

    public void CycleSpeed()
    {
        if (availableFeedIntervals == null || availableFeedIntervals.Length == 0)
        {
            Debug.LogWarning($"{name} (FishFeeder): no availableFeedIntervals configured.");
            return;
        }

        currentSpeedIndex = (currentSpeedIndex + 1) % availableFeedIntervals.Length;
        feedIntervalTicks = availableFeedIntervals[currentSpeedIndex];
        tickCounter = 0;

        UpdateSpeedText();


        if (feedIntervalTicks == 0)
        {
            Debug.Log($"{name} (FishFeeder): speed set to PAUSED.");
        }
        else
        {
            Debug.Log($"{name} (FishFeeder): speed set to every {feedIntervalTicks} ticks.");
        }
    }

    private void ApplyStartSpeed()
    {
        if (availableFeedIntervals == null || availableFeedIntervals.Length == 0) return;

        currentSpeedIndex = Mathf.Clamp(startSpeedIndex, 0, availableFeedIntervals.Length - 1);
        feedIntervalTicks = availableFeedIntervals[currentSpeedIndex];
        tickCounter = 0;

        UpdateSpeedText();
    }

    public void UpdateFeederFillVisual(int amount)
    {
        if (foodSprites == null || foodSprites.Length == 0)
            return;

        int max = foodSprites.Length;
        amount = Mathf.Clamp(amount, 0, max);

        for (int i = 0; i < max; i++)
        {
            foodSprites[i].SetActive(i < amount);
        }
    }
    public void UpdateFeederTypeVisual(GameObject foodType)
    {
        if (foodSprites == null || foodSprites.Length == 0 || foodType == null)
            return;

        // Try to get a pool of sprites from the food's behavior
        List<Sprite> spritePool = null;
        Scr_FoodBehavior foodBehavior = foodType.GetComponent<Scr_FoodBehavior>();
        if (foodBehavior != null && foodBehavior.SpriteOptions != null && foodBehavior.SpriteOptions.Count > 0)
        {
            spritePool = foodBehavior.SpriteOptions;
        }

        // Fallback: single sprite from the prefab's SpriteRenderer
        Sprite fallbackSprite = null;
        var srcSR = foodType.GetComponent<SpriteRenderer>();
        if (srcSR != null)
        {
            fallbackSprite = srcSR.sprite;
        }

        foreach (GameObject go in foodSprites)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            // Pick a sprite for this slot
            Sprite chosenSprite = null;
            if (spritePool != null && spritePool.Count > 0)
            {
                int idx = Random.Range(0, spritePool.Count);
                chosenSprite = spritePool[idx];
            }
            else
            {
                chosenSprite = fallbackSprite;
            }

            if (chosenSprite == null)
            {
                sr.sprite = null;
                continue;
            }

            sr.sprite = chosenSprite;

            // Scale this slot so its sprite fits nicely in the tank
            Bounds b = chosenSprite.bounds;
            float maxSize = Mathf.Max(b.size.x, b.size.y);
            float desired = 0.5f;               // your good size
            float scale = desired / (maxSize > 0f ? maxSize : 1f);

            go.transform.localScale = Vector3.one * scale;

            go.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
        }
    }
    private void UpdateSpeedText()
    {
        if (speedText == null)
            return;

        int displaySpeed = GetDisplaySpeedLevel();
        speedText.text = displaySpeed.ToString();

        speedText.color = (displaySpeed == 0) ? Color.gray : Color.white;
    }

    private int GetDisplaySpeedLevel()
    {
        // paused always shows 0
        if (feedIntervalTicks == 0)
            return 0;

        // count how many non-zero speeds are "up to" our currentSpeedIndex
        // so it displays 1,2,3,... even if actual tick intervals skip numbers
        int level = 0;

        for (int i = 0; i <= currentSpeedIndex && i < availableFeedIntervals.Length; i++)
        {
            if (availableFeedIntervals[i] != 0)
                level++;
        }

        // level is now 1..N for non-zero entries
        return level;
    }

    private void KeepSpeedTextUpright()
    {
        if (speedText == null) return;

        float parentX = transform.localScale.x;

        Vector3 s = speedText.transform.localScale;
        s.x = Mathf.Abs(s.x) * Mathf.Sign(parentX);
        speedText.transform.localScale = s;
    }
}
