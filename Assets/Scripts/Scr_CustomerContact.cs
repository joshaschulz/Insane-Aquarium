using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Customer/Contact")]
public class Scr_CustomerContact : ScriptableObject
{
    public string contactName;         // Name of the contact
    [TextArea(2, 6)] public string[] dialogueLines; // Dialogue lines

    public int indexToEnableSelection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
