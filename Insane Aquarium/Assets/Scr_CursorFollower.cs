using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CursorFollower : MonoBehaviour
{

    private void FixedUpdate()
    {
        Vector2 cursorPos = Input.mousePosition;
        transform.position = new Vector2(cursorPos.x, cursorPos.y);
    }


}
