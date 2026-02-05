using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_BubbleHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private Scr_GameManager gameManager;

    private Animator animator;
    private bool popped = false;
    public float timeToFloatUpInitially = 0f;

    void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
        animator = GetComponent<Animator>();
    }
    
    public void Start()
    {
        PauseAnimation();
        Invoke("UnpauseAnimation", timeToFloatUpInitially);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Hovered", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Hovered", false);
    }
    public void PopBubble()
    {
        gameManager.PlaySoundEffect(gameManager.SFX_bubblePop, 1f, 0.85f, 1.15f);
        animator.SetTrigger("Pop");
        popped = true;
    }

    public void RiseBubbleIfPopped()
    {
        if (popped)
        {
            animator.Play("Rise Up");
            popped = false;
        }
    }
    public void PauseAnimation()
    {
        animator.speed = 0;
    }
    public void UnpauseAnimation()
    {
        animator.speed = 1;
    }
}
