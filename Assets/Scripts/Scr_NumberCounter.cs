using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]

public class Scr_NumberCounter : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";

    public string TextBeforeNumber;
    public string TextAfterNumber;

    private int _value;

    public int SetValue
    {
        get
        {
            return _value;
        }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }
    private Coroutine CountingCoroutine;

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    private void UpdateText(int newValue)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }

        CountingCoroutine = StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newvalue)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _value;
        int stepAmount;

        if (newvalue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newvalue - previousValue) / (CountFPS * Duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newvalue - previousValue) / (CountFPS * Duration));
        }

        if (previousValue < newvalue)
        {
            while(previousValue < newvalue)
            {
                previousValue += stepAmount;
                if (previousValue > newvalue)
                {
                    previousValue = newvalue;
                }

                Text.SetText(TextBeforeNumber + previousValue.ToString(NumberFormat) + TextAfterNumber);

                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newvalue)
            {
                previousValue += stepAmount;
                if (previousValue < newvalue)
                {
                    previousValue = newvalue;
                }

                Text.SetText(TextBeforeNumber + previousValue.ToString(NumberFormat) + TextAfterNumber);

                yield return Wait;
            }
        }
    }

}
