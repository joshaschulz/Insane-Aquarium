using System.Collections.Generic;
using UnityEngine;

public class Scr_MenuStarfishSpawner : MonoBehaviour
{
    [Header("Starfish prefab (menu-only)")]
    public GameObject starfishPrefab;

    [Header("Spawn bounds (top edge used)")]
    public BoxCollider2D spawnBounds2D;

    [Header("Limits")]
    public int maxStarfishOnScreen = 1;

    [Header("Spawn cadence (chance-based)")]
    public float checkInterval = 1.0f;        // how often we roll
    [Range(0f, 1f)]
    public float spawnChance = 0.20f;         // 0.20 @ 1s checks ~= one spawn per ~5s on average

    [Header("Anti-spam / spacing")]
    public float minSecondsBetweenSpawns = 5.0f; // hard cooldown
    public float initialSpawnDelay = 2.0f;       // no spawning immediately on scene start

    [Header("Anti-clump (X spacing near top)")]
    public float minXSeparation = 0.8f;
    public float spawnLaneCheckHeight = 1.5f;
    public int spawnPickAttempts = 20;

    private float checkTimer = 0f;
    private float lastSpawnTime = -999f;

    private bool spawningEnabled = true;


    private readonly List<GameObject> activeStarfish = new List<GameObject>();

    void Update()
    {
        if (!spawningEnabled)
            return;

        if (starfishPrefab == null || spawnBounds2D == null) return;

        Bounds b = spawnBounds2D.bounds;
        float spawnY = b.max.y;
        float despawnY = GetDespawnY(b);

        // cleanup + despawn
        for (int i = activeStarfish.Count - 1; i >= 0; i--)
        {
            GameObject s = activeStarfish[i];
            if (s == null)
            {
                activeStarfish.RemoveAt(i);
                continue;
            }

            if (s.transform.position.y < despawnY)
            {
                Destroy(s);
                activeStarfish.RemoveAt(i);
            }
        }

        // no immediate spawn
        if (Time.unscaledTime < initialSpawnDelay)
            return;

        // roll for spawns
        checkTimer += Time.unscaledDeltaTime;

        if (checkTimer >= checkInterval && activeStarfish.Count < maxStarfishOnScreen)
        {
            checkTimer = 0f;

            // cooldown gate
            if (Time.unscaledTime - lastSpawnTime < minSecondsBetweenSpawns)
                return;

            // chance gate
            if (Random.value <= spawnChance)
            {
                lastSpawnTime = Time.unscaledTime;
                SpawnOne(b, spawnY);
            }
        }
    }
    public void StopAndClear()
    {
        spawningEnabled = false;

        // destroy everything this spawner created
        foreach (var obj in activeStarfish) // or activeStarfish
        {
            if (obj != null)
                Destroy(obj);
        }

        activeStarfish.Clear(); // or activeStarfish.Clear()
    }

    public void StartSpawning()
    {
        spawningEnabled = true;

        // reset cadence safely
        checkTimer = 0f;
        lastSpawnTime = Time.unscaledTime;
    }

    private float GetDespawnY(Bounds b)
    {
        return b.min.y - 1.0f;
    }

    private void SpawnOne(Bounds b, float spawnY)
    {
        float x = PickNonClumpyX(b);

        GameObject s = Instantiate(starfishPrefab);
        s.transform.position = new Vector3(x, spawnY, 0f);
        s.tag = "Menu Starfish";

        var script = s.GetComponent<Scr_MenuStarfish>();
        if (script != null)
            script.Initialize();

        activeStarfish.Add(s);
    }

    private float PickNonClumpyX(Bounds b)
    {
        for (int attempt = 0; attempt < spawnPickAttempts; attempt++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            if (IsXSpacedEnough(x))
                return x;
        }

        return Random.Range(b.min.x, b.max.x);
    }

    private bool IsXSpacedEnough(float candidateX)
    {
        Bounds b = spawnBounds2D.bounds;
        float topRegionY = b.max.y - spawnLaneCheckHeight;

        for (int i = 0; i < activeStarfish.Count; i++)
        {
            GameObject s = activeStarfish[i];
            if (s == null) continue;

            // only compare starfish still near the top
            if (s.transform.position.y < topRegionY)
                continue;

            float dx = Mathf.Abs(s.transform.position.x - candidateX);
            if (dx < minXSeparation)
                return false;
        }

        return true;
    }
}
