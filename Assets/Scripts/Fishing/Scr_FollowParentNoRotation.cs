using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FollowParentNoRotation : MonoBehaviour
{
    private Transform parentTransform;
    private Quaternion initialLocalRotation; // Store initial local rotation

    void Start()
    {
        if (transform.parent != null)
        {
            parentTransform = transform.parent;
            initialLocalRotation = transform.rotation; // Save the original rotation
        }
        else
        {
            Debug.LogWarning("This object has no parent assigned.");
        }
    }

    void LateUpdate()
    {
        if (parentTransform != null)
        {
            // Keep the original local rotation (ignore parent's rotation)
            transform.rotation = initialLocalRotation;
        }
    }
}
