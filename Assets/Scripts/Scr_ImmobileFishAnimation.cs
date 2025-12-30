using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ImmobileFishAnimation : MonoBehaviour
{


    private GameObject frontContainer;
    public Animator frontAnimator;


    public void Awake()
    {
        // These are the gameobjects that hold the front and side images of the fish and their animators. The side one also has the mouth collision circle
        frontContainer = transform.GetChild(0).gameObject;


        if (!frontContainer.name.Contains("Front Container"))
            Debug.Log(gameObject.name + "'s second child's name does not contain 'Front Container'.");


        frontAnimator = frontContainer.GetComponent<Animator>();
        frontAnimator.Play("Fish Spawn");
    }

    public bool IsAnimationPlaying(Animator anim, string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime < 1f;
    }
}
