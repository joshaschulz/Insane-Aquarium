using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_TooltipRent : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private Scr_GameManager gameManager;
    public Scr_Tooltip tooltip;

    private void Awake()
    {
        gameManager = Scr_GameManager.GMinstance;
    }

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
        gameManager.CalculateBills();

        string message;

        message = $"Rent: {gameManager.rentText.text}\nIncome Tax: {gameManager.incomeTaxText.text}";

        if (gameManager.loanAmount > 0)
            message += $"\nLoan Interest: {gameManager.loanInterestText.text}";

        if (gameManager.exoticFishTaxText.text != "0")
            message += $"\nExotic Fish Tax: {gameManager.exoticFishTaxText.text}";


        return message;
    }
}
