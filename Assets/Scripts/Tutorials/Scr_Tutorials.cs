using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_Tutorials : MonoBehaviour
{
    Scr_GameManager gameManager;

    public bool tutorialCompleted; //0 for no, 1 for yes (easier to store when saving)
    public List<bool> tutorialsShown;
    public List<GameObject> boxes; //box display with text (already positioned in the scene)

    public GameObject hudCanvas;
    public List<GameObject> hudButtons; //back, fish, food, structures

    private void Awake()
    {
        gameManager = FindObjectOfType<Scr_GameManager>();

        tutorialsShown = new List<bool>(10);
        tutorialsShown.AddRange(new bool[10]);
    }

    public void StartTutorial()
    {

        ShowHUDStart();
    }

    public void ShowTutorialBox(int index)
    {
        gameManager.PlaySoundEffect(gameManager.SFX_notif, 0.05f, 1.2f);

        boxes[index].SetActive(true);

        StartCoroutine(OpenBox(boxes[index]));
    }

    public void HideTutorialBox(int index)
    {
        StartCoroutine(CloseBox(boxes[index]));
    }

    public IEnumerator OpenBox(GameObject box)
    {
        RectTransform rect = box.GetComponent<RectTransform>();
        Vector3 targetScale = rect.localScale;
        rect.localScale = Vector3.zero;
        float duration = 0.15f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rect.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        rect.localScale = targetScale;
    }

    public IEnumerator CloseBox(GameObject box)
    {
        RectTransform rect = box.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        Vector3 targetScale = Vector3.zero;
        float duration = 0.15f; // how fast it closes
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        rect.localScale = Vector3.zero;
        box.SetActive(false);

        rect.localScale = originalScale; // reset for next time
    }

    public void ShowElement(GameObject obj) //this is on the game manager, but I wanted everything on the tutorials script
    {
        obj.SetActive(true);
    }
    public void HideElement(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void ShowAllHUDButtons()
    {
        foreach (GameObject button in hudButtons)
        {
            button.SetActive(true);
        }
    }
    public void HideAllHUDButtons()
    {
        foreach (GameObject button in hudButtons)
        {
            button.SetActive(false);
        }
    }
    public void DisableHUDButton(int index)
    {
        hudButtons[index].GetComponent<Button>().interactable = false;
    }
    public void EnableHUDButton(int index)
    {
        hudButtons[index].GetComponent<Button>().interactable = true;
    }
    public void DisableAllHUDButtons()
    {
        foreach (GameObject button in hudButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }
    public void EnableAllHUDButtons()
    {
        foreach (GameObject button in hudButtons)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }

    public void ShowHUDStart() //show only the back and the hunger button
    {
        ShowElement(hudCanvas);
        HideAllHUDButtons();

    }
}
