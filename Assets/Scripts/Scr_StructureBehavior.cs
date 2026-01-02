using UnityEngine;
using UnityEngine.UI;

public class Scr_StructureButton : MonoBehaviour
{
    [Header("assign in inspector")]
    public Scr_GameManager gameManager;
    public GameObject structurePrefab;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("[Scr_StructureButton] no Button component found on " + gameObject.name);
            return;
        }

        button.onClick.AddListener(OnClick);

        Debug.Log("[Scr_StructureButton] wired OnClick on " + gameObject.name);
    }

    private void OnClick()
    {
        Debug.Log("[Scr_StructureButton] clicked " + gameObject.name);

        if (gameManager == null)
        {
            Debug.LogError("[Scr_StructureButton] gameManager is NULL (assign it in inspector).");
            return;
        }

        if (structurePrefab == null)
        {
            Debug.LogError("[Scr_StructureButton] structurePrefab is NULL (assign it in inspector).");
            return;
        }

        gameManager.SelectStructureToPlace(structurePrefab, gameObject);
        Debug.Log("[Scr_StructureButton] called SelectStructureToPlace successfully.");
    }
}
