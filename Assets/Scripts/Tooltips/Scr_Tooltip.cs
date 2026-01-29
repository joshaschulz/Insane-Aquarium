using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scr_Tooltip : MonoBehaviour
{
    public static Scr_Tooltip Instance;

    public TextMeshPro text;
    public Vector3 tooltipOffset = new Vector3(0.05f, -0.05f, 0f);
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
        if (textRenderer == null) return;

        // 1) desired TOP-LEFT position from mouse (do the offset in SCREEN space)
        Vector3 mouse = Input.mousePosition;

        // convert tooltipOffset (world) -> better to use a screen offset:
        // if you already have tooltipOffset in world, keep it, but screen offset is more consistent.
        Vector3 desiredWorld = cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, cam.nearClipPlane));
        desiredWorld.z = 0f;

        // optional: keep your existing world offset if you like
        desiredWorld += tooltipOffset;

        // 2) place roughly first (bounds depend on position)
        transform.position = desiredWorld;

        // 3) compute current top-left of the tooltip in WORLD
        Bounds b = textRenderer.bounds;
        Vector3 currentTopLeft = new Vector3(b.min.x, b.max.y, 0f);

        // 4) shift so top-left sits exactly at the cursor anchor point
        Vector3 anchorDelta = desiredWorld - currentTopLeft;
        anchorDelta.z = 0f;
        transform.position += anchorDelta;

        // 5) clamp to screen (same idea as your code, but after anchoring)
        b = textRenderer.bounds;

        Vector3 min = cam.WorldToScreenPoint(b.min);
        Vector3 max = cam.WorldToScreenPoint(b.max);

        float left = Mathf.Min(min.x, max.x);
        float right = Mathf.Max(min.x, max.x);
        float bottom = Mathf.Min(min.y, max.y);
        float top = Mathf.Max(min.y, max.y);

        float dx = 0f;
        float dy = 0f;

        if (left < screenPaddingPixels) dx = screenPaddingPixels - left;
        else if (right > Screen.width - screenPaddingPixels) dx = (Screen.width - screenPaddingPixels) - right;

        if (bottom < screenPaddingPixels) dy = screenPaddingPixels - bottom;
        else if (top > Screen.height - screenPaddingPixels) dy = (Screen.height - screenPaddingPixels) - top;

        if (dx != 0f || dy != 0f)
        {
            Vector3 worldA = cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, cam.nearClipPlane));
            Vector3 worldB = cam.ScreenToWorldPoint(new Vector3(mouse.x + dx, mouse.y + dy, cam.nearClipPlane));

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