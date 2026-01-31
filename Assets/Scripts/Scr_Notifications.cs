using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_Notifications : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public GameObject container;
    public GameObject background;
    public TextMeshProUGUI text;

    private int autoHideSecondsDefault = 5;

    //for resizing the background
    public Vector2 backgroundPadding; // pixels
    private RectTransform textRect;

    private Vector3 containerOriginalScale;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        text.text = string.Empty;

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
        if (background == null || text == null) return;

        // force TMP to calculate correct geometry
        text.ForceMeshUpdate();

        RectTransform backgroundRect = background.GetComponent<RectTransform>();

        float wrapWidth = textRect.rect.width;

        // exact size the text wants (pixels)
        Vector2 preferred = text.GetPreferredValues(text.text, wrapWidth, 0f);
        
        // resize background to text size + padding
        backgroundRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            wrapWidth + backgroundPadding.x
        );

        backgroundRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            preferred.y + backgroundPadding.y
        ); ;

        // keep background centered on text
        backgroundRect.anchoredPosition = text.textBounds.center - new Vector3(0, backgroundPadding.y/4f, 0);
    }

    private IEnumerator AutoHideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}
