using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_TankBounds : MonoBehaviour
{
    public Vector2 size = new Vector2(3f, 2f); // width, height in world units

    [Tooltip("The for-sale sticker GameObject under this tank.")]
    public GameObject forSaleSign; // assign the sticker here

    public Bounds GetBounds()
    {
        return new Bounds(transform.position, size);
    }

    public bool IsForSaleActive
    {
        get { return forSaleSign != null && forSaleSign.activeInHierarchy; }
    }
}
