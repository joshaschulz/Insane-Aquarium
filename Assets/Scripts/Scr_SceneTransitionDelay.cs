using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scr_SceneTransitionDelay : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public ParticleSystem bubbleParticles;
    public float delay = 2f; // time before firing the actual events


    // Drag-and-drop your existing OnClick actions here
    public UnityEvent delayedEvents;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

    }

    public void OnButtonClicked()
    {
        Debug.LogWarning("DELAY START");
        // 1. Start bubbles immediately
        if (bubbleParticles != null)
        {
            bubbleParticles.Play();

            // Play the bubbles popping sound effects (Low and High pitch bubbles)
            Invoke(nameof(PlayBubblesTransitionSound), 0.5f);
        }

        // 2. Fire the delayed events after a delay
        StartCoroutine(FireDelayedEvents());
    }
    private System.Collections.IEnumerator FireDelayedEvents()
    {
        yield return new WaitForSeconds(delay);
        delayedEvents.Invoke(); // calls all your methods with drag-and-drop references
        gameManager.GetComponent<Scr_TimeHandler>().UnpauseTime();

    }

    private void PlayBubblesTransitionSound()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_bubblesTransition, 0.7f);
    }

}