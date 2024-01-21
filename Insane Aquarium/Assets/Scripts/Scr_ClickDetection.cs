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
                        hit.collider.gameObject.GetComponent<Scr_CoinBehavior>().GetClicked();
                    }
                }
                
                // Clicked on nothing - attempt to feed
                else
                {
                    gameManager.DropFood();
                }
            }
        }
    }
}
