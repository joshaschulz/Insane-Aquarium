using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_BubbleWarble : MonoBehaviour
{
    [Header("Scale Warble")]
    public float widthAmount = 0.05f;
    public float heightAmount = 0.05f;
    public float speed = 1.5f;

    [Header("Scale Speed & Amplitude Boost on Hover")]
    public float hoverMultiplier = 1.7f;           // amplitude multiplier
    public float hoverScaleSpeedMultiplier = 2f;   // frequency multiplier
    public float hoverLerpSpeed = 8f;              // how quickly hover changes

    [Header("Position Drift")]
    public float positionAmount = 6f;
    public float positionFrequency = 0.6f;

    private RectTransform rectTransform;
    private Animator animator;

    private Vector3 baseScale;
    private Vector2 basePosition;

    // Per-bubble variation
    private float timeOffset;
    private float heightFrequency;
    private float xOffset;
    private float yOffset;

    private float currentHoverMultiplier = 1f;        // amplitude lerp
    private float currentScaleSpeedMultiplier = 1f;   // frequency lerp

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponentInParent<Animator>();

        baseScale = rectTransform.localScale;
        basePosition = rectTransform.anchoredPosition;

        // Randomize phase for variation per bubble
        timeOffset = Random.Range(0f, 100f);
        heightFrequency = Random.Range(1.1f, 1.5f);
        xOffset = Random.Range(0f, 100f);
        yOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        bool hovered = animator != null && animator.GetBool("Hovered");

        // Smoothly ramp amplitude (bigger squish) and speed (faster squish) on hover
        float targetHoverMultiplier = hovered ? hoverMultiplier : 1f;
        currentHoverMultiplier = Mathf.Lerp(
            currentHoverMultiplier,
            targetHoverMultiplier,
            Time.deltaTime * hoverLerpSpeed
        );

        float targetScaleSpeed = hovered ? hoverScaleSpeedMultiplier : 1f;
        currentScaleSpeedMultiplier = Mathf.Lerp(
            currentScaleSpeedMultiplier,
            targetScaleSpeed,
            Time.deltaTime * hoverLerpSpeed
        );

        float scaleT = (Time.time + timeOffset) * speed * currentScaleSpeedMultiplier;
        float driftT = (Time.time + timeOffset) * speed;

        // Scale warble (faster AND bigger on hover)
        float xScale = 1f + Mathf.Sin(scaleT) * widthAmount * currentHoverMultiplier;
        float yScale = 1f + Mathf.Cos(scaleT * heightFrequency) * heightAmount * currentHoverMultiplier;

        rectTransform.localScale = new Vector3(
            baseScale.x * xScale,
            baseScale.y * yScale,
            baseScale.z
        );

        // Position drift (unchanged, calm)
        float xPos = Mathf.Sin(driftT * positionFrequency + xOffset) * positionAmount;
        float yPos = Mathf.Cos(driftT * positionFrequency + yOffset) * positionAmount;

        rectTransform.anchoredPosition = basePosition + new Vector2(xPos, yPos);
    }
}