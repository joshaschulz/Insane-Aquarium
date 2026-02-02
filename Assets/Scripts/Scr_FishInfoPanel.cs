using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FishInfoPanel : MonoBehaviour
{
    private Scr_GameManager gameManager;

    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_Text nameBorderText;
    public TMPro.TMP_Text speciesText;
    public TMPro.TMP_Text generationText;
    public TMPro.TMP_Text radiatedText;
    public TMPro.TMP_Text wildText;
    public TMPro.TMP_Text valueText;
    public TMPro.TMP_Text fishStateText;
    public GameObject stressFactorTextPrefab;

    public Vector3 stressFactorOffset;


    public TMPro.TMP_Text statText;


    [Header("Follow Settings")]
    public Vector3 worldOffset = new Vector3(0f, -2f, 0f); // below fish in WORLD units
    public float screenPadding = 10f; // padding from screen edges

    public Transform currentTarget;
    private Camera cam;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private float originalRectHeight;

    private string prevFishName;

    public Scr_Fish currentFish;
    //public Scr_Fish CurrentFish => currentFish; // read-only property


    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        gameManager = Scr_GameManager.GMinstance;
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        originalRectHeight = rectTransform.rect.height;

    }



    private void OnDisable()
    {

        Hide();

    }

    public void Show(Scr_Fish fish)
    {
        gameObject.SetActive(true);
        
        currentFish = fish;
        //currentTarget = fish.transform;

        nameInput.text = fish.name;
        nameBorderText.text = fish.name;
        speciesText.text = fish.tag;
        generationText.text = fish.generation.ToString();
        radiatedText.text = fish.radiated ? "Yes" : "No";
        wildText.text = fish.wild ? "Yes" : "No";
        valueText.text = fish.fishValue.ToString();
        DetermineFishState();
        

        currentTarget = fish.transform;


        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalRectHeight);

    }

    public void ShowExotic(Scr_Starfish fish)
    {
        gameObject.SetActive(true);

        currentTarget = fish.transform;

        nameInput.text = fish.name;
        nameBorderText.text = fish.name;


        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);


    }



    public void Hide()
    {
        currentTarget = null;

        gameObject.SetActive(false);
    }

    public void HideIfFish(Scr_Fish fish)
    {
        if (currentTarget == fish.transform)
            Hide();
    }
    public void BeginRename()
    {
        if (currentFish == null) return;

        prevFishName = nameInput.text;

        nameInput.readOnly = false;
        nameInput.ActivateInputField();
    }
    public void FinishRename()
    {
        if (currentFish == null) return;

        if (nameInput.text == "")
        {
            nameInput.text = prevFishName;
        }

        currentFish.name = nameInput.text;
        nameBorderText.text = nameInput.text;
        nameInput.readOnly = true;

        DetermineFishState();
    }
    public void UpdateBorderText()
    {
        if (currentFish == null) return;

        nameBorderText.text = nameInput.text;
    }

    public void DetermineFishState()
    {
        fishStateText.text = currentFish.isStressed ? (currentFish.name + " is stressed") : (currentFish.name + " is doing fine");
        
        for (int i = fishStateText.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(fishStateText.transform.GetChild(i).gameObject);
        }

        if (currentFish.isStressed)
        {
            int numOfStressFactors = 0;
            foreach (Scr_Stress.StressFactor s in currentFish.currentTankScript.speciesStressDict[currentFish.thisPrefab])
            {
                numOfStressFactors++;
                GameObject newStressFactor = Instantiate(stressFactorTextPrefab, fishStateText.transform);
                newStressFactor.transform.localPosition += stressFactorOffset * numOfStressFactors;
                switch (s)
                {
                    case Scr_Stress.StressFactor.Crowded:
                        newStressFactor.GetComponent<TMPro.TMP_Text>().text = (currentFish.name + " is too crowded");
                        break;
                    case Scr_Stress.StressFactor.Lonely:
                        newStressFactor.GetComponent<TMPro.TMP_Text>().text = (currentFish.name + " is too lonely");
                        break;
                    case Scr_Stress.StressFactor.Hungry:
                        newStressFactor.GetComponent<TMPro.TMP_Text>().text = (currentFish.name + " is hungry");
                        break;
                    case Scr_Stress.StressFactor.PredatorNearby:
                        newStressFactor.GetComponent<TMPro.TMP_Text>().text = (currentFish.name + " is in danger");
                        break;
                    case Scr_Stress.StressFactor.DirtyTank:
                        newStressFactor.GetComponent<TMPro.TMP_Text>().text = (currentFish.name + " is disgusted with their tank");
                        break;
                }

            }
        }
    }
}
