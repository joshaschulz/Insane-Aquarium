using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishInfoPanel : MonoBehaviour
{
    public TMPro.TMP_Text nameText;

    public UnityEngine.UI.Image[] priceStars;
    public UnityEngine.UI.Image[] appealStars;
    public UnityEngine.UI.Image[] hungerCapacityStars;
    public UnityEngine.UI.Image[] freakuencyStars;
    public UnityEngine.UI.Image[] poopIntervalStars;

    public void Show(Scr_Fish fish)
    {
        nameText.text = fish.name;
        SetStars(priceStars, fish.priceModifier);
        SetStars(appealStars, fish.appeal);
        SetStars(hungerCapacityStars, fish.hungerCapacity);
        SetStars(freakuencyStars, fish.freakuency);
        SetStars(poopIntervalStars, fish.poopInterval);

        gameObject.SetActive(true);
    }

    void SetStars(UnityEngine.UI.Image[] stars, int value)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i < value; // turn on first `value` stars
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
