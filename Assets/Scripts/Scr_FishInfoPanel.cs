using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishInfoPanel : MonoBehaviour
{
    public TMPro.TMP_Text nameText;

    public GameObject starfishDiagram;

    private GameObject[] appealLegs;
    private GameObject[] freakLegs;
    private GameObject[] hungerLegs;
    private GameObject[] poopLegs;
    private GameObject[] priceLegs;

    public TMPro.TMP_Text[] legsNum;

    [Header("Follow Settings")]
    public Vector3 worldOffset = new Vector3(0f, -2f, 0f); // below fish in WORLD units
    public float screenPadding = 10f; // padding from screen edges

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

        GetLegs();

    }

    void LateUpdate()
    {

    }

    public void Follow()
    {
        if (currentTarget == null || cam == null || rectTransform == null) return;

        // if this panel lives under a world-space canvas, we need its RectTransform
        RectTransform canvasRect = parentCanvas != null ? parentCanvas.GetComponent<RectTransform>() : null;
        if (canvasRect == null) return;

        // the camera that renders the UI (for World Space canvas this matters)
        Camera uiCam = parentCanvas.worldCamera != null ? parentCanvas.worldCamera : cam;

        // 1) world position (fish + offset)
        Vector3 worldPos = currentTarget.position + worldOffset;

        // 2) world -> screen
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // if fish is behind the camera, optionally hide the panel
        if (screenPos.z < 0f)
        {
            // gameObject.SetActive(false); // optional
            return;
        }
        // else gameObject.SetActive(true); // optional

        // 3) compute panel size in screen pixels (same idea as before)
        float scaleFactor = parentCanvas != null ? parentCanvas.scaleFactor : 1f;
        float panelWidth = rectTransform.rect.width * scaleFactor;
        float panelHeight = rectTransform.rect.height * scaleFactor;

        float halfW = panelWidth * 0.5f;
        float halfH = panelHeight * 0.5f;

        // 4) clamp so the whole panel stays on screen (still in screen pixels)
        float minX = halfW + screenPadding;
        float maxX = Screen.width - halfW - screenPadding;
        float minY = halfH + screenPadding;
        float maxY = Screen.height - halfH - screenPadding;

        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        // 5) screen -> world ON the canvas plane, then apply
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect,
                screenPos,
                uiCam,
                out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint;
        }
    }

    public void GetLegs()
    {
        if (starfishDiagram == null)
        {
            Debug.LogError("starfishDiagram is not assigned.");
            return;
        }

        // helper local function to fetch children as an array
        GameObject[] GetChildren(Transform parent)
        {
            GameObject[] children = new GameObject[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                children[i] = parent.GetChild(i).gameObject;
            }
            return children;
        }

        // iterate through the first-level children under starfishDiagram
        foreach (Transform category in starfishDiagram.transform)
        {
            switch (category.name)
            {
                case "Appeal":
                    appealLegs = GetChildren(category);
                    break;

                case "Freak":
                    freakLegs = GetChildren(category);
                    break;

                case "Hunger":
                    hungerLegs = GetChildren(category);
                    break;

                case "Poop":
                    poopLegs = GetChildren(category);
                    break;

                case "Price":
                    priceLegs = GetChildren(category);
                    break;
            }
        }
    }

    public void Show(Scr_Fish fish)
    {
        gameObject.SetActive(true);

        //currentTarget = fish.transform;

        nameText.text = fish.name;

        SetLegs(appealLegs, fish.appeal);
        SetLegs(freakLegs, fish.freakuency);
        SetLegs(hungerLegs, fish.hungerCapacity);
        SetLegs(poopLegs, fish.poopInterval);
        SetLegs(priceLegs, fish.priceModifier);

        SetLegsNum(legsNum[0], fish.appeal);
        SetLegsNum(legsNum[1], fish.freakuency);
        SetLegsNum(legsNum[2], fish.hungerCapacity);
        SetLegsNum(legsNum[3], fish.poopInterval);
        SetLegsNum(legsNum[4], fish.priceModifier);

        currentTarget = fish.transform;


        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalRectHeight);

    }

    public void ShowExotic(Scr_Starfish fish)
    {
        gameObject.SetActive(true);

        currentTarget = fish.transform;

        nameText.text = fish.name;

        starfishDiagram.SetActive(false);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);


    }


    void SetLegs(GameObject[] legs, int value)
    {
        int highestActiveOrder = 3;
        int distAway = legs.Length - 1 - highestActiveOrder;
        //int lowestActiveOrder = 0;


        for (int i = legs.Length - 2; i >= 0; i--)
        {
            SpriteRenderer sr = legs[i].GetComponent<SpriteRenderer>();
            Color c = sr.color;

            if (i < value)
            {
                c.r = 1f;
                c.g = 1f;
                c.b = 1f;
                //legs[i].SetActive(true);

                sr.sortingOrder = highestActiveOrder--;


            }
            else
            {
                c.r = 0.5f;
                c.g = 0.5f;
                c.b = 0.5f;
                //legs[i].SetActive(false);

                sr.sortingOrder = i - value - distAway;

            }

            sr.color = c;
        }
    }

    void SetLegsNum(TMPro.TMP_Text text, int val)
    {
        text.text = val.ToString();

        Transform textPos;
        GameObject[] legsToUse = null;

        switch (text.name)
        {
            case "Appeal Num":
                legsToUse = appealLegs;
                break;

            case "Freak Num":
                legsToUse = freakLegs;
                break;

            case "Hunger Num":
                legsToUse = hungerLegs;
                break;

            case "Poop Num":
                legsToUse = poopLegs;
                break;

            case "Price Num":
                legsToUse = priceLegs;
                break;
        }



        textPos = legsToUse[val].transform;

        text.transform.position = textPos.position;
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
