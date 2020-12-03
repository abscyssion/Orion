using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetJumpScreen : Screen
{
    public GameObject infoScreen;
    public GameObject jumpingScreen;

    public GameObject cursor;
        private RectTransform cursorRect;
        private CursorHandler cursorScript;

    public RectTransform parentRect;

    public Sprite boxTexture;

    public RectTransform buttonRect;
    public Image buttonBackground;
        private Color buttonColDef;
        private bool changingColor = false;

    public TMP_FontAsset font;

    private void Awake()
    {
        cursorRect = cursor.GetComponent<RectTransform>();
        cursorScript = cursor.GetComponent<CursorHandler>();

        buttonColDef = buttonBackground.color;

        DrawBars();
    }

    
    private void Update()
    {
        if (cursorScript.active)
        {
            Vector2 cursorPos = cursorRect.anchoredPosition;

            if (Screen.IsHovering(buttonRect, cursorPos))
            {
                if (!changingColor)
                {
                    buttonBackground.color = buttonColHover;

                    if (Input.GetMouseButtonDown(0))
                    {
                        StopAllCoroutines();

/*                        if (sysJumpHandler.Jump())
                            StartCoroutine(ChangeButtonColor(buttonBackground, buttonColClick, buttonColDef));
                        else
                            StartCoroutine(ChangeButtonColor(buttonBackground, buttonColInvalid, buttonColDef));*/
                    }
                }
            }
            else
            {
                buttonBackground.color = buttonColDef;
            }
        }
    }

    private void DrawBars()
    {
        const float paddingPerc = 0.05f;
        Vector2 parentDiam = parentRect.sizeDelta;

        List<PlanetJumpHandler.SysObj> sysObjs = PlanetJumpHandler.GetSysObjects();

        float spacePerObj = parentDiam.y / sysObjs.Count;
        float padding = spacePerObj * paddingPerc;

        int i = 0;
        foreach(PlanetJumpHandler.SysObj sysObj in sysObjs)
        {
            #region Parent

            GameObject obj = new GameObject(sysObj.type + " Bar");
            obj.transform.SetParent(parentRect.transform);

            RectTransform rect = obj.AddComponent<RectTransform>();
            Screen.RefreshRect(rect);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(parentDiam.x, spacePerObj - padding);
            rect.anchoredPosition = new Vector2(0, (spacePerObj + padding) * -i);

            Image img = obj.AddComponent<Image>();
            img.sprite = boxTexture;
            img.type = Image.Type.Sliced;

            #endregion

            #region Top text

            string topString = "<color=yellow>'" + sysObj.name + "'</color>, " + sysObj.type;

            const float topFontPerc = 0.5f;
            GameObject topObj = new GameObject("Top");
            topObj.transform.SetParent(obj.transform);

            RectTransform topRect = topObj.AddComponent<RectTransform>();
            Screen.RefreshRect(topRect);
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.pivot = new Vector2(0.5f, 1);

            TextMeshProUGUI topText = topObj.AddComponent<TextMeshProUGUI>();
            topText.fontStyle = FontStyles.Bold;
            topText.alignment = TextAlignmentOptions.BottomGeoAligned;
            topText.font = font;
            topText.fontSize = topRect.sizeDelta.y * topFontPerc;
            topText.text = topString;
            
            Screen.RefreshRectOffset(topRect);
            topRect.sizeDelta = new Vector2(0, rect.sizeDelta.y / 2);

            #endregion

            #region Bottom text
            PlanetFlight flight = PlanetJumpHandler.GenerateFlight(sysObj);
            string botString;
            if (flight.distance > 0)
                botString = flight.details.distance + " AU from you, " + flight.details.fuel + " fuel.";
            else
                botString = "You are here.";

            const float botFontPerc = 0.5f;
            GameObject botObj = new GameObject("Bot");
            botObj.transform.SetParent(obj.transform);

            RectTransform botRect = botObj.AddComponent<RectTransform>();
            Screen.RefreshRect(botRect);
            botRect.anchorMin = new Vector2(0, 0);
            botRect.anchorMax = new Vector2(1, 0);
            botRect.pivot = new Vector2(0.5f, 0);

            TextMeshProUGUI botText = botObj.AddComponent<TextMeshProUGUI>();
            botText.alignment = TextAlignmentOptions.TopGeoAligned;
            botText.font = font;
            botText.fontSize = botRect.sizeDelta.y * botFontPerc;
            botText.text = botString;

            Screen.RefreshRectOffset(botRect);
            botRect.sizeDelta = new Vector2(0, rect.sizeDelta.y / 2);

            #endregion

            i++;
        }
    }
}
