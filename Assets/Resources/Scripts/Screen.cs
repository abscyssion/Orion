using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour
{
    protected static bool IsHovering(RectTransform hoverRect, Vector2 cursorPos)
    {
        Vector2 hoverPos = hoverRect.anchoredPosition;
        Vector2 hoverSize = hoverRect.sizeDelta;

        //Points
        Vector2 hoverMin = hoverPos;
        Vector2 hoverMax = hoverPos + hoverSize;

        if (cursorPos.x > hoverMin.x && cursorPos.y > hoverMin.y && cursorPos.x < hoverMax.x && cursorPos.y < hoverMax.y)
            return true;
        else
            return false;
    }
}
