using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishHue : MonoBehaviour
{
    public float defaultHueValue = 1f;
    public float thisFishHue;

    private SpriteRenderer[] spriteRenderers;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        // Get all SpriteRenderers in children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        mpb = new MaterialPropertyBlock();

        thisFishHue = defaultHueValue;
    }

    void Start()
    {
        //float randomHueValue = defaultHueValue += Random.Range(-100f, 100f);
        //UpdateHue(randomHueValue);
    }

    public void SetHue(float _newHueValue)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_HueValue", _newHueValue);
            sr.SetPropertyBlock(mpb);
        }
        thisFishHue = _newHueValue;
        Debug.Log("Fish Hue Set To : " + thisFishHue);
    }

    public float GetHue()
    {
        return thisFishHue;
    }
}
