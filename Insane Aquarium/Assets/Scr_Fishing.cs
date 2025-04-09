using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Fishing : MonoBehaviour
{
    public Sprite leftRodSprite, rightRodSprite;
    public float rotationSpeed;

    private SpriteRenderer spriteRenderer;
    private float screenMiddleX;
    private bool isLeft; // Track the current state to avoid unnecessary updates

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        screenMiddleX = Screen.width / 2; // Get middle X point of the screen

        // Initialize sprite based on starting position
        isLeft = Input.mousePosition.x < screenMiddleX;
        Debug.Log("initial rod is left? " + isLeft);
        if (isLeft)
        {
            spriteRenderer.sprite = leftRodSprite;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            spriteRenderer.sprite = rightRodSprite;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        ScrollWheelReel();
        RodFollowCursorX();
        CheckToFlipRod();
    }

    private void ScrollWheelReel()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // Rotate around the Z-axis (adjust axis if needed)
            transform.GetChild(0).Rotate(0, 0, scrollInput * rotationSpeed * 10000 * Time.deltaTime);
        }
    }

    private void RodFollowCursorX()
    {
        Vector3 mousePosition = Input.mousePosition; // Get cursor position in screen space
        mousePosition.z = Camera.main.nearClipPlane; // Ensure it's in the correct depth for conversion

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert to world space

        // Update only the X position, keep Y and Z unchanged
        transform.position = new Vector3(worldPosition.x, transform.position.y, transform.position.z);
    }
    private void CheckToFlipRod()
    {
        float mouseX = Input.mousePosition.x; // Get cursor X position

        if (mouseX < screenMiddleX && !isLeft)
        {
            spriteRenderer.sprite = leftRodSprite; // Use left rod when mouse is on the left
            isLeft = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (mouseX >= screenMiddleX && isLeft)
        {
            spriteRenderer.sprite = rightRodSprite; // Use right rod when mouse is on the right
            isLeft = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
