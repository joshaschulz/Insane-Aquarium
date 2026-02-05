using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Notifications : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public GameObject container;
    public TextMeshProUGUI text;

    private int autoHideSecondsDefault = 5;


    public GameObject tinyBox;
    public GameObject smallBox;
    public GameObject normalBox;
    public GameObject largeBox;
    public GameObject hugeBox;

    private List<GameObject> boxes;

    //for resizing the background
    public Vector2 backgroundPadding; // pixels
    private RectTransform textRect;

    private Vector3 containerOriginalScale;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        text.text = string.Empty;

        boxes = new List<GameObject> { tinyBox, smallBox, normalBox, largeBox, hugeBox };

        containerOriginalScale = container.transform.localScale;

        textRect = text.GetComponent<RectTransform>();
    }

    public void ShowDelayed(string message, float delay, bool isWarning)
    {
        StartCoroutine(ShowDelayedRoutine(message, delay, isWarning));
    }

    private IEnumerator ShowDelayedRoutine(string message, float delay, bool isWarning)
    {
        yield return new WaitForSeconds(delay);
        Show(message, isWarning);
    }


    public void Show(string message, bool isWarning)
    {
        Show(message, autoHideSecondsDefault, isWarning);
    }

    public void Show(string message, float hideSeconds, bool isWarning)
    {
        if (container.activeSelf)
        {
            StopAllCoroutines();
            HideQuick();
        }

        if (isWarning)
            gameManager.PlaySoundEffect(gameManager.SFX_warningNotif, 0.05f);
        else
            gameManager.PlaySoundEffect(gameManager.SFX_notif, 0.05f, 1.2f);

        container.SetActive(true);
        text.text = message;

        ResizeBackgroundToText();

        StartCoroutine(OpenBox(container));

        StartCoroutine(AutoHideAfterDelay(hideSeconds));
    }

    public void Hide()
    {
        StartCoroutine(CloseBox(container));
    }

    public void HideQuick()
    {
        text.text = string.Empty;
        container.transform.localScale = containerOriginalScale;
        container.SetActive(false);
    }

    public IEnumerator OpenBox(GameObject box)
    {
        RectTransform rect = box.GetComponent<RectTransform>();
        Vector3 targetScale = rect.localScale;
        rect.localScale = Vector3.zero;
        float duration = 0.15f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rect.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        rect.localScale = targetScale;
    }

    public IEnumerator CloseBox(GameObject box)
    {
        RectTransform rect = box.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        Vector3 targetScale = Vector3.zero;
        float duration = 0.15f; // how fast it closes
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        rect.localScale = Vector3.zero;
        text.text = string.Empty;
        box.SetActive(false);

        rect.localScale = originalScale; // reset for next time
    }

    private void ResizeBackgroundToText()
    {
        if (text == null) return;

        // ensure TMP is updated so rendered size is correct
        text.ForceMeshUpdate();

        // 1) actual rendered size in PIXELS
        Vector2 textPx = text.GetRenderedValues(false);
        float neededW = textPx.x + backgroundPadding.x;
        float neededH = textPx.y + backgroundPadding.y;

        bool found = false;

        for (int i = 0; i < boxes.Count; i++)
        {
            GameObject boxObj = boxes[i];
            if (boxObj == null) continue;

            var img = boxObj.GetComponent<Image>();
            var rt = boxObj.GetComponent<RectTransform>();
            if (img == null || rt == null || img.sprite == null) continue;

            // 2) sprite size in PIXELS (from the sprite rect)
            Vector2 spritePx = img.sprite.rect.size;
            float boxW = spritePx.x * boxObj.transform.localScale.x;
            float boxH = spritePx.y * boxObj.transform.localScale.y;

            if (!found && boxH >= neededH && boxW >= neededW) // pick by height first (your original goal)
            {
                found = true;

                boxObj.SetActive(true);

                // align: center behind the text (UI local space)
                rt.anchoredPosition = text.rectTransform.anchoredPosition;
            }
            else
            {
                boxObj.SetActive(false);
            }
        }
    }

    private IEnumerator AutoHideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}
