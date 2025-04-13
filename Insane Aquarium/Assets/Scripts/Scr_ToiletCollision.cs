using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ToiletCollision : MonoBehaviour
{
    private Scr_GameManager gameManager;
    public Scr_Fishing fishingRod;

    private void Start()
    {
        gameManager = Scr_GameManager.GMinstance;
        Debug.Log(fishingRod);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            if (!fishingRod.StopFishing)
            {
                fishingRod.FishEscape();
            }
        }
    }
}
