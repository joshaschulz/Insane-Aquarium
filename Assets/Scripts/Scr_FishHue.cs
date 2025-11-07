using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishHue : MonoBehaviour
{
    public float defaultHueValue = 1f;

    private SpriteRenderer[] spriteRenderers;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        // Get all SpriteRenderers in children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        mpb = new MaterialPropertyBlock();
    }

    void Start()
    {
        float randomHueValue = defaultHueValue += Random.Range(-100f, 100f);

        UpdateHue(randomHueValue);
    }

    public void UpdateHue(float _newHueValue)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_HueValue", _newHueValue);
            sr.SetPropertyBlock(mpb);
        }
    }
}
