using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scr_ClickDetection : MonoBehaviour
{

    Scr_GameManager gameManager;


    private void Awake()
    {
        gameManager = GetComponent<Scr_GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked a UI element");
            }
            else
            {
                // Check if the mouse is over a coin
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // Clicked on something
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Coin"))
                    {
                        // Clicked on a coin
                        Debug.Log("Clicked a coin");

                        gameManager.PlaySoundEffect(gameManager.SFX_MoneyPickup, 1, 0.5f, 1.5f);
                        hit.collider.gameObject.GetComponent<Scr_CoinBehavior>().GetClicked();
                    }
                }
                
                // Clicked on nothing - attempt to feed
                else
                {
                    if (gameManager.currentFishFoodSelected != null)
                    {
                        if (gameManager.GetFishFoodAmount(gameManager.currentFishFoodSelected) > 0)
                        {
                            gameManager.DropFood(gameManager.currentFishFoodSelected);

                        }
                        else // Out of selected food
                        {
                            gameManager.PlaySoundEffect(gameManager.SFX_Error, 0.3f);
                            Debug.Log("Out of Selected Fish Food");

                            // Make cursor icon and selected food button flash red
                            gameManager.FlashColor(gameManager.cursorFollower.gameObject, Color.red, 0.5f, 0.1f);
                            gameManager.FlashColor(gameManager.currentFishFoodButtonSelected, Color.red, 0.5f, 0.1f);
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (gameManager.currentFishFoodSelected != null)
            {
                gameManager.ChangeFishFoodTypeToDrop(null);
            }
        }
    }
}
