using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Check if we hit any UI objects
            if (results.Count > 0)
            {
                Debug.Log("Clicked UI element: " + results[0].gameObject.name);

                // If we clicked on the Dialogue Text...
                if (results[0].gameObject.transform.parent.GetComponent<Scr_Dialogue>())
                {
                    results[0].gameObject.transform.parent.GetComponent<Scr_Dialogue>().AdvanceText();
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // Clicked on something
                if (hit.collider != null)
                {
                    foreach (var fishPrefab in gameManager.fishPrefabs)
                    {
                        if (hit.collider.CompareTag(fishPrefab.tag) && gameManager.canIBagFish)
                        {
                            GameObject fishToBag = hit.collider.gameObject;
                            Debug.Log("BAG THIS " + fishToBag.name);
                            gameManager.BagAFish(fishToBag);
                        }
                    }

                    /*
                    else if (hit.collider.CompareTag("Coin"))
                    {
                        // Clicked on a coin
                        Debug.Log("Clicked a coin");

                        gameManager.PlaySoundEffect(gameManager.SFX_MoneyPickup, 1, 0.5f, 1.5f);
                        hit.collider.gameObject.GetComponent<Scr_CoinBehavior>().GetClicked();
                    }
                    */
                }


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

                        // Make cursor icon, selected food button, and food amount text flash red
                        gameManager.FlashColor(gameManager.cursorFollower.gameObject, Color.red, 0.5f, 0.1f);
                        gameManager.FlashColor(gameManager.currentFishFoodButtonSelected, Color.red, 0.5f, 0.1f);
                        gameManager.FlashTextColor(gameManager.currentFishFoodButtonSelected.transform.GetChild(0).GetComponent<TextMeshProUGUI>(), Color.red, 0.5f, 0.1f);
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
            if (gameManager.canIBagFish)
            {
                gameManager.DeselectFishBag();
            }
            /*
            if (gameManager.currentBaggedFishButtonSelected != null)
            {
                gameManager.DeselectBaggedFish();
            }
            */
        }

    }

}
