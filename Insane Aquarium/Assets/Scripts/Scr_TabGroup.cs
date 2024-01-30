using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_TabGroup : MonoBehaviour
{

    public List<Scr_TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;
    public Scr_TabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public Scr_PanelGroup panelGroup;

    public void Subscribe(Scr_TabButton _button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<Scr_TabButton>();
        }
        tabButtons.Add(_button);
    }

    public void OnTabEnter(Scr_TabButton _button)
    {
        ResetTabs();
        if (selectedTab == null || _button != selectedTab)
        {
            _button.background.color = tabHover;
        }
    }
    public void OnTabExit(Scr_TabButton _button)
    {
        ResetTabs();
    }
    public void OnTabSelected(Scr_TabButton _button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = _button;

        selectedTab.Select();

        ResetTabs();
        _button.background.color = tabActive;
        int index = _button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
        if (panelGroup != null)
        {
            panelGroup.SetPageIndex(_button.transform.GetSiblingIndex());
        }
    }
    public void ResetTabs()
    {
        foreach(Scr_TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.color = tabIdle;
        }
    }

    
    /*
    public List<Scr_TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;
    public Scr_TabButton selectedTab;
    public List<GameObject> objectsToSwap;




    protected List<Scr_TabButton> _tabs = new List<Scr_TabButton>();

    public bool tabSwapsActiveGameobject;
    public GameObject[] gameObjects;

    public Scr_PanelGroup panelGroup;

    [SerializeField]
    protected Scr_TabButton activeTab;

    public Action onTabSelectedCallback;


    private void Start()
    {
        StartActiveTab();
    }

    public void StartActiveTab()
    {
        if (StartActiveTab != null)
        {
            SetActive(activeTab);
        }
    }

    public void Subscribe(Scr_TabButton tab)
    {
        if (_tabs == null)
        {
            _tabs = new List<Scr_TabButton>();
        }
        _tabs.Add(tab);
    }

    public void Unsubscribe(Scr_TabButton tab)
    {
        _tabs.Remove(tab);
        if (activeTab = tab)
        {
            activeTab = null;
        }
    }

    public void SetActive(Scr_TabButton tab)
    {
        foreach (Scr_TabButton t in _tabs)
        {
            t.Deactivate();

        }

        activeTab = tab;
        activeTab.Activate();

        if (onTabSelectedCallback != null)
        {
            onTabSelectedCallback();
        }

        if (tabSwapsActiveGameObjects)
        {
            SwapGameObject();
        }

        if (panelGroup != null)
        {
            panelGroup.SetPageIndex(tab.transform.GetSiblingIndex());
        }

    }


    public void SetActive(int siblingIndex)
    {
        foreach (Tab t in _tabs)
        {
            if (t.transform.GetSiblingIndex() == siblingIndex)
            {
                SetActive(t);
                return;
            }
        }
    }

    void SwapGameObject()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(false);
        }
        if (activeTab.transform.GetSiblingIndex() < gameObjects.Length)
        {
            gameObjects[activeTab.transform.GetSiblingIndex()].SetActive(true);
        }
    }

    public void DisableTag(int index)
    {
        if (_tabs.Count > index)
        {
            _tabs[index].Disable();
        }
    }

    public void EnableTab(int index)
    {
        if (_tabs.Count > index)
        {
            _tabs[index].Enable();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (activeTab.transform.GetSiblingIndex() > 0)
            {
                SetActive(activeTab.transform.GetSiblingIndex() - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activeTab.transform.GetSiblingIndex() < (transform.childCount - 1))
            {
                SetActive(activeTab.transform.GetSiblingIndex() + 1);
            }
        }
    }

    */

}
