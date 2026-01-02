using UnityEngine;
using UnityEngine.UI;

public class Scr_StructurePlacementRules : MonoBehaviour
{

    public Button structureButton;

    public enum AnchorMode
    {
        Free,
        LockToCameraBottom,
        LockToCameraSide // clamp X to left/right edge, follow Y
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

    [Tooltip("If true, object flips when switching sides (mirror on X scale)")]
    public bool flipOnSideSwitch = true;
}
