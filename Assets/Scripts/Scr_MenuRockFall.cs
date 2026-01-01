using System.Collections;
using UnityEngine;

public class Scr_MenuRockFall : MonoBehaviour
{
    public static bool AllowFalling = false;

    private Scr_GameManager gameManager;

    [Header("Fall Settings")]
    public float startOffsetY = 1000f;
    public float minFallSpeed = 1800f;
    public float maxFallSpeed = 2200f;
    public float maxAdditionalDelay = 0.4f;

    [Header("Sound Settings")]
    public float minVolume = 0.6f;
    public float maxVolume = 0.9f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minSoundInterval = 0.05f;

    [Header("Rotation Settings")]
    public float maxInitialRotation = 15f;

    private Vector3 targetPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float fallSpeed;

    private static float lastRockSoundTime = 0f;

    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        float randomZ = Random.Range(-maxInitialRotation, maxInitialRotation);
        startRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z + randomZ
        );
        transform.rotation = startRotation;

        transform.position += new Vector3(0, startOffsetY, 0);
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        // WAIT FOR LOGO TO FINISH
        while (!AllowFalling)
            yield return null;

        float normalizedHeight = targetPosition.y / 100f;
        float actualDelay = normalizedHeight * maxAdditionalDelay;

        if (actualDelay > 0)
            yield return new WaitForSeconds(actualDelay);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                fallSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        if (gameManager != null && Time.time - lastRockSoundTime >= minSoundInterval)
        {
            lastRockSoundTime = Time.time;
            float volume = Random.Range(minVolume, maxVolume);
            float pitch = Random.Range(minPitch, maxPitch);
            gameManager.PlaySoundEffect(gameManager.SFX_RockHit, volume, pitch, pitch);
        }
    }
}
