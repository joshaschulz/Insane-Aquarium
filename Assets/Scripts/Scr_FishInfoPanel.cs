using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishInfoPanel : MonoBehaviour
{
    public TMPro.TMP_Text nameText;

    public UnityEngine.UI.Image[] priceStars;
    public UnityEngine.UI.Image[] appealStars;
    public UnityEngine.UI.Image[] hungerCapacityStars;
    public UnityEngine.UI.Image[] freakuencyStars;
    public UnityEngine.UI.Image[] poopIntervalStars;

    [Header("Follow Settings")]
    public Vector3 worldOffset = new Vector3(0f, -2f, 0f); // below fish in WORLD units
    public float screenPadding = 10f; // padding from screen edges

    public GameObject priceStarsObj;
    public GameObject appealStarsObj;
    public GameObject hungerStarsObj;
    public GameObject freakStarsObj;
    public GameObject poopStarsObj;

    public Transform currentTarget;
    private Camera cam;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private float originalRectHeight;

    //private Scr_Fish currentFish;
    //public Scr_Fish CurrentFish => currentFish; // read-only property


    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        originalRectHeight = rectTransform.rect.height;
    }

    void LateUpdate()
    {
        if (currentTarget == null || cam == null) return;

        // 1) world position (fish + offset)
        Vector3 worldPos = currentTarget.position + worldOffset;

        // 2) world → screen
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // 3) compute panel size in screen pixels
        float scaleFactor = parentCanvas != null ? parentCanvas.scaleFactor : 1f;
        float panelWidth = rectTransform.rect.width * scaleFactor;
        float panelHeight = rectTransform.rect.height * scaleFactor;

        float halfW = panelWidth * 0.5f;
        float halfH = panelHeight * 0.5f;

        // 4) clamp so the whole panel stays on screen
        float minX = halfW + screenPadding;
        float maxX = Screen.width - halfW - screenPadding;
        float minY = halfH + screenPadding;
        float maxY = Screen.height - halfH - screenPadding;

        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        // 5) apply to UI
        rectTransform.position = screenPos;
    }

    public void Show(Scr_Fish fish)
    {
        gameObject.SetActive(true);

        nameText.text = fish.name;

        priceStarsObj.SetActive(true);
        appealStarsObj.SetActive(true);
        hungerStarsObj.SetActive(true);
        freakStarsObj.SetActive(true);
        poopStarsObj.SetActive(true);

        SetStars(priceStars, fish.priceModifier);
        SetStars(appealStars, fish.appeal);
        SetStars(hungerCapacityStars, fish.hungerCapacity);
        SetStars(freakuencyStars, fish.freakuency);
        SetStars(poopIntervalStars, fish.poopInterval);

        currentTarget = fish.transform;


        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalRectHeight);

    }

    public void ShowExotic(Scr_Starfish fish)
    {
        gameObject.SetActive(true);

        currentTarget = fish.transform;

        nameText.text = fish.name;

        priceStarsObj.SetActive(false);
        appealStarsObj.SetActive(false);
        hungerStarsObj.SetActive(false);
        freakStarsObj.SetActive(false);
        poopStarsObj.SetActive(false);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);


    }


    void SetStars(UnityEngine.UI.Image[] stars, int value)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i < value;
        }
    }

    public void Hide()
    {
        currentTarget = null;

        gameObject.SetActive(false);
    }

    public void HideIfFish(Scr_Fish fish)
    {
        if (currentTarget == fish.transform)
            Hide();
    }
}
