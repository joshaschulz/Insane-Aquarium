using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ToiletFish : MonoBehaviour
{
    private Vector3 startPos;
    private float targetX;
    private float moveSpeed;
    private float timeToNextChange;
    private float timer;

    [Header("Difficulty Settings")]
    [Tooltip("Higher values make the fish move faster and more erratic.")]
    public float difficultyMultiplier = 1f;

    [Header("Motion Settings")]
    public float maxHorizontalRange = 0.5f;

    private float lastX;

    void Start()
    {
        startPos = transform.position;
        lastX = startPos.x;
        SetNewTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move toward target
        Vector3 currentPos = transform.position;
        float newX = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * moveSpeed);
        transform.position = new Vector3(newX, currentPos.y, currentPos.z);

        // Flip sprite based on direction
        float deltaX = newX - lastX;
        if (Mathf.Abs(deltaX) > 0.001f)
        {
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Sign(deltaX) * Mathf.Abs(scale.y);  // Flip vertically instead
            transform.localScale = scale;
        }

        lastX = newX;

        if (timer >= timeToNextChange)
        {
            SetNewTarget();
            timer = 0f;
        }
    }

    void SetNewTarget()
    {
        targetX = startPos.x + Random.Range(-maxHorizontalRange, maxHorizontalRange);
        moveSpeed = Random.Range(2f, 6f) * difficultyMultiplier;
        timeToNextChange = Random.Range(0.3f, 1.2f) / difficultyMultiplier;
    }
}