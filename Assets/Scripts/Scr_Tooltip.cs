using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scr_Tooltip : MonoBehaviour
{
    public static Scr_Tooltip Instance;

    public TextMeshProUGUI text;
    public Vector2 offset = new Vector2(20, -20);

    private RectTransform rect;
    private Canvas canvas;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rect.localPosition = localPoint + offset;
    }

    public void Show(string message)
    {
        text.text = message;
        text.enabled = true;
        Debug.Log("SHOWING TOOLTIP: " + message);
    }

    public void Hide()
    {
        text.enabled = false;
        Debug.Log("HIDING TOOLTIP");
    }
}