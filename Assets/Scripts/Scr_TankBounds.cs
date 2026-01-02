using UnityEngine;

public class Scr_TankBounds : MonoBehaviour
{
    public Vector2 size = new Vector2(3f, 2f);

    // runtime reference (set automatically when a sign is placed inside this tank)
    [HideInInspector] public Scr_ForSaleSticker activeForSaleSticker;

    public Bounds GetBounds()
    {
        return new Bounds(transform.position, size);
    }

    public bool IsForSaleActive
    {
        get { return activeForSaleSticker != null && activeForSaleSticker.gameObject.activeInHierarchy; }
    }

    public void RegisterForSaleSticker(Scr_ForSaleSticker sign)
    {
        activeForSaleSticker = sign;
    }

    public void UnregisterForSaleSticker(Scr_ForSaleSticker sign)
    {
        if (activeForSaleSticker == sign) activeForSaleSticker = null;
    }
}
