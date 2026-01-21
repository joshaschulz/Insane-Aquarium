using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Scr_FishingMinigamePanel : MonoBehaviour
{
    private Scr_GameManager gameManager;

    [Header("ui")]
    public GameObject panel;

    [Header("fish presentation (ui target)")]
    [Tooltip("RectTransform under a Screen Space - Camera canvas.")]
    public RectTransform fishPosition;

    [Tooltip("The camera that renders the fishing minigame (the small viewport camera).")]
    public Camera minigameCamera;

    [Tooltip("seconds it takes the fish to lerp into position")]
    public float fishLerpDuration = 0.6f;

    [Tooltip("if true, panel starts hidden")]
    public bool hidePanelOnAwake = true;

    private Coroutine moveRoutine;

    public Transform winChestLocation;
    public GameObject winChest;

    public GameObject lineConnectorRoot; // drag the line connector parent here
    public GameObject treasureQuantityText;
    public int treasureQuantity;

    public Button bagFishButton;
    public Button flushFishButton;

    public void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;

    }

    private void OnEnable()
    {

        if (hidePanelOnAwake && panel != null)
            panel.SetActive(false);
    }

    public void SetTreasureQuantity(int quantity)
    {
        treasureQuantity = quantity;
    }

    /// <summary>
    /// Call this when the fish is successfully caught.
    /// </summary>
    public void ShowWithFish(GameObject fish) //win
    {
        if (fish == null || fishPosition == null)
            return;

        if (panel != null)
            panel.SetActive(true);

        // hide the line connector
        if (lineConnectorRoot != null)
            lineConnectorRoot.SetActive(false);

        // freeze fish bar at green/full and stop it from draining
        Scr_FishingMinigameFishController fishController =
            GetComponent<Scr_FishingMinigameFishController>();
        if (fishController != null)
            fishController.ForceWinUIAndStop();



        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        SetSortingGroupLayer(fish.gameObject, "UI2", 5);

        Scr_FishMinigameMovement movement = fish.GetComponentInChildren<Scr_FishMinigameMovement>();

        movement.fishAnimation.SetState(Scr_FishAnimation.FishState.Idle);
        movement.CancelInvoke();
        movement.fishAnimation.frontAnimator.speed = 1;

        // turn off fish movement
        if (movement != null)
            movement.enabled = false;

        FishTeleportToPosition(fish.gameObject);

        Scr_FishingMinigameChestController chestController =
            GetComponent<Scr_FishingMinigameChestController>();

        // destroy any remaining chest still in the field + hide chest UI
        if (chestController != null)
            chestController.ClearActiveChestAndUI();

        // spawn win chest if any were collected
        if (chestController != null && chestController.chestCount > 0)
        {
            GameObject prefab = chestController.treasureChestPrefabs[
                Random.Range(0, chestController.treasureChestPrefabs.Length)
            ];

            winChest = Instantiate(
                prefab,
                new Vector3(winChestLocation.position.x, winChestLocation.position.y, 0f),
                Quaternion.identity
            );

            SetTreasureQuantity(Random.Range(50, 101));
            treasureQuantityText.GetComponent<TextMeshProUGUI>().text = "x" + treasureQuantity.ToString();
            gameManager.AddMoneyAmount(treasureQuantity);

            Animator anim = winChest.GetComponentInChildren<Animator>();

            anim.Play("Crate Opening");

            SetSortingGroupLayer(winChest, "UI2", 5);
        }
    }

    public void HidePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void FishTeleportToPosition(GameObject fish)
    {
        Vector3 targetWorldPos = fishPosition.transform.position;//GetWorldPointUnderMinigameCamera(fishPosition, minigameCamera);
        Quaternion targetRot = fishPosition.rotation;

        fish.transform.position = targetWorldPos;
        fish.transform.rotation = targetRot;
    }

    public static void SetSortingGroupLayer(GameObject obj, string sortingLayerName, int sortingOrder = 0)
    {
        if (obj == null) return;

        SortingGroup[] groups = obj.GetComponentsInChildren<SortingGroup>(true);

        foreach (SortingGroup group in groups)
        {
            group.sortingLayerName = sortingLayerName;
            group.sortingOrder = sortingOrder;
        }
    }

}
