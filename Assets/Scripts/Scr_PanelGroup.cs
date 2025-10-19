using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PanelGroup : MonoBehaviour
{
    public GameObject[] panels;

    public Scr_TabGroup tabGroup;

    public int panelIndex;


    private void Awake()
    {
        ShowCurrentPanel();
    }

    void ShowCurrentPanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == panelIndex)
            {
                panels[i].gameObject.SetActive(true);
            }
            else
            {
                panels[i].gameObject.SetActive(false);
            }
        }
    }
    public void SetPageIndex(int _index)
    {
        panelIndex = _index;
        ShowCurrentPanel();
    }
}
