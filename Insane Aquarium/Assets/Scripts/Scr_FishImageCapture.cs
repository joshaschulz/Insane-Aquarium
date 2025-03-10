using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scr_FishImageCapture : MonoBehaviour
{
    public Camera mainCamera; // Assign the Main Camera
    public int imageSize = 256; // Resolution of captured texture
    private RenderTexture renderTexture;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Transform originalParent;
    private Sprite capturedSprite; // Stores the resulting sprite

    void Start()
    {
        // Create RenderTexture (Prevents the camera from affecting the main screen)
        renderTexture = new RenderTexture(imageSize, imageSize, 24);
    }

    public Sprite GetCapturedSprite()
    {
        return capturedSprite;
    }

    public void CaptureFishImage(GameObject fishPrefab)
    {
        Debug.Log("atleast got here");
        StartCoroutine(CaptureFishAsSprite(fishPrefab));
    }

    public void PrintHI()
    {
        Debug.Log("Hello");
    }

    private IEnumerator CaptureFishAsSprite(GameObject fishPrefab)
    {
        if (fishPrefab == null || mainCamera == null)
        {
            Debug.LogError("Fish prefab or Main Camera is missing!");
            yield break;
        }

        Debug.Log("Got here 1");

        // Instantiate fish
        GameObject fishInstance = Instantiate(fishPrefab);

        // Store Main Camera's original position and rotation
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;
        originalParent = mainCamera.transform.parent; // Store parent if attached to anything

        // Calculate fish bounds to position the camera correctly
        Bounds bounds = GetPrefabBounds(fishInstance);
        mainCamera.transform.position = bounds.center + new Vector3(0, 0, -5); // Adjust as needed
        mainCamera.transform.LookAt(bounds.center);

        Debug.Log("Got here 2");

        // Assign RenderTexture to Main Camera
        mainCamera.targetTexture = renderTexture;

        // Wait for rendering
        yield return new WaitForEndOfFrame();

        // Capture the image
        Texture2D texture = new Texture2D(imageSize, imageSize, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        Debug.Log("Got here 3");


        // Convert to Sprite and store it
        capturedSprite = Sprite.Create(texture, new Rect(0, 0, imageSize, imageSize), new Vector2(0.5f, 0.5f));

        Debug.Log("Captured Sprite Stored!");

        // Restore Main Camera position, rotation, and parent
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.transform.rotation = originalCameraRotation;
        mainCamera.transform.parent = originalParent;

        // Clear RenderTexture to avoid affecting gameplay
        mainCamera.targetTexture = null;

        // Destroy the fish instance
        Destroy(fishInstance);
    }

    private Bounds GetPrefabBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
}
