using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_PlayerSkills : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Scr_Tooltip tooltip;


    public GameObject skillsObj;

    public GameObject skillInfoPanel;
    public TextMeshPro skillNameText;
    public TextMeshPro skillDescriptionText;

    public GameObject fishkeepingSkilsLeg;
    public GameObject fishingSkilsLeg;
    public GameObject customerServiceSkilsLeg;
    public GameObject researchSkilsLeg;
    public GameObject accountingSkilsLeg;

    public bool[] currentFishkeepingSkills = new bool[5];
    public bool[] currentFishingSkils = new bool[5];
    public bool[] currentCustomerServiceSkils = new bool[5];
    public bool[] currentResearchSkils = new bool[5];
    public bool[] currentAccountingSkils = new bool[5];

    public int[] skillCosts = new int[5] { 200, 300, 500, 800, 1300 };

    private Dictionary<GameObject, bool[]> skillMap;

    private void Awake()
    {
        gameManager = GetComponent<Scr_GameManager>();
        InitializeSkillMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            skillsObj.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            skillsObj.SetActive(false);
        }
    }

    public void BuySkill(GameObject legObj)
    {
        foreach (KeyValuePair<GameObject, bool[]> kvp in skillMap)
        {
            GameObject parent = kvp.Key;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);
                GameObject childObj = child.gameObject;

                if (legObj == childObj) //found skill to buy
                {
                    if (CheckCanBuySkill(kvp.Value, i))
                    {
                        kvp.Value[i] = true;
                        gameManager.SubtractMoneyAmount(skillCosts[i]);

                        legObj.GetComponent<Scr_SkillsHover>().correspondingSkill.SetActive(true);
                        return;
                    }
                }
            }
        }
    }

    private bool CheckCanBuySkill(bool[] skills, int index)
    {
        if (index == 0 || skills[index - 1]) //it's the first skill or has boughten the previous skill
        {
            if (gameManager.moneyAmount > skillCosts[index]) //check enough money
            {
                return true;
            }
            else
            {
                gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);
                gameManager.notifications.Show("You do not have enough money to purchase that skill.");
                return false;
            }
        }
        else
        {
            gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);
            gameManager.notifications.Show("You must buy the previous skill first.");
            return false;
        }
    }

    public void ShowSkill(string skillName, string skillDescription)
    {
        //display panel in the center of starfish and tooltip to say purchase, if available for purchase
        skillInfoPanel.SetActive(true);
        skillNameText.text = skillName;
        skillDescriptionText.text = skillDescription;


        //tooltip.Show("Buy skill");
    }

    public void HideSkill()
    {
        skillInfoPanel.SetActive(false);
        skillNameText.text = string.Empty;
        skillDescriptionText.text = string.Empty;

        tooltip.Hide();
    }

    private void InitializeSkillMap()
    {
        skillMap = new Dictionary<GameObject, bool[]> {
        { fishkeepingSkilsLeg, currentFishkeepingSkills },
        { fishingSkilsLeg, currentFishingSkils },
        { customerServiceSkilsLeg, currentCustomerServiceSkils },
        { researchSkilsLeg, currentResearchSkils },
        { accountingSkilsLeg, currentAccountingSkils }
        };
    }
}
