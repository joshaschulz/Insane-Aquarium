using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_EndDay : MonoBehaviour
{
    private Scr_GameManager gameManager;

    [Header("refs")]
    public CanvasGroup blackScreenCanvasGroup;   // CanvasGroup on BlackScreen image
    public RectTransform endOfDayPaper;          // RectTransform of the paper panel
    public GameObject skillsStarfish;

    public Canvas endOfDayCanvas;

    public TextMeshProUGUI endDayClockText;
    public TextMeshProUGUI endDayMoneyText;

    public Scr_Dialogue dialogueBox;
    public Scr_CustomerContact fishyGuyDialogue;

    public RectTransform nextDaySign;

    [Header("timing")]
    public float fadeInSeconds = 0.6f;
    public float paperSlideSeconds = 0.6f;

    [Header("paper motion")]
    [Tooltip("how far below its final position the paper starts (in UI pixels)")]
    public float paperStartOffsetY = 900f;

    private float starfishUnitsUp = 11;

    private Vector2 paperOnScreenPos;
    private Vector2 paperOffScreenPos;

    private Vector2 nextDayButtonOnScreenPos;
    private Vector2 nextDayButtonOffScreenPos;
    private Coroutine routine;

    private Vector3 starfishOriginalPos;

    void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();

        starfishOriginalPos = skillsStarfish.transform.position;

        endDayClockText.text = string.Format("{0:00}:{1:00}", gameManager.GetComponent<Scr_TimeHandler>().endHour, gameManager.GetComponent<Scr_TimeHandler>().endMinute);
        gameManager.UpdateText(endDayMoneyText, gameManager.moneyAmount);


        // cache final position as the 'on screen' target
        if (endOfDayPaper != null)
        {
            paperOnScreenPos = endOfDayPaper.anchoredPosition;
            paperOffScreenPos = paperOnScreenPos + new Vector2(0f, -paperStartOffsetY);
        }

        if (nextDaySign != null)
        {
            nextDayButtonOnScreenPos = nextDaySign.anchoredPosition;
            nextDayButtonOffScreenPos = nextDayButtonOnScreenPos + new Vector2(400, 0);
        }

        // start hidden (optional safety)
        if (blackScreenCanvasGroup != null) blackScreenCanvasGroup.alpha = 0f;
        if (endOfDayPaper != null) endOfDayPaper.anchoredPosition = paperOffScreenPos;

        endOfDayCanvas.gameObject.SetActive(false);// if you want this whole canvas hidden until EndDay
    }

    public void PlayEndDayUI()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(EndDaySequence());
        gameManager.UpdateText(endDayMoneyText, gameManager.moneyAmount);

    }

    public void PlayCloseEndDayUI()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(CloseEndDaySequence());
    }

    public void PlayOpenDialogue()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(OpenDialogueSequence());
    }

    public void PlayOpenSkills()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(OpenSkillsSequence());
    }

    private IEnumerator OpenSkillsSequence()
    {
        if (skillsStarfish != null)
        {
            skillsStarfish.gameObject.SetActive(true);
        }

        skillsStarfish.GetComponent<BoxCollider2D>().enabled = false;

        // 2) slide paper up
        if (skillsStarfish != null)
            yield return SlideTransform(skillsStarfish.transform, skillsStarfish.transform.position, Vector3.zero, paperSlideSeconds);

        if (nextDaySign != null)
        {
            nextDaySign.gameObject.SetActive(true);
        }

        if (nextDaySign != null)
            yield return SlideRect(nextDaySign, nextDayButtonOffScreenPos, nextDayButtonOnScreenPos, paperSlideSeconds);


        if (!gameManager.tutorials.tutorialCompleted)
        {
            gameManager.tutorials.HideTutorialBox();
            gameManager.tutorials.ShowNextTutorialBoxDelay(0.6f);
        }
    }

    private IEnumerator OpenDialogueSequence()
    {
        // slide paper back down
        if (endOfDayPaper != null)
            yield return SlideRect(endOfDayPaper, paperOnScreenPos, paperOffScreenPos, paperSlideSeconds);

        // Set dialogue lines
        dialogueBox.lines = (string[])fishyGuyDialogue.dialogueLines.Clone();

        dialogueBox.gameObject.SetActive(true);
        dialogueBox.StartDialogue();
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
            yield return FadeCanvasGroup(blackScreenCanvasGroup, blackScreenCanvasGroup.alpha, 0.5f, fadeInSeconds);

        // 2) slide paper up
        if (endOfDayPaper != null)
            yield return SlideRect(endOfDayPaper, paperOffScreenPos, paperOnScreenPos, paperSlideSeconds);

        gameManager.UpdateSceneTexts();
        gameManager.UpdateText(endDayMoneyText, gameManager.moneyAmount);

    }

    private IEnumerator CloseEndDaySequence()
    {
        skillsStarfish.GetComponent<BoxCollider2D>().enabled = false;

        // slide paper back down
        if (nextDaySign != null)
            yield return SlideRect(nextDaySign, nextDayButtonOnScreenPos, nextDayButtonOffScreenPos, paperSlideSeconds);

        nextDaySign.gameObject.SetActive(false);

        if (skillsStarfish != null)
            yield return SlideTransform(skillsStarfish.transform, skillsStarfish.transform.position, starfishOriginalPos, paperSlideSeconds);

        skillsStarfish.SetActive(false);
        // fade black screen out

        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.blocksRaycasts = false; //blocks clicks behind it
            yield return FadeCanvasGroup(blackScreenCanvasGroup, 0.5f, 0f, fadeInSeconds);
        }

        // disable canvas
        if (endOfDayCanvas != null)
            endOfDayCanvas.gameObject.SetActive(false);

        gameManager.CalculateBills();
        gameManager.UpdateSceneTexts();
        gameManager.FinishBuyingSKills();
        gameManager.EnableButton(gameManager.backButton.GetComponent<Button>());


        gameManager.PlaySelectSound(1f);
        gameManager.lightSwitchOn.SetActive(true);
        gameManager.lightSwitchOff.SetActive(false);
        gameManager.lightsOff.SetActive(false);
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

    private IEnumerator SlideTransform(Transform tr, Vector3 from, Vector3 to, float seconds)
    {
        tr.position = from;

        if (seconds <= 0f)
        {
            tr.position = to;
            yield break;
        }

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / seconds);

            // smoothstep easing
            float eased = u * u * (3f - 2f * u);
            tr.position = Vector3.Lerp(from, to, eased);

            yield return null;
        }

        tr.position = to;
    }


}
