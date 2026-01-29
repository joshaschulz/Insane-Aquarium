using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scr_PlayerSkills : MonoBehaviour
{
    private Scr_GameManager gameManager;
    private Scr_EndDay endDay;
    public Scr_Tooltip tooltip;


    public GameObject skillsObj;
    public GameObject buySkillsButton;

    public GameObject skillInfoPanel;
    public TextMeshPro skillNameText;
    public TextMeshPro skillDescriptionText;
    public TextMeshPro skillPriceText;

    public GameObject fishkeepingSkilsLeg;
    public GameObject fishingSkilsLeg;
    public GameObject customerServiceSkilsLeg;
    public GameObject researchSkilsLeg;
    public GameObject accountingSkilsLeg;

    public bool[] currentFishkeepingSkills = new bool[5];
    public bool[] currentFishingSkills = new bool[5];
    public bool[] currentCustomerServiceSkills = new bool[5];
    public bool[] currentResearchSkills = new bool[5];
    public bool[] currentAccountingSkills = new bool[5];

    public int[] skillCosts = new int[5] { 200, 300, 500, 800, 1300 };

    public Dictionary<GameObject, bool[]> skillMap;

    private void Awake()
    {
        gameManager = GetComponent<Scr_GameManager>();
        endDay = FindObjectOfType<Scr_EndDay>();
        InitializeSkillMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            skillsObj.SetActive(true);
            buySkillsButton.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            skillsObj.SetActive(false);
            buySkillsButton.SetActive(false);
        }
    }
    public void BuyAllSkills()
    {
        foreach (KeyValuePair<GameObject, bool[]> kvp in skillMap)
        {
            GameObject parent = kvp.Key;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);
                GameObject childObj = child.gameObject;

                kvp.Value[i] = true;
                childObj.GetComponent<Scr_SkillsHover>().correspondingSkill.SetActive(true);
            }
        }

        SendFishUpdate();
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
                        gameManager.UpdateText(endDay.endDayMoneyText, gameManager.moneyAmount);

                        gameManager.PlaySoundEffect(gameManager.SFX_CashRegister, 0.1f);
                        gameManager.PlaySoundEffect(gameManager.SFX_MoneyCounter, 0.3f);
                        gameManager.PlaySoundEffect(gameManager.SFX_buyingSkill, 0.5f);

                        legObj.GetComponent<Scr_SkillsHover>().correspondingSkill.SetActive(true);

                        SendFishUpdate();

                        return;
                    }
                }
            }
        }
    }

    public void UpdateSkillsOnLoad()
    {
        foreach (KeyValuePair<GameObject, bool[]> kvp in skillMap)
        {
            GameObject parent = kvp.Key;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);
                GameObject childObj = child.gameObject;

                if (kvp.Value[i])
                {
                    childObj.GetComponent<Scr_SkillsHover>().correspondingSkill.SetActive(true);
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
                gameManager.notifications.Show("You do not have enough money to purchase that skill.", true);
                return false;
            }
        }
        else
        {
            gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);
            gameManager.notifications.Show("You must buy the previous skill first.", true);
            return false;
        }
    }

    public void ShowSkill(GameObject leg, string skillName, string skillDescription)
    {
        //display panel in the center of starfish and tooltip to say purchase, if available for purchase
        skillInfoPanel.SetActive(true);
        skillNameText.text = skillName;
        skillDescriptionText.text = skillDescription;
        skillPriceText.text = skillCosts[leg.transform.GetSiblingIndex()].ToString();

        gameManager.PlaySoundEffect(gameManager.SFX_skillHoverPop, 0.05f, 0.7f);

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
        { fishingSkilsLeg, currentFishingSkills },
        { customerServiceSkilsLeg, currentCustomerServiceSkills },
        { researchSkilsLeg, currentResearchSkills },
        { accountingSkilsLeg, currentAccountingSkills }
        };
    }

    private void SendFishUpdate()
    {
        //UPDATE EXISTING FISH'S SKILLS
        //ALL FISH IN SCENE
        foreach (var kvp in gameManager.foodFishDictionary)
        {
            if (kvp.Key.GetComponent<Scr_Fish>() != null)
                kvp.Key.GetComponent<Scr_Fish>().UpdateSkills();
            else if (kvp.Key.GetComponent<Scr_ExoticFish>() != null)
                kvp.Key.GetComponent<Scr_ExoticFish>().UpdateSkills();
        }

        //ALL FISH IN BAGS
        foreach (GameObject socket in gameManager.baggedFishSockets)
        {
            if (socket.transform.childCount == 0)
                continue;

            GameObject baggedFishObj = socket.transform.GetChild(0).gameObject;

            if (baggedFishObj.GetComponent<Scr_Fish>())
                baggedFishObj.GetComponent<Scr_Fish>().UpdateSkills();
            else if (baggedFishObj.GetComponent<Scr_ExoticFish>())
                baggedFishObj.GetComponent<Scr_ExoticFish>().UpdateSkills();

        }
    }
}
