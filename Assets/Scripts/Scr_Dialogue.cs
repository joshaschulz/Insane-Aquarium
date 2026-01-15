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
    public Scr_PhoneContact currentContact;
    public GameObject customerTradePanel;
    public GameObject fishyGuyTradePanel;

    public int index;

    private Coroutine typeLine;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = Scr_GameManager.GMinstance;

        textComponent.text = string.Empty;

    }

    private void OnEnable()
    {

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

    public void StartDialogue(GameObject element)
    {
        textComponent.text = string.Empty;
        index = 0;
        StartCoroutine(StartDialogueSequence(element));
    }

    IEnumerator StartDialogueSequence()
    {
        // Play the opening animation first
        yield return StartCoroutine(OpenBox());

        // Then start typing
        yield return typeLine = StartCoroutine(TypeLine());
    }
    IEnumerator StartDialogueSequence(GameObject element)
    {
        // Play the opening animation first
        yield return StartCoroutine(CustomerOpenBox(element));

        // Then start typing
        yield return typeLine = StartCoroutine(TypeLine());
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

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            typeLine = StartCoroutine(TypeLine());
        }
        else
        {
            StartCloseBoxEnum();
            gameManager.currentlyCalling = "";
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
            if (typeLine != null)
            {
                StopCoroutine(typeLine);
                textComponent.text = lines[index];
            }

        }
    }

    public IEnumerator OpenBox()
    {
        RectTransform rect = GetComponent<RectTransform>();
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
    public void StartCloseBoxEnum()
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(CloseBox());
    }
    public IEnumerator CloseBox()
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
        rect.localScale = originalScale; // reset for next time
    }


    public void StartCustomerOpenBoxEnum(GameObject element)
    {
        StartCoroutine(CustomerOpenBox(element));
    }
    IEnumerator CustomerOpenBox(GameObject element)
    {
        element.SetActive(true);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector3 targetScale = rect.localScale;
        rect.localScale = Vector3.zero;
        float duration = 0.15f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rect.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            Debug.Log(rect.localScale);

            yield return null;
        }

        rect.localScale = targetScale;

        Debug.Log("OPENED " + element.name + " BOX");
    }
    public void StartCustomerCloseBoxEnum(GameObject element)
    {
        if (!gameObject.activeInHierarchy)
            return;
        StartCoroutine(CustomerCloseBox(element));
    }
    IEnumerator CustomerCloseBox(GameObject element)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
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
        rect.localScale = originalScale; // reset for next time

        element.SetActive(false);
        gameObject.SetActive(false);
    }


}
