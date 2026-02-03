using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Scr_ClickableColorImage : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Colors")]
    public Color hoverColor = Color.gray;
    public Color pressedColor = Color.black;

    private Image image;
    private Color originalColor;
    private bool isHovered;
    private bool isPressed;

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    void OnEnable()
    {
        originalColor = image.color;
        image.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (!isPressed)
            image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isPressed)
            image.color = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        image.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        image.color = isHovered ? hoverColor : originalColor;
    }
}