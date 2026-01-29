using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_TooltipValue : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Scr_Tooltip tooltip;
    public Scr_FishInfoPanel panel;


    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(DetermineMessage());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }

    private void OnDisable()
    {
        tooltip.Hide();
    }

    private string DetermineMessage()
    {
        Scr_Fish fish = panel.currentFish;
        Scr_PlayerSkills skills = FindObjectOfType<Scr_GameManager>().skills;
        string message;

        message = $"Base value: {fish.originalfishValue}\n";
        if (skills.currentFishkeepingSkills[2] && !fish.wild)
            message += $"Pure Bred: x2\n";
        if (skills.currentFishingSkills[3] && fish.wild)
            message += $"Free Range: x5\n";
        if (fish.mutated)
            message += $"Mutated: x1.5\n";
        if (fish.legendary)
            message += $"Legendary: x5\n";
        if (!fish.legendary && FindObjectOfType<Scr_GameManager>().legendaryCountBySpecies[fish.tag] > 0)
            message += $"Legendary Aura: x1.2";

        return message;
    }
}