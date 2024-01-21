using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Spawn : MonoBehaviour
{
    public GameObject fish;
    public Vector3 spawnPosition;

    public void SpawnFish()
    {
        Instantiate(fish, spawnPosition, Quaternion.identity);
    }
}
