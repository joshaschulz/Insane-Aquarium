using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scr_Tooltip : MonoBehaviour
{
    public static Scr_Tooltip Instance;

    public TextMeshPro text;
    public Vector3 tooltipOffset = new Vector3(0.3f, -0.3f, 0f);
    public float screenPaddingPixels = 10f;

    public SpriteRenderer backgroundRenderer;
    public Vector2 backgroundPadding = new Vector2(0.15f, 0.1f); // world units
    public Vector3 backgroundLocalOffset = new Vector3(0f, 0f, 0.01f);

    private Camera cam;
    private Renderer textRenderer;


    private void Awake()
    {
        cam = Camera.main;

        if (text != null)
            textRenderer = text.GetComponent<Renderer>();

        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }


    private void LateUpdate()
    {
        if (!gameObject.activeSelf || cam == null) return;

        // 1) desired position from mouse
        Vector3 screenPos = Input.mousePosition;
        Vector3 desiredWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane));
        desiredWorld.z = 0f;

        Vector3 desired = desiredWorld + tooltipOffset;
        transform.position = desired;

        // 2) if no renderer, nothing to clamp
        if (textRenderer == null) return;

        // 3) compute tooltip bounds in screen pixels (AABB)
        Bounds b = textRenderer.bounds;

        Vector3 min = cam.WorldToScreenPoint(b.min);
        Vector3 max = cam.WorldToScreenPoint(b.max);

        float left = Mathf.Min(min.x, max.x);
        float right = Mathf.Max(min.x, max.x);
        float bottom = Mathf.Min(min.y, max.y);
        float top = Mathf.Max(min.y, max.y);

        // 4) shift screenPos so bounds stay inside screen (with padding)
        float dx = 0f;
        float dy = 0f;

        if (left < screenPaddingPixels) dx = screenPaddingPixels - left;
        else if (right > Screen.width - screenPaddingPixels) dx = (Screen.width - screenPaddingPixels) - right;

        if (bottom < screenPaddingPixels) dy = screenPaddingPixels - bottom;
        else if (top > Screen.height - screenPaddingPixels) dy = (Screen.height - screenPaddingPixels) - top;

        if (dx != 0f || dy != 0f)
        {
            // convert that screen delta into world delta at the tooltip depth
            Vector3 worldA = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane));
            Vector3 worldB = cam.ScreenToWorldPoint(new Vector3(screenPos.x + dx, screenPos.y + dy, cam.nearClipPlane));

            Vector3 worldDelta = worldB - worldA;
            worldDelta.z = 0f;

            transform.position += worldDelta;
        }
    }

    public void Show(string message)
    {
        text.text = message;
        gameObject.SetActive(true);

        ResizeBackgroundToText();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ResizeBackgroundToText()
    {
        if (backgroundRenderer == null || text == null) return;

        // ensure text bounds are correct this frame
        text.ForceMeshUpdate();

        // bounds are in LOCAL space for TextMeshPro
        Bounds tb = text.bounds;

        // size the sprite to text size + padding
        backgroundRenderer.size = new Vector2(
            tb.size.x + backgroundPadding.x,
            tb.size.y + backgroundPadding.y
        );

        // center background on text
        backgroundRenderer.transform.localPosition = tb.center + backgroundLocalOffset;
    }
}