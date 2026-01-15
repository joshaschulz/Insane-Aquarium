using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_EndDay : MonoBehaviour
{
    [Header("refs")]
    public CanvasGroup blackScreenCanvasGroup;   // CanvasGroup on BlackScreen image
    public RectTransform endOfDayPaper;          // RectTransform of the paper panel

    public Canvas endOfDayCanvas;

    [Header("timing")]
    public float fadeInSeconds = 0.6f;
    public float paperSlideSeconds = 0.6f;

    [Header("paper motion")]
    [Tooltip("how far below its final position the paper starts (in UI pixels)")]
    public float paperStartOffsetY = 900f;

    private Vector2 paperOnScreenPos;
    private Vector2 paperOffScreenPos;
    private Coroutine routine;

    void Awake()
    {
        // cache final position as the 'on screen' target
        if (endOfDayPaper != null)
        {
            paperOnScreenPos = endOfDayPaper.anchoredPosition;
            paperOffScreenPos = paperOnScreenPos + new Vector2(0f, -paperStartOffsetY);
        }

        // start hidden (optional safety)
        if (blackScreenCanvasGroup != null) blackScreenCanvasGroup.alpha = 0f;
        if (endOfDayPaper != null) endOfDayPaper.anchoredPosition = paperOffScreenPos;

        endOfDayCanvas.gameObject.SetActive(false); ; // if you want this whole canvas hidden until EndDay
    }

    public void PlayEndDayUI()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(EndDaySequence());
    }

    public void PlayCloseEndDayUI()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(CloseEndDaySequence());
    }


    private IEnumerator EndDaySequence()
    {
        endOfDayCanvas.gameObject.SetActive(true);

        // ensure correct starting states every time
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0f;
            blackScreenCanvasGroup.blocksRaycasts = true; // blocks clicks behind it
        }

        if (endOfDayPaper != null)
        {
            endOfDayPaper.anchoredPosition = paperOffScreenPos;
            endOfDayPaper.gameObject.SetActive(true);
        }

        // 1) fade black in
        if (blackScreenCanvasGroup != null)
            yield return FadeCanvasGroup(blackScreenCanvasGroup, 0f, 1f, fadeInSeconds);

        // 2) slide paper up
        if (endOfDayPaper != null)
            yield return SlideRect(endOfDayPaper, paperOffScreenPos, paperOnScreenPos, paperSlideSeconds);
    }

    private IEnumerator CloseEndDaySequence()
    {
        // slide paper back down
        if (endOfDayPaper != null)
            yield return SlideRect(endOfDayPaper, paperOnScreenPos, paperOffScreenPos, paperSlideSeconds);

        // fade black screen out
        if (blackScreenCanvasGroup != null)
            yield return FadeCanvasGroup(blackScreenCanvasGroup, 1f, 0f, fadeInSeconds);

        // disable canvas
        if (endOfDayCanvas != null)
            endOfDayCanvas.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float seconds)
    {
        cg.alpha = from;

        if (seconds <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // works even if you paused time
            float u = Mathf.Clamp01(t / seconds);

            // smooth-ish fade (optional)
            float eased = u * u * (3f - 2f * u); // smoothstep
            cg.alpha = Mathf.Lerp(from, to, eased);

            yield return null;
        }

        cg.alpha = to;
    }

    private IEnumerator SlideRect(RectTransform rt, Vector2 from, Vector2 to, float seconds)
    {
        rt.anchoredPosition = from;

        if (seconds <= 0f)
        {
            rt.anchoredPosition = to;
            yield break;
        }

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / seconds);

            // smooth slide
            float eased = u * u * (3f - 2f * u); // smoothstep
            rt.anchoredPosition = Vector2.Lerp(from, to, eased);

            yield return null;
        }

        rt.anchoredPosition = to;
    }


}
