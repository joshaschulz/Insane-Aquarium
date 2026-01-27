using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_HoverableUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Scr_GameManager gameManager;

    private Vector3 originalScale;
    private bool isHovering = false;
    private Coroutine pulseRoutine;

    public float scaleAmount = 1.05f;  // Maximum size multiplier
    public float pulseSpeed = 1.1f;   // Speed of pulsing



    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
    }

    void OnEnable()
    {
        // Only store originalScale the first time
        if (originalScale == Vector3.zero)
        {
            originalScale = transform.localScale;
        }

        isHovering = false;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If time is paused...
        if (gameManager.Scr_TimeHandler.timePaused)
        {
            // If you are hovering over an object whose panel is active...
            if (gameObject.name == "Notepad" && gameManager.fishpedia.activeSelf)
            {
                StartHoverPulse();
                return;
            }
            else if (gameObject.name == "Tackle Box" && gameManager.baitAndTackleScreen.activeSelf)
            {
                StartHoverPulse();
                return;
            }
            else
            {
                return;
            }
        }

        StartHoverPulse();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        transform.localScale = originalScale; // Reset scale when exiting
    }
    void StartHoverPulse()
    {
        isHovering = true;
        if (pulseRoutine == null)
        {
            pulseRoutine = StartCoroutine(PulseEffect());
        }
    }
    IEnumerator PulseEffect()
    {
        while (isHovering)
        {
            float timer = 0f;
            while (timer < 1f)
            {
                float scale = Mathf.Lerp(1f, scaleAmount, Mathf.Sin(timer * Mathf.PI));
                transform.localScale = originalScale * scale;
                timer += Time.deltaTime * pulseSpeed;
                yield return null;
            }
        }
    }
}