using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Scr_ClickDetection : MonoBehaviour
{
    Scr_GameManager gameManager;
    public Scr_FishInfoPanel infoPanel;
    public Transform hudButtonPanel;

    [Header("raycast priority")]
    public LayerMask structureMask; // set to Structures in inspector
    public LayerMask fishMask;      // set to Fish (and/or Default) in inspector

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


            // If we clicked while dialogue text is open...
            if (gameManager.currentlyCalling != "")
            {
                if (gameManager.CheckIfOnSelectionDialogue() || gameManager.CheckIfOnFinalPurchaseDialogue() || gameManager.CheckIfOnPurchaseAgain())
                    return;
                else
                {
                    gameManager.dialogueBoxPhone.AdvanceText();
                }
            }

            if (gameManager.dialogueBoxCustomer.isActiveAndEnabled && gameManager.CheckIfInBathroom())
            {
                gameManager.dialogueBoxCustomer.AdvanceTextCustomer();
            }

            if (gameManager.dialogueBoxEndDay.isActiveAndEnabled)
            {
                if (gameManager.dialogueBoxEndDay.textComponent.text == gameManager.dialogueBoxEndDay.lines[gameManager.dialogueBoxEndDay.index])
                    gameManager.Scr_EndDay.PlayOpenSkills();

                gameManager.dialogueBoxEndDay.AdvanceText();
            }

            if (results.Count > 0)
            {
                Debug.Log("Clicked UI element: " + results[0].gameObject.name);

                // -----------------------------------------
                // NEW: cancel structure placement if clicking any UI
                // -----------------------------------------
                if (gameManager.currentStructurePrefabSelected != null)
                {
                    gameManager.CancelStructurePlacement();
                    return; // do not also place structure or drop food
                }


                // Deselect bagging
                if (gameManager.canIBagFish)
                {
                    gameManager.DeselectFishBag();
                }
                // Deselect Wrench
                if (gameManager.canIRemoveStructures)
                {
                    gameManager.DeselectWrench();
                }

                return; // UI click handled
            }
            else
            {
                // -----------------------------------------
                // 0) If we are in STRUCTURE PLACEMENT MODE:
                //    place the structure at the cursor and return.
                // -----------------------------------------
                
                if (gameManager.currentStructurePrefabSelected != null)
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                    gameManager.PlaceStructureAt(worldPos);

                    return; // do NOT bag fish or drop food on this click
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, fishMask);
                RaycastHit2D structHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, structureMask);

                // Clicked on something
                if (hit.collider != null)
                {
                    // Attempt to bag a fish
                    Debug.Log(hit.collider.gameObject.name);

                    if (hit.collider.gameObject.GetComponent<Scr_Fish>())
                    {
                        foreach (var fishPrefab in gameManager.fishPrefabs)
                        {
                            if (hit.collider.CompareTag(fishPrefab.tag) && gameManager.canIBagFish)
                            {
                                GameObject fishToBag = hit.collider.gameObject;

                                gameManager.BagAFish(fishToBag);

                            }
                        }
                    }
                    else if (hit.collider.gameObject.GetComponent<Scr_Starfish>())
                    {
                        foreach (var exoticFishPrefab in gameManager.exoticFishPrefabs)
                        {
                            if (hit.collider.CompareTag(exoticFishPrefab.tag) && gameManager.canIBagFish)
                            {
                                GameObject fishToBag = hit.collider.gameObject;

                                gameManager.BagAStarfish(fishToBag);
                            }
                        }
                    }
                }

                if (structHit.collider != null)
                {
                    if (structHit.collider.gameObject.GetComponent<Scr_StructurePlacementRules>() != null && gameManager.canIRemoveStructures)
                    {
                        Debug.Log(structHit.collider.gameObject);
                        GameObject structureToRemove = structHit.collider.gameObject;
                        gameManager.RemoveStructure(structureToRemove);
                    }


                    Scr_FishFeeder feeder = structHit.collider.GetComponentInParent<Scr_FishFeeder>();
                    if (feeder != null)
                    {
                        if (gameManager.currentFishFoodSelected != null)
                            feeder.SetFoodType(gameManager.currentFishFoodSelected, gameManager.currentFishFoodButtonSelected);
                        else
                            feeder.CycleSpeed();

                        return; // handled
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
            gameManager.EnableElement(hudButtonPanel.GetChild(1).gameObject);
            gameManager.DisableElement(hudButtonPanel.GetChild(2).gameObject);
            gameManager.EnableElement(hudButtonPanel.GetChild(3).gameObject);
            gameManager.DisableElement(hudButtonPanel.GetChild(4).gameObject);
            gameManager.EnableElement(hudButtonPanel.GetChild(5).gameObject);
            gameManager.DisableElement(hudButtonPanel.GetChild(6).gameObject);

            // -----------------------------------------
            // If we are placing a structure, right-click cancels build mode
            // -----------------------------------------
            if (gameManager.currentStructurePrefabSelected != null)
            {
                gameManager.CancelStructurePlacement();
                return;
            }

            // ==============================================
            // 1) RIGHT-CLICKED NOTHING IMPORTANT:
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
            // Deselect Wrench
            if (gameManager.canIRemoveStructures)
            {
                gameManager.DeselectWrench();
            }

            // Hide fish info panel
            infoPanel.Hide();

            // --- CAST TO WORLD ---
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            // ==============================================
            // 1) PRIORITY: STRUCTURES FIRST
            // ==============================================
            foreach (var h in hits)
            {
                if (h.collider == null) continue;

                Scr_FishFeeder feeder = h.collider.GetComponentInParent<Scr_FishFeeder>();
                if (feeder != null)
                {
                    if (gameManager.currentFishFoodSelected != null)
                        feeder.SetFoodType(gameManager.currentFishFoodSelected, gameManager.currentFishFoodButtonSelected);
                    else
                        feeder.CycleSpeed();

                    return; // handled
                }
            }

            // ==============================================
            // 2) NEXT: FISH INFO
            // ==============================================
            foreach (var h in hits)
            {
                if (h.collider == null) continue;

                Debug.Log("GOT TO HERE");

                Scr_Fish fish = h.collider.GetComponentInParent<Scr_Fish>();
                if (fish != null)
                {
                    if (infoPanel.gameObject.activeSelf && infoPanel.currentTarget == fish.transform)
                        infoPanel.Hide();
                    else
                        infoPanel.Show(fish);

                    return;
                }

                Scr_Starfish starfish = h.collider.GetComponentInParent<Scr_Starfish>();
                if (starfish != null)
                {
                    if (infoPanel.gameObject.activeSelf && infoPanel.currentTarget == starfish.transform)
                        infoPanel.Hide();
                    else
                        infoPanel.ShowExotic(starfish);

                    return;
                }
            }



        }
    }
}

