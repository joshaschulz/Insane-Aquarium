using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Fishing : MonoBehaviour
{
    private Scr_GameManager gameManager;
    AudioSource ReelingAudioSource;
    private bool isReeling = false; // Flag to track if the player is reeling
    private float scrollInputThreshold = 0.005f; // Minimum threshold for scroll input
    private float scrollTimeout = 0.4f; // Time to wait after stopping scroll input before stopping sound
    private float scrollTimer = 0f; // Timer to track scroll inactivity



    public Sprite leftRodSprite, rightRodSprite;
    public float rotationSpeed;

    private SpriteRenderer spriteRenderer;
    private float screenMiddleX;
    private bool isLeft; // Track the current state to avoid unnecessary updates

    public Transform fishSpawnPosition;
    private Scr_LineConnector lineConnector;

    [SerializeField] private List<GameObject> fishesToCatch;
    [SerializeField] private List<int> weightedChanceToCatch;

    [SerializeField] private Vector3 leftLineStartPosition, rightLineStartPosition;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ReelingAudioSource = GetComponent<AudioSource>();
        ReelingAudioSource.clip = gameManager.SFX_Reeling;
        screenMiddleX = Screen.width / 2; // Get middle X point of the screen


        lineConnector = transform.GetChild(1).GetComponent<Scr_LineConnector>();

        // Initialize sprite based on starting position
        isLeft = Input.mousePosition.x < screenMiddleX;
        Debug.Log("initial rod is left? " + isLeft);
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

        ToiletFishMovement();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject toiletFish = Instantiate(GetRandomFish(), fishSpawnPosition);
            Scr_Fish toiletFishScr = toiletFish.GetComponent<Scr_Fish>();

            toiletFishScr.FaceSideways();
            toiletFishScr.enabled = false;
            toiletFish.GetComponent<Scr_ToiletFish>().enabled = true;

            toiletFish.GetComponentInChildren<Animator>().speed = 5;

            lineConnector.fishCaught = true;
            lineConnector.pointB = FindMouthInFishChildren(toiletFish.transform, "Mouth Position");
            if (lineConnector.pointB == null)
            {
                Debug.LogWarning("Fish's 'Mouth Position' Not Found!");
            }
        }
    }

    private void ToiletFishMovement()
    {

    }
    private void ScrollWheelReel()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

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

        if (scrollInput > 0)
            ReelingAudioSource.pitch = 1f;
        else if (scrollInput < 0)
            ReelingAudioSource.pitch = 1.3f;
    }


    private void RodFollowCursorX()
    {
        Vector3 mousePosition = Input.mousePosition; // Get cursor position in screen space
        mousePosition.z = Camera.main.nearClipPlane; // Ensure it's in the correct depth for conversion

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert to world space

        // Update only the X position, keep Y and Z unchanged
        transform.position = new Vector3(worldPosition.x, transform.position.y, transform.position.z);
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
}
