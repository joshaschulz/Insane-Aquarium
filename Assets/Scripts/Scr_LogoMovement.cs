using System.Collections;
using UnityEngine;

public class Scr_LogoMovement : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private AudioSource reelAudioSource;

    [Header("Movement Settings")]
    public float startYOffset = -300f;
    public float segmentDuration = 0.4f;
    public float struggleDuration = 0.2f;
    public float horizontalOffset = 50f;

    [Header("Jitter Settings")]
    public float maxJitterX = 5f;
    public float maxJitterY = 5f;
    public float jitterFrequency = 20f;

    [Header("Line Settings")]
    public RectTransform fishingLine;
    public RectTransform lineEnd;
    public float lineRetractDuration = 0.25f;
    public float lineRetractSpeedMultiplier = 1.2f;

    [Header("Audio Settings")]
    public float reelVolume = 0.7f;

    private RectTransform rt;
    private Vector2 targetPosition;
    void Awake()
    {
        Scr_MenuRockFall.AllowFalling = false;
    }

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        rt = GetComponent<RectTransform>();
        targetPosition = rt.anchoredPosition;
        rt.anchoredPosition = targetPosition + new Vector2(0, startYOffset);

        // Setup AudioSource for reel
        reelAudioSource = GetComponent<AudioSource>();
        if (reelAudioSource == null)
            reelAudioSource = gameObject.AddComponent<AudioSource>();

        reelAudioSource.clip = gameManager.SFX_Reeling;
        reelAudioSource.loop = true;
        reelAudioSource.volume = reelVolume;
        reelAudioSource.Play(); // start reel immediately

        StartCoroutine(PullLogoSequence());
    }

    private IEnumerator PullLogoSequence()
    {
        Vector2 startPos = rt.anchoredPosition;

        // First segment
        Vector2 firstTarget = startPos + new Vector2(
            -horizontalOffset,
            (targetPosition.y - startPos.y) / 3f
        );
        yield return StartCoroutine(MoveWithJitter(firstTarget, segmentDuration));
        yield return new WaitForSeconds(struggleDuration);

        // Second segment
        Vector2 secondTarget = startPos + new Vector2(
            horizontalOffset,
            2f * (targetPosition.y - startPos.y) / 3f
        );
        yield return StartCoroutine(MoveWithJitter(secondTarget, segmentDuration));
        yield return new WaitForSeconds(struggleDuration);

        // Final segment
        yield return StartCoroutine(MoveWithJitter(targetPosition, segmentDuration));

        UpdateFishingLine();

        // Retract line
        yield return StartCoroutine(RetractFishingLineUpward());

        // Stop reel sound AFTER pull + line retract
        if (reelAudioSource != null)
            reelAudioSource.Stop();

        // Trigger rocks to fall
        Scr_MenuRockFall.AllowFalling = true;

        gameManager.PlaySong(gameManager.mainMenuSong);
    }

    private IEnumerator MoveWithJitter(Vector2 destination, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeT = Mathf.Sin(t * Mathf.PI * 0.5f);

            Vector2 basePos = Vector2.Lerp(start, destination, easeT);

            float jitterX =
                (Mathf.PerlinNoise(Time.time * jitterFrequency, 0f) - 0.5f) * 2f * maxJitterX;
            float jitterY =
                (Mathf.PerlinNoise(0f, Time.time * jitterFrequency) - 0.5f) * 2f * maxJitterY;

            rt.anchoredPosition = basePos + new Vector2(jitterX, jitterY);

            UpdateFishingLine();
            yield return null;
        }

        rt.anchoredPosition = destination;
        UpdateFishingLine();
    }

    private IEnumerator RetractFishingLineUpward()
    {
        if (fishingLine == null)
            yield break;

        float elapsed = 0f;
        float startLength = fishingLine.sizeDelta.y;
        Vector3 startLocalPos = fishingLine.localPosition;
        Vector3 upDir = fishingLine.up;

        while (elapsed < lineRetractDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lineRetractDuration;
            float easeT = t * t;

            float newLength = Mathf.Lerp(startLength, 0f, easeT);
            fishingLine.sizeDelta = new Vector2(fishingLine.sizeDelta.x, newLength);

            fishingLine.localPosition =
                startLocalPos + upDir * (startLength - newLength) * lineRetractSpeedMultiplier;

            yield return null;
        }

        Destroy(fishingLine.gameObject);
    }

    private void UpdateFishingLine()
    {
        if (fishingLine == null || lineEnd == null)
            return;

        Vector3 localEnd = fishingLine.parent.InverseTransformPoint(lineEnd.position);
        Vector3 dir = localEnd - fishingLine.localPosition;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        fishingLine.localRotation = Quaternion.Euler(0f, 0f, angle);

        fishingLine.sizeDelta = new Vector2(fishingLine.sizeDelta.x, dir.magnitude);
    }
}
