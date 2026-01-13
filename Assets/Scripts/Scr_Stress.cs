using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Stress : MonoBehaviour
{

    private Scr_Fish fishScr;

    // Start is called before the first frame update
    void Start()
    {
        fishScr = GetComponent<Scr_Fish>();
        //fishScr.SetStressed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
