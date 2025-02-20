using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Hoverable : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isHovering = false;
    private Coroutine pulseRoutine;

    public float scaleAmount = 1.05f;  // Maximum size multiplier
    public float pulseSpeed = 1.1f;   // Speed of pulsing

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        isHovering = true;
        if (pulseRoutine == null) // Start the coroutine only if it's not already running
        {
            pulseRoutine = StartCoroutine(PulseEffect());
        }
    }

    void OnMouseExit()
    {
        isHovering = false;
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        transform.localScale = originalScale; // Reset scale when exiting
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
