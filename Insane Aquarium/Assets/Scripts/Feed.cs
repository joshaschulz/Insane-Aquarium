using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feed : MonoBehaviour
{

    public GameObject foodPellet;

    private Camera _Camera;
    private void Awake()
    {
        _Camera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePixelPos = Input.mousePosition;

            mousePixelPos.z = 20f;

            Vector3 mouseWorldPosition = _Camera.ScreenToWorldPoint(mousePixelPos);

            mouseWorldPosition.z = 0;

            Instantiate(foodPellet, mouseWorldPosition, Quaternion.identity);
        }
    }
}
