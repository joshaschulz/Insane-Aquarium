using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_StarfishDiagramHover : MonoBehaviour
{
    private Scr_FishInfoPanel panel;
    private bool isHovering = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        panel = FindObjectOfType<Scr_FishInfoPanel>();

        Debug.Log(panel.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        // Use the camera that actually renders the diagram
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        // check if mouse is inside THIS object's collider
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        bool nowHovering = col.OverlapPoint(mouseWorld);

        if (nowHovering && !isHovering)
        {
            isHovering = true;
            Debug.Log("HOVERED");
            panel.ShowHoverNum(gameObject);
        }
        else if (!nowHovering && isHovering)
        {
            isHovering = false;
            panel.HideHoverNum();
        }
    }


    private void OnMouseEnter()
    {
        Debug.Log("HOVERED THIS");
        panel.ShowHoverNum(gameObject);
    }
    private void OnMouseExit()
    {
        Debug.Log("HOVERED THIS NOT");
        panel.HideHoverNum();
    }
}
