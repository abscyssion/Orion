using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    private static StarVisScreen starVisScreen;
    private static StarScanScreen starScanScreen;
    private static SysJumpScreen sysJumpScreen;
    private static   PlanetJumpScreen planetJumpScreen;

    protected static Color buttonColHover;
    protected static Color buttonColClick;
    protected static Color buttonColInvalid;
    private static float buttonColorDelay;
    
    private void Awake()
    {
        starVisScreen = GameObject.Find("Star Visualisation Screen").GetComponent<StarVisScreen>();
        starScanScreen = GameObject.Find("Star Scan Screen").GetComponent<StarScanScreen>();
        sysJumpScreen = GameObject.Find("System Jump Screen").GetComponent<SysJumpScreen>();
        planetJumpScreen = GameObject.Find("Planet Jump Screen").GetComponent<PlanetJumpScreen>();

        buttonColHover = new Color(1, 1, 1, 0.3f);
        buttonColClick = new Color(0, 1, 0, 0.3f);
        buttonColInvalid = new Color(1, 0, 0, 0.3f);
        buttonColorDelay = 0.2f;
    }

    protected static bool IsHovering(RectTransform hoverRect, Vector2 cursorPos)
    {
        if (hoverRect.anchorMin == hoverRect.anchorMax && hoverRect.anchorMax == hoverRect.pivot)
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
        else
        {
            Debug.LogWarning("Error with IsHovering function. All anchors and the pivot must be the same.");
            return false;
        }
    }

/*    protected static void ConvertToZeroAnchor(RectTransform rect)
    {
        Vector2 localPosPre = rect.localPosition;
        rect.anchorMax = new Vector2(0, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.localPosition = localPosPre - new Vector2(rect.sizeDelta.x / 2, 0);
    }*/

    public static void ChangeScreensAll(bool onScreen)
    {
        starVisScreen.ChangeScreen(onScreen);
        starScanScreen.ChangeScreen(onScreen);
        sysJumpScreen.ChangeScreen(onScreen);
        planetJumpScreen.ChangeScreen(onScreen);
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

    public bool changingColor = false;
    public void ChangeColor(Image image, Color colorCh, Color colorDef)
    {
        StartCoroutine(ChangingColor(image, colorCh, colorDef));
    }

    private IEnumerator ChangingColor(Image image, Color colorCh, Color colorDef)
    {
        changingColor = true;

        image.color = colorCh;

        yield return new WaitForSeconds(buttonColorDelay);

        image.color = colorDef;

        changingColor = false;
    }
}
