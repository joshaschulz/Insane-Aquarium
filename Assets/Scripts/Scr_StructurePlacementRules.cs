using UnityEngine;
using UnityEngine.UI;

public class Scr_StructurePlacementRules : MonoBehaviour
{

    public Button structureButton;
    public GameObject thisPrefab;

    public bool placedOnRight;

    public enum AnchorMode
    {
        Free,
        LockToCameraBottom,
        LockToCameraSide, // clamp X to left/right edge, follow Y
        FreeAboveY
    }

    public int cost;

    [Header("Placement mode")]
    public AnchorMode anchorMode = AnchorMode.Free;

    [Header("Bottom anchor")]
    [Tooltip("World units ABOVE the bottom of the camera view")]
    public float offsetFromCameraBottom = 0.5f;

    [Header("Side anchor")]
    [Tooltip("World units IN from the left/right edge of the camera view")]
    public float offsetFromCameraSide = 0.5f;
    public bool flipOnSideSwitch = true;
}
