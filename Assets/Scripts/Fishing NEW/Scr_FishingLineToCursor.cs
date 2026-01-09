using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Scr_FishingLineToCursor : MonoBehaviour
{
    public LineRenderer line;
    public Transform fishingHook;

    [Header("camera")]
    public Camera targetCamera; // leave null to use Camera.main

    [Header("anchor (viewport space)")]
    [Range(0f, 1f)]
    public float anchorViewportX = 0.5f;

    [Tooltip("0 = bottom of view, 1 = top of view. Use >1 to go above the screen.")]
    public float anchorViewportY = 1.15f;

    [Header("cursor lag")]
    public bool useCursorLag = true;

    [Tooltip("when lag is on: higher = snappier, lower = more drag. try 8–20.")]
    public float cursorFollowSpeed = 12f;
    public float baseCursorFollowSpeed;

    [Header("line smoothness")]
    [Tooltip("how many points the line uses. 40–80 is great; you chose 60.")]
    public int points = 60;

    [Header("line sag")]
    [Tooltip("base sag amount in world units (try 0.15–0.5 depending on camera scale)")]
    public float sagAmount = 0.35f;

    [Tooltip("if true, sag scales with line length (feels more natural)")]
    public bool sagScalesWithLength = true;

    [Tooltip("multiplier when sag scales with length (0.05–0.2 is typical)")]
    public float sagPerUnitLength = 0.10f;

    // add near the top with other fields
    private Vector3 currentTipWorld;

    private Camera cam;
    private Vector3 smoothedCursorWorld;
    private bool hasInit = false;

    private Vector3[] linePoints;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        cam = targetCamera != null ? targetCamera : Camera.main;

        baseCursorFollowSpeed = cursorFollowSpeed;

        AllocatePoints();

        if (line.material != null)
            line.material.renderQueue = 5000;
    }

    private void OnEnable()
    {
        cam = targetCamera != null ? targetCamera : Camera.main;
        hasInit = false;
        AllocatePoints();


    }

    private void OnValidate()
    {
        if (points < 2) points = 2;

        if (line != null)
            AllocatePoints();
    }

    private void AllocatePoints()
    {
        if (points < 2) points = 2;

        if (linePoints == null || linePoints.Length != points)
            linePoints = new Vector3[points];

        if (line != null)
            line.positionCount = points;
    }

    private void Update()
    {
        if (cam == null)
            cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        Vector3 anchorWorld = GetAnchorWorld(cam);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        anchorWorld.z = mouseWorld.z = 0f;

        Vector3 bottomWorld;

        if (!useCursorLag)
        {
            // snap directly to cursor
            bottomWorld = mouseWorld;

            // keep smoothed value in sync so toggling lag on doesn't "jump"
            smoothedCursorWorld = mouseWorld;
            hasInit = true;

        }
        else
        {
            // lag behind cursor
            if (!hasInit)
            {
                smoothedCursorWorld = mouseWorld;
                hasInit = true;
            }

            float k = cursorFollowSpeed <= 0f
                ? 1f
                : 1f - Mathf.Exp(-cursorFollowSpeed * Time.deltaTime);

            smoothedCursorWorld = Vector3.Lerp(smoothedCursorWorld, mouseWorld, k);
            bottomWorld = smoothedCursorWorld;
        }

        // determine sag
        float sag = sagAmount;
        if (sagScalesWithLength)
        {
            float length = Vector2.Distance(anchorWorld, bottomWorld);
            sag = sagAmount + (length * sagPerUnitLength);
        }

        // sample the curve with many points
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);

            Vector3 p = Vector3.Lerp(anchorWorld, bottomWorld, t);

            float sagFactor = Mathf.Sin(t * Mathf.PI); // 0..1..0
            p.y -= sagFactor * sag;

            linePoints[i] = p;
        }

        // lock endpoints
        linePoints[0] = anchorWorld;
        linePoints[points - 1] = bottomWorld;

        currentTipWorld = bottomWorld; // or smoothedCursorWorld if that's what you use

        line.SetPositions(linePoints);



        Vector3 hookDir = currentTipWorld - linePoints[points - 2];
        float hookAngle = Mathf.Atan2(hookDir.y, hookDir.x) * Mathf.Rad2Deg;

        Quaternion hookRot = Quaternion.Euler(0f, 0f, hookAngle + 90f);

        fishingHook.position = currentTipWorld;
        fishingHook.rotation = hookRot;
    }

    private Vector3 GetAnchorWorld(Camera c)
    {
        float z = Mathf.Abs(c.transform.position.z);
        Vector3 v = new Vector3(anchorViewportX, anchorViewportY, z);
        Vector3 world = c.ViewportToWorldPoint(v);
        world.z = 0f;
        return world;
    }

    // add this public getter anywhere inside the class
    public Vector2 GetTipWorldPosition()
    {
        return (Vector2)currentTipWorld;
    }
}
