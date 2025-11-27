using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Fishing : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Scr_Fishpedia fishpedia;

    public Transform pipeSpawnPoint;
    public GameObject[] pipePrefabs;

    AudioSource ReelingAudioSource;
    private bool isReeling = false; // Flag to track if the player is reeling
    private float scrollInputThreshold = 0.005f; // Minimum threshold for scroll input
    private float scrollTimeout = 0.4f; // Time to wait after stopping scroll input before stopping sound
    private float scrollTimer = 0f; // Timer to track scroll inactivity
    public GameObject toiletFish;
    private GameObject toiletFishToDestroy;
    GameObject caughtToiletFish;
    private float scrollInput;
    public float reelInSpeed;
    public float lateralPullStrength = 1.5f; // Tune this to make the fish swing more
    public float fishEscapeHeight;
    public Transform toiletWinLine;
    //public Transform bathroomTransform;
    public Button goToBathroom;

    public int numberOfPipes = 3;
    public Camera mainCamera;
    public float cameraSmooth = 5f;
    public float rodScreenHeight = 0.4f; // 0 = bottom, 1 = top

    public float scaleReductionWhenFishing = 0.25f;
    private Vector3 originalFishScale;

    public Sprite leftRodSprite, rightRodSprite;
    public float rotationSpeed;

    private SpriteRenderer spriteRenderer;
    private float screenMiddleX;
    private bool isLeft; // Track the current state to avoid unnecessary updates

    private Vector3 fishSpawnPosition;
    private Scr_LineConnector lineConnector;
    private Scr_TimeHandler Scr_TimeHandler;
    public Scr_SpawnToiletFish Scr_SpawnToiletFish;

    [SerializeField] private List<GameObject> fishesToCatch;
    [SerializeField] private List<int> weightedChanceToCatch;

    [SerializeField] private Vector3 leftLineStartPosition, rightLineStartPosition;

    void OnDisable()
    {
        if (toiletFish)
            FishEscape();
    }

    private void OnEnable()
    {
        gameManager = Scr_GameManager.GMinstance;

        StartCoroutine(gameManager.RecenterThenUnlock());

        SpawnPipes(numberOfPipes);
    }

    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        ReelingAudioSource = GetComponent<AudioSource>();
        ReelingAudioSource.clip = gameManager.SFX_Reeling;
        screenMiddleX = Screen.width / 2; // Get middle X point of the screen

        Scr_SpawnToiletFish = FindObjectOfType<Scr_SpawnToiletFish>();


        lineConnector = transform.GetChild(1).GetComponent<Scr_LineConnector>();

        ChangeGameSettings();



        // Initialize sprite based on starting position
        isLeft = Input.mousePosition.x < screenMiddleX;
        //Debug.Log("initial rod is left? " + isLeft);
        if (isLeft)
        {
            spriteRenderer.sprite = leftRodSprite;
            transform.GetChild(0).gameObject.SetActive(true);
            lineConnector.transform.localPosition = leftLineStartPosition;
        }
        else
        {
            spriteRenderer.sprite = rightRodSprite;
            transform.GetChild(0).gameObject.SetActive(false);
            lineConnector.transform.localPosition = rightLineStartPosition;
        }
    }

    void Update()
    {
        ScrollWheelReel();
        RodFollowCursorX();
        CheckToFlipRod();
        PullFishLaterally();
        CheckForFishHeight();



        if (Scr_SpawnToiletFish.shouldSpawn)
        {
            SpawnToiletFish();
        }
    }

    private void LateUpdate()
    {
        if (toiletFish == null) return;

        Vector3 cameraPos = mainCamera.transform.position;

        // Only follow the Y of the fish
        //cameraPos.x = Mathf.Lerp(cameraPos.x, toiletFish.transform.position.x, Time.deltaTime * cameraSmooth);
        //cameraPos.y = Mathf.Lerp(cameraPos.y, toiletFish.transform.position.y, Time.deltaTime * cameraSmooth);
        cameraPos.x = toiletFish.transform.position.x;
        cameraPos.y = toiletFish.transform.position.y;

        mainCamera.transform.position = cameraPos;
    }
    public void ChangeGameSettings()
    {
        Scr_GameSettings settings = Scr_GameManager.ActiveSettings;
        reelInSpeed = settings.toiletFishReelInSpeed;
        lateralPullStrength = settings.toiletFishLateralPullStrength;
    }

    private void CheckForFishHeight()
    {
        if (!toiletFish)
            return;

        EdgeCollider2D fishEdgeCollider = toiletFish.GetComponent<EdgeCollider2D>();
        float mouthToFinLength = Vector2.Distance(fishEdgeCollider.points[0], fishEdgeCollider.points[1]);
        if (lineConnector.pointB.position.y >= toiletWinLine.position.y + mouthToFinLength * toiletFish.transform.localScale.x)
        {
            //Debug.Log("FISH CAUGHT");
            FishCaught();
        }

        if (lineConnector.pointB.position.y <= fishEscapeHeight)
        {
            FishEscapeNoSound();
        }
    }

    private void FishCaught()
    {
        caughtToiletFish = toiletFish;

        // Enabling fishpedia buttons upon catching a fish
        foreach (var prefab in gameManager.fishPrefabs)
        {
            if (prefab.CompareTag(caughtToiletFish.tag))
            {
                prefab.GetComponent<Scr_Fish>().numberCaught++;
                int buttonIndex = 0;
                foreach (Button button in fishpedia.buttons)
                {
                    if (caughtToiletFish.name.Contains(button.name))
                    {
                        fishpedia.EnableEntryButton(buttonIndex);
                        fishpedia.SetImages(buttonIndex, prefab.GetComponent<Scr_FishAnimation>().sideSprite, prefab.GetComponent<Scr_FishAnimation>().frontSprite);
                        fishpedia.SetName(buttonIndex, prefab.name);
                        fishpedia.SetDescription(buttonIndex, prefab.GetComponent<Scr_Fish>().fishDescription);
                        fishpedia.SetStat(buttonIndex, 0, prefab.GetComponent<Scr_Fish>().baseFishCost);
                        fishpedia.SetStat(buttonIndex, 2, prefab.GetComponent<Scr_Fish>().minutesUntilHungry);
                        fishpedia.SetStat(buttonIndex, 4, prefab.GetComponent<Scr_ToiletFish>().difficultyMultiplier);
                        fishpedia.SetStat(buttonIndex, 5, prefab.GetComponent<Scr_Fish>().numberCaught);
                    }
                    buttonIndex++;
                }
            }
        }


        toiletFish.GetComponent<Scr_ToiletFish>().enabled = false;
        if (isReeling)
        {
            isReeling = false;
            ReelingAudioSource.Stop();
        }
        toiletFish.transform.localScale = originalFishScale;
        gameManager.BagToiletFish(toiletFish);

        //Scr_SpawnToiletFish.toiletFishExist = false;
        toiletFish = null;

        Invoke("GoToBathroom", 1f);

        Invoke("DestroyPipes", 1f);

        //FishDestroy();
    }
    public void FishEscape()
    {
        //Debug.Log("Fish has escaped");

        toiletFish.GetComponent<Scr_ToiletFish>().escapeFactor = 20;

        gameManager.PlaySoundEffect(gameManager.SFX_FishHitToilet, 1f);
        gameManager.PlaySoundEffect(gameManager.SFX_ToiletSplash, 0.3f);
        gameManager.PlaySoundEffect(gameManager.SFX_Flush, 0.3f);

        if (isReeling)
        {
            isReeling = false;
            ReelingAudioSource.Stop();
        }
        //lineConnector.line.enabled = false;

        //Scr_SpawnToiletFish.toiletFishExist = false;

        toiletFishToDestroy = toiletFish; //so that fish can still exist and drop into the toilet before being set to null and destroying in FishDestroy
        toiletFish = null;

        Invoke("GoToBathroom", 1f);

        Invoke("FishDestroy", 1f);

        Invoke("DestroyPipes", 1f);


    }

    public void FishEscapeNoSound()
    {
        Debug.Log("Fish too low! It has escaped!");

        toiletFish.GetComponent<Scr_ToiletFish>().escapeFactor = 20;

        gameManager.PlaySoundEffect(gameManager.SFX_Flush, 0.3f);

        if (isReeling)
        {
            isReeling = false;
            ReelingAudioSource.Stop();
        }
        //lineConnector.line.enabled = false;

        //Scr_SpawnToiletFish.toiletFishExist = false;
        toiletFish = null;

        Invoke("GoToBathroom", 1f);

        Invoke("FishDestroy", 1f);

        Invoke("DestroyPipes", 1f);
    }

    public void GoToBathroom()
    {
        Scr_SpawnToiletFish.toiletFishExist = false;
        goToBathroom.onClick.Invoke();
    }

    public void FishDestroy()
    {
        //Scr_SpawnToiletFish.toiletFishExist = false;

        Destroy(toiletFishToDestroy);

        toiletFishToDestroy = null;


    }

    private void SpawnToiletFish()
    {
        toiletFish = Instantiate(GetRandomFish(), fishSpawnPosition, Quaternion.Euler(0f, 0f, 90f));
        originalFishScale = toiletFish.transform.localScale;
        toiletFish.transform.localScale *= scaleReductionWhenFishing;

        Scr_Fish toiletFishScr = toiletFish.GetComponent<Scr_Fish>();
        Scr_FishAnimation toiletFishAnimScr = toiletFish.GetComponent<Scr_FishAnimation>();

        toiletFishScr.thisPrefab = FindMatchingPrefabGivenTag(toiletFish.tag);

        toiletFishAnimScr.SetState(Scr_FishAnimation.FishState.Move);
        toiletFishScr.enabled = false;
        toiletFish.GetComponent<Scr_ToiletFish>().enabled = true;

        toiletFish.GetComponentInChildren<Animator>().speed = 5;

        lineConnector.pointB = FindMouthInFishChildren(toiletFish.transform, "Mouth Position");
        if (lineConnector.pointB == null)
        {
            Debug.LogWarning("Fish's 'Mouth Position' Not Found!");
        }

        Scr_SpawnToiletFish.toiletFishExist = true;
        Scr_SpawnToiletFish.shouldSpawn = false;

    }

    private GameObject FindMatchingPrefabGivenTag(string fishTag)
    {
        //GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Fish");

        foreach (var prefab in gameManager.fishPrefabs)
        {
            if (prefab.CompareTag(fishTag))
            {
                return prefab;
            }
        }

        return null;
    }

    private void PullFishLaterally()
    {
        if (!toiletFish)
            return;

        // Pull fish horizontally based on pole offset from center
        float poleOffset = lineConnector.transform.position.x - toiletFish.transform.position.x;

        float pullAmount = poleOffset * lateralPullStrength * Time.deltaTime;
        toiletFish.transform.position += new Vector3(pullAmount, 0f, 0f);
    }
    private void ScrollWheelReel()
    {
        if (!toiletFish)
            return;

        //scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetMouseButton(0))
            scrollInput = 0.01f;
        else if (Input.GetMouseButton(1))
            scrollInput = -0.01f;
        else
            scrollInput = 0;


        if (Mathf.Abs(scrollInput) > scrollInputThreshold)
        {
            // Rotate around the Z-axis (adjust axis if needed)
            transform.GetChild(0).Rotate(0, 0, scrollInput * rotationSpeed * 10000 * Time.deltaTime);

            // If scrolling, reset the timer and ensure the sound plays
            scrollTimer = scrollTimeout; // Reset the timer

            if (!isReeling)
            {
                isReeling = true;
                ReelingAudioSource.loop = true;  // Ensure the sound loops
                ReelingAudioSource.Play();
            }
        }
        else if (scrollTimer > 0f)
        {
            // If no input but there's still time left on the scrollTimer, continue the sound
            scrollTimer -= Time.deltaTime;
        }
        else
        {
            // Stop the reeling sound when there's no scroll input
            if (isReeling)
            {
                isReeling = false;
                ReelingAudioSource.Stop();
            }
        }

        toiletFish.transform.Translate(scrollInput, 0, 0);
        if (scrollInput > 0) // Reeling out
        {
            ReelingAudioSource.pitch = 1f;
        }
        else if (scrollInput < 0) // Reeling in
        {
            ReelingAudioSource.pitch = 1.3f;

        }
    }

    private void RodFollowCursorX()
    {
        Vector3 mousePosition = Input.mousePosition; // Get cursor position in screen space
        mousePosition.z = Camera.main.nearClipPlane; // Ensure it's in the correct depth for conversion

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert to world space

        // Find world Y of a FIXED screen height (rod stays visually in same spot)
        Vector3 fixedScreenY = new Vector3(Screen.width * 0.5f, Screen.height * rodScreenHeight, mousePosition.z);

        float fixedWorldY = Camera.main.ScreenToWorldPoint(fixedScreenY).y;

        // Apply the new position
        transform.position = new Vector3(worldPosition.x, fixedWorldY, transform.position.z);
    }
    private void CheckToFlipRod()
    {
        float mouseX = Input.mousePosition.x; // Get cursor X position

        if (mouseX < screenMiddleX && !isLeft)
        {
            spriteRenderer.sprite = leftRodSprite; // Use left rod when mouse is on the left
            isLeft = true;
            transform.GetChild(0).gameObject.SetActive(true);
            // Move line point to the left rod position
            lineConnector.transform.localPosition = leftLineStartPosition;

        }
        else if (mouseX >= screenMiddleX && isLeft)
        {
            spriteRenderer.sprite = rightRodSprite; // Use right rod when mouse is on the right
            isLeft = false;
            transform.GetChild(0).gameObject.SetActive(false);
            // Move line point to the right rod position
            lineConnector.transform.localPosition = rightLineStartPosition;
        }
    }
    public GameObject GetRandomFish()
    {
        if (fishesToCatch == null || weightedChanceToCatch == null || fishesToCatch.Count != weightedChanceToCatch.Count || fishesToCatch.Count == 0)
        {
            Debug.LogWarning("Fishing table not set up correctly.");
            return null;
        }

        int totalWeight = 0;
        foreach (int weight in weightedChanceToCatch)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        for (int i = 0; i < weightedChanceToCatch.Count; i++)
        {
            currentSum += weightedChanceToCatch[i];
            if (randomValue < currentSum)
            {
                return fishesToCatch[i];
            }
        }

        return null; // shouldn't happen if weights > 0
    }
    public Transform FindMouthInFishChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindMouthInFishChildren(child, name);
            if (result != null)
                return result;
        }
        return null;
    }



    public void SpawnPipes(int _count)
    {
        Transform currentAttachPoint = pipeSpawnPoint; // drag your toilet's end point in inspector

        for (int i = 0; i < _count; i++)
        {
            GameObject prefab = pipePrefabs[Random.Range(0, pipePrefabs.Length)];
            GameObject pipe = Instantiate(prefab);
            pipe.transform.parent = transform.parent;

            // Position the pipe so its StartPoint aligns with currentAttachPoint
            Transform start = pipe.transform.Find("Start Point");
            Vector3 offset = start.position - pipe.transform.position;
            pipe.transform.position = currentAttachPoint.position - offset;

            // Update attach point for next pipe
            currentAttachPoint = pipe.transform.Find("End Point");

            // Assign the fishing rod
            pipe.GetComponent<Scr_ToiletCollision>().fishingRod = this;

            // Set the bottom of the last pipe to be the Fish Escape Height and the Fish Spawn Position
            if (i == _count - 1)
            {
                fishEscapeHeight = currentAttachPoint.position.y - 3;

                fishSpawnPosition = currentAttachPoint.position;
            }
        }
    }

    public void DestroyPipes()
    {
        foreach (Transform child in transform.parent)
        {
            if (child.name.Contains("Pipe") && child.name.Contains("Clone"))
            {
                Destroy(child.gameObject);
            }
        }
    }

}
