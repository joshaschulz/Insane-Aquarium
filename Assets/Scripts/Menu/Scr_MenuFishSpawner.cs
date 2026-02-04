using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Scr_MenuFishSpawner : MonoBehaviour
{
    [Header("Fish prefabs (menu-only versions)")]
    public GameObject[] fishPrefabs;

    [Header("Swim area bounds")]
    public BoxCollider2D swimBounds2D;
    public BoxCollider2D spawnBounds2D;
    public float edgePadding = 0.25f;

    [Header("Spawn timing")]
    public int maxFishOnScreen = 6;
    public float spawnInterval = 1.2f;

    [Header("Anti-clump")]
    public float minYSeparation = 0.6f;
    public float spawnLaneCheckWidth = 2.0f;
    public int spawnPickAttempts = 20;

    [Header("Startup delay")]
    public float startDelay = 0.5f;   // wait before *anything* starts

    private float spawnTimer = 0f;
    private float startTime;

    private bool spawningEnabled = true;
    public int currentOrderInLayer;


    private readonly List<GameObject> activeFish = new List<GameObject>();

    void Start()
    {
        // delay the entire system so loading finishes first
        startTime = Time.unscaledTime + startDelay;
    }

    void Update()
    {
        if (!spawningEnabled)
            return;

        // global startup delay
        if (Time.unscaledTime < startTime)
            return;

        if (swimBounds2D == null || fishPrefabs == null || fishPrefabs.Length == 0)
            return;

        Bounds b = swimBounds2D.bounds;
        float leftEdge = b.min.x;
        float rightEdge = b.max.x;

        // cleanup + despawn
        for (int i = activeFish.Count - 1; i >= 0; i--)
        {
            GameObject f = activeFish[i];
            if (f == null)
            {
                activeFish.RemoveAt(i);
                continue;
            }

            if (!b.Contains(f.transform.position))
            {
                Destroy(f);
                activeFish.RemoveAt(i);
            }
        }

        // spawn logic
        spawnTimer += Time.unscaledDeltaTime;

        if (spawnTimer >= spawnInterval && activeFish.Count < maxFishOnScreen)
        {
            spawnTimer = 0f;
            SpawnOne(spawnBounds2D.bounds);
        }
    }



    public void StopAndClear()
    {
        spawningEnabled = false;

        // destroy everything this spawner created
        foreach (var obj in activeFish) // or activeStarfish
        {
            if (obj != null)
                Destroy(obj);
        }

        activeFish.Clear(); // or activeStarfish.Clear()
    }

    public void StartSpawning()
    {
        spawningEnabled = true;

        // reset timers so we don’t burst-spawn
        spawnTimer = 0f;
        startTime = Time.unscaledTime + startDelay;
    }

    private void SpawnOne(Bounds b)
    {
        float y = PickNonClumpyY(b);

        GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        GameObject fish = Instantiate(prefab);

        //Fish appear in front and behind of fish sometimes because of same order in layer
        GameObject fishSideContainer = fish.transform.GetChild(0).gameObject;
        GameObject fishFrontContainer = null;

        if (fish.transform.childCount > 1)
            fishFrontContainer = fish.transform.GetChild(1).gameObject;

        fishSideContainer.GetComponent<SortingGroup>().sortingOrder = currentOrderInLayer % 10;

        if (fishFrontContainer != null)
            fishFrontContainer.GetComponent<SortingGroup>().sortingOrder = currentOrderInLayer % 10;

        currentOrderInLayer++;

        fish.tag = "Menu Fish";
        fish.transform.position = new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), 0f);

        var script = fish.GetComponent<Scr_MenuFish>();
        if (script != null)
            script.Initialize(y);

        activeFish.Add(fish);
    }

    private float PickNonClumpyY(Bounds b)
    {
        float leftEdge = b.min.x;

        for (int attempt = 0; attempt < spawnPickAttempts; attempt++)
        {
            float y = Random.Range(b.min.y, b.max.y);

            if (IsYSpacedEnough(y, leftEdge))
                return y;
        }

        return Random.Range(b.min.y, b.max.y);
    }

    private bool IsYSpacedEnough(float candidateY, float leftEdge)
    {
        for (int i = 0; i < activeFish.Count; i++)
        {
            GameObject f = activeFish[i];
            if (f == null) continue;

            // only compare fish still near spawn side
            if (f.transform.position.x > leftEdge + spawnLaneCheckWidth)
                continue;

            if (Mathf.Abs(f.transform.position.y - candidateY) < minYSeparation)
                return false;
        }

        return true;
    }
}
