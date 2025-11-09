using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Scr_Dialogue : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        textComponent.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue()
    {
        textComponent.text = string.Empty;
        index = 0;
        StartCoroutine(StartDialogueSequence());
    }
    IEnumerator StartDialogueSequence()
    {
        // Play the opening animation first
        yield return StartCoroutine(OpenBox());

        // Then start typing
        yield return StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        int lineIndex = 0;
        int lineLength = lines[index].ToCharArray().Length - 1;

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            bool isLast = (lineIndex == lineLength);
            if (!isLast)
            {
                // Play the blip sound — skip silent characters so it doesn't sound messy
                if (char.IsLetterOrDigit(c))
                {
                    float randomPitch = Random.Range(0.35f, 0.4f);
                    gameManager.PlaySoundEffect(gameManager.SFX_TextScroll, 0.15f, randomPitch);
                }
                lineIndex++;
                yield return new WaitForSeconds(textSpeed);
            }
            else
            {
                // End the Blip sounds with a higher pitched one
                yield return new WaitForSeconds(textSpeed);
                gameManager.PlaySoundEffect(gameManager.SFX_TextScrollEnd, 0.15f, 1.4f);
            }
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCloseBoxEnum();
        }
    }

    public void AdvanceText()
    {
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }
    IEnumerator OpenBox()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
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
    public void StartCloseBoxEnum()
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(CloseBox());
    }
    IEnumerator CloseBox()
    {
        RectTransform rect = GetComponent<RectTransform>();
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

        rect.localScale = targetScale;
        gameObject.SetActive(false);
        rect.localScale = originalScale; // reset for next time
    }
}
