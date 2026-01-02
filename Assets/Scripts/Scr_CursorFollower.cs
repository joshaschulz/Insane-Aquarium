using UnityEngine;

public class Scr_CursorFollower : MonoBehaviour
{
    [Header("world vs screen")]
    public bool followInWorldSpace = true;
    public float zDistanceFromCamera = 10f;

    [Header("lock y")]
    public bool lockY = false;
    public float lockedYValue = 0f;

    [Header("lock x")]
    public bool lockX = false;
    public float lockedXValue = 0f;

    [Header("dynamic side clamp (lock x to camera edge)")]
    public bool clampToCameraSide = false;
    public float offsetFromCameraSide = 0.5f;
    public bool flipOnSideSwitch = true;

    private bool isOnRightSide = false;
    private Vector3 originalLocalScale;

    public bool IsOnRightSide => isOnRightSide;


    private void Awake()
    {
        originalLocalScale = transform.localScale;

        if (clampToCameraSide)
        {
            InitializeSideFromCursor();
        }
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (!followInWorldSpace)
        {
            transform.position = mousePos;
            return;
        }

        // get world point under cursor (on z=0 plane)
        mousePos.z = zDistanceFromCamera;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // dynamic side clamp
        if (clampToCameraSide)
        {
            float viewportX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
            bool nowRight = viewportX >= 0.5f;

            // compute left/right edge x in world space (at same depth)
            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, zDistanceFromCamera));
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, zDistanceFromCamera));

            float clampedX = nowRight ? (rightEdge.x - offsetFromCameraSide) : (leftEdge.x + offsetFromCameraSide);
            worldPos.x = clampedX;

            // optional flip
            if (flipOnSideSwitch && nowRight != isOnRightSide)
            {
                isOnRightSide = nowRight;

                Vector3 s = originalLocalScale;
                s.x = Mathf.Abs(s.x) * (isOnRightSide ? 1f : -1f);
                transform.localScale = s;
            }
        }
        else
        {
            // fixed locks
            if (lockX) worldPos.x = lockedXValue;
        }

        if (lockY) worldPos.y = lockedYValue;

        transform.position = worldPos;
    }

    public void InitializeSideFromCursor()
    {
        float viewportX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        isOnRightSide = viewportX >= 0.5f;

        if (flipOnSideSwitch)
        {
            Vector3 s = originalLocalScale;
            s.x = Mathf.Abs(s.x) * (isOnRightSide ? 1f : -1f);
            transform.localScale = s;
        }
    }
}
