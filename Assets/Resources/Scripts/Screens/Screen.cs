using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour
{
    protected Color buttonColHover;
    protected Color buttonColClick;
    protected Color buttonColInvalid;
    protected float buttonColorDelay;

    private void Awake()
    {
        buttonColHover = new Color { r = 0, g = 255, b = 0, a = 0.3f };
        buttonColClick = new Color { r = 255, g = 255, b = 255, a = 0.3f };
        buttonColInvalid = new Color { r = 255, g = 0, b = 0, a = 0.3f };
        buttonColorDelay = 0.2f;
    }

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

    protected static void RefreshRect(RectTransform rect)
    {
        rect.sizeDelta = new Vector2(0, 0);
        rect.localScale = new Vector3(1, 1, 1);
        rect.localPosition = new Vector3(0, 0, 0);
        rect.localEulerAngles = new Vector3(0, 0, 0);
    }

    protected static void RefreshRectOffset(RectTransform rect)
    {
        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);
    }
}
