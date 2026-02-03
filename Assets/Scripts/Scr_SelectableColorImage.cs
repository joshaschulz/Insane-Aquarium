using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Scr_SelectableColorImage : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Colors")]
    public Color hoverColor = Color.gray;
    public Color selectedColor = Color.black;

    private Image image;
    private Color originalColor;

    private bool isHovered;
    private bool isSelected;

    // All buttons of this type in the scene
    private static readonly List<Scr_SelectableColorImage> group = new();

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        group.Add(this);
    }

    void OnDestroy()
    {
        group.Remove(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;
        isHovered = true;
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;
        isHovered = false;
        image.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        foreach (var button in group)
        {
            button.Deselect();
        }

        isSelected = true;
        image.color = selectedColor;
    }

    public void Deselect()
    {
        isSelected = false;
        isHovered = false;
        image.color = originalColor;
    }
}