using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishAnimation : MonoBehaviour
{
    public enum FishState { Idle, Move }
    private FishState currentState;


    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite sideSprite;


    private GameObject sideContainer;
    private GameObject frontContainer;
    public Animator frontAnimator;


    public void Awake()
    {
        // These are the gameobjects that hold the front and side images of the fish and their animators. The side one also has the mouth collision circle
        sideContainer = transform.GetChild(0).gameObject;
        frontContainer = transform.GetChild(1).gameObject;

        if (!sideContainer.name.Contains("Side Container"))
            Debug.Log(gameObject.name + "'s first child's name does not contain 'Side Container'.");
        if (!frontContainer.name.Contains("Front Container"))
            Debug.Log(gameObject.name + "'s second child's name does not contain 'Front Container'.");


        frontAnimator = frontContainer.GetComponent<Animator>();
        frontAnimator.Play("Fish Spawn");
    }


    public void SetState(FishState newState)
    {
        if (currentState == newState) return; // skip redundant changes
        currentState = newState;

        switch (currentState)
        {
            case FishState.Idle:
                frontContainer.SetActive(true);
                sideContainer.SetActive(false);
                break;
            case FishState.Move:
                frontContainer.SetActive(false);
                sideContainer.SetActive(true);
                break;
        }
    }
    public FishState GetState()
    {
        return currentState;
    }
    public void FaceDirection(Vector2 target)
    {
        // Get vector from fish to target
        Vector2 toTarget = target - (Vector2)transform.position;

        // Ignore tiny horizontal differences
        if (Mathf.Abs(toTarget.x) < 0.01f)
            return;

        // Copy current scale
        Vector3 scale = sideContainer.transform.localScale;

        // Force positive before flipping
        scale.x = Mathf.Abs(scale.x) * (toTarget.x > 0 ? 1 : -1);
        sideContainer.transform.localScale = scale;
    }

    public bool IsAnimationPlaying(Animator anim, string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime < 1f;
    }
}
