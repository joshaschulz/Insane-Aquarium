using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Scr_OpenLinkButton : MonoBehaviour
{
    public void OpenWebsite(string url)
    {
        Application.OpenURL(url);
    }
}