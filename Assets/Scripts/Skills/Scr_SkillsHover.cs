using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SkillsHover : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_PlayerSkills playerSkills;

    public GameObject correspondingSkill;

    [TextArea]
    public string skillName;
    [TextArea]
    public string skillDescription;

    private float underlayOriginalValue = 0.5f;
    private float underlayHighlightValue = 0.8f;
    private float overlayOriginalValue = 1f;
    private float overlayHighlightValue = 0.7f;




    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();
        playerSkills = gameManager.GetComponent<Scr_PlayerSkills>();
    }

    private void OnMouseEnter()
    {
        if (correspondingSkill.activeSelf)
            return;

        Highlight();

        playerSkills.ShowSkill(skillName, skillDescription);
    }
    private void OnMouseExit()
    {
        Unhighlight();

        playerSkills.HideSkill();
    }

    private void OnMouseUp()
    {
        if (correspondingSkill.activeSelf)
            return;

        playerSkills.BuySkill(gameObject);
    }


    private void OnDisable()
    {
        Unhighlight();

        playerSkills.HideSkill();
    }

    private void Highlight()
    {
        SetBrightness(gameObject, underlayHighlightValue);

        /*
        if (correspondingSkill.activeSelf)
            SetBrightness(correspondingSkill, overlayHighlightValue);*/
    }

    private void Unhighlight()
    {
        SetBrightness(gameObject, underlayOriginalValue);

        /*
        if (correspondingSkill.activeSelf)
            SetBrightness(correspondingSkill, overlayOriginalValue);*/
    }

    private void SetBrightness(GameObject obj, float value)
    {
        Color.RGBToHSV(obj.GetComponent<SpriteRenderer>().color, out float h, out float s, out float v);

        v = value;

        obj.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(h, s, v);
    }
}
