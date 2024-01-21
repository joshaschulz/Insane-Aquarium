using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Feed : MonoBehaviour
{
    Scr_GameManager gameManager;
    public GameObject foodPellet;

    public int feedingCost;

    private Camera _Camera;
    private void Awake()
    {
        gameManager = GetComponent<Scr_GameManager>();
        _Camera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-Click - Attempt to drop food
        {
            if (gameManager.GetMoneyAmount() >= feedingCost)
            {

                Vector3 mousePixelPos = Input.mousePosition;

                mousePixelPos.z = 20f;

                Vector3 mouseWorldPosition = _Camera.ScreenToWorldPoint(mousePixelPos);

                mouseWorldPosition.z = 0;

                Instantiate(foodPellet, mouseWorldPosition, Quaternion.identity);

                gameManager.SetMoneyAmount(gameManager.GetMoneyAmount() - feedingCost);
            }
            else
            {
                Debug.Log("Insufficient Money to Feed");
            }
        }
    }
}
