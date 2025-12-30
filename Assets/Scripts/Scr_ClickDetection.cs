using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Scr_ClickDetection : MonoBehaviour
{

    Scr_GameManager gameManager;
    public Scr_FishInfoPanel infoPanel;



    private void Awake()
    {
        gameManager = GetComponent<Scr_GameManager>();
    }

    void Update()
    {
        // ============================
        // LEFT CLICK
        // ============================
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
                    // Attempt to bag a fish
                    foreach (var fishPrefab in gameManager.fishPrefabs)
                    {
                        if (hit.collider.CompareTag(fishPrefab.tag) && gameManager.canIBagFish)
                        {
                            GameObject fishToBag = hit.collider.gameObject;

                            if (fishToBag.CompareTag("Starfish"))
                            {
                                gameManager.BagAStarfish(fishToBag);
                            }
                            else
                            {
                                gameManager.BagAFish(fishToBag);
                            }

                        }
                    }
                }

                // Drop food if selected
                if (gameManager.currentFishFoodSelected != null)
                {
                    gameManager.DropFood(gameManager.currentFishFoodSelected);

                }
            }
        }

        // ============================
        // RIGHT CLICK
        // ============================
        if (Input.GetMouseButtonDown(1))
        {
            // --- CAST TO WORLD ---
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // ==============================================
                // 1) FIRST: Check for a FishFeeder interaction
                // ==============================================
                Scr_FishFeeder feeder = hit.collider.GetComponent<Scr_FishFeeder>();

                if (feeder != null)
                {
                    // If holding food → assign that food to feeder
                    if (gameManager.currentFishFoodSelected != null)
                    {
                        feeder.SetFoodType(gameManager.currentFishFoodSelected, gameManager.currentFishFoodButtonSelected);
                    }
                    else
                    {
                        // Empty hand → cycle feeder speed
                        feeder.CycleSpeed();
                    }

                    // We handled this click—do NOT run fish info / clearing logic
                    return;
                }

                // ==============================================
                // 2) NEXT: Check for fish info panel logic
                // ==============================================

                if (hit.collider.GetComponent<Scr_Fish>() != null)
                {
                    Scr_Fish fish = hit.collider.GetComponent<Scr_Fish>();

                    if (fish != null)
                    {
                        if (infoPanel.gameObject.activeSelf && infoPanel.CurrentFish == fish)
                        {
                            infoPanel.Hide();
                        }
                        else
                        {
                            infoPanel.Show(fish);
                        }

                        return; // stop further processing
                    }
                }

            }

            // ==============================================
            // 3) RIGHT-CLICKED NOTHING IMPORTANT:
            //    → reset tools & hide UI (original behavior)
            // ==============================================

            // Clear selected food (original behavior)
            if (gameManager.currentFishFoodSelected != null)
            {
                gameManager.ChangeFishFoodTypeToDrop(null);
            }

            // Deselect bagging
            if (gameManager.canIBagFish)
            {
                gameManager.DeselectFishBag();
            }

            // Hide fish info panel
            infoPanel.Hide();
        }
    }
}
