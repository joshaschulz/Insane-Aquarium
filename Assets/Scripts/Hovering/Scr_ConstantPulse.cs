using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scr_ConstantPulse : MonoBehaviour
{
    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    public float scaleAmount = 1.05f;  // Maximum size multiplier
    public float pulseSpeed = 1.1f;   // Speed of pulsing

    void OnEnable()
    {
        // Only store originalScale the first time
        if (originalScale == Vector3.zero)
        {
            originalScale = transform.localScale;
        }

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        transform.localScale = originalScale;

        pulseRoutine = StartCoroutine(PulseEffect());
    }

    void OnDisable()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        transform.localScale = originalScale; // Reset scale when exiting
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (pulseRoutine == null) // Start the coroutine only if it's not already running
        {
            pulseRoutine = StartCoroutine(PulseEffect());
        }
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
    }*/

    private IEnumerator PulseEffect()
    {
        while (true)
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