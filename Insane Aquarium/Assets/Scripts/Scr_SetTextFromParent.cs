using UnityEngine;
using TMPro;


[ExecuteInEditMode]
public class Scr_SetTextFromParent : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private string lastParentName;

    private void Update()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        if (textComponent != null && transform.parent != null)
        {
            string currentParentName = transform.parent.name;

            if (currentParentName != lastParentName)
            {
                textComponent.text = transform.parent.name;
                lastParentName = currentParentName;
            }
        }
    }
}
