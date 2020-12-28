using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static World;
using static PlanetJumpHandler;


public class PlanetJumpScreen : Screen
{
    public PlanetJumpHandler planetJumpHandler;

    private Screen scr;

    public GameObject onScreen;
    public GameObject offScreen;

    public GameObject cursor;
        private RectTransform cursorRect;
        private CursorHandler cursorScript;

    public GameObject parentObj;
    public RectTransform virtParentRect;

    public Sprite boxTexture;

    public RectTransform buttonRect;
    public Image buttonBackground;
        private Color buttonColDef;

    public TMP_FontAsset font;

    private Color outlineDefColor = Color.grey;
    private Color outlineSelColor = Color.white;
    private Color outlineLockColor = Color.yellow;

    private bool locked = false;
    private int lockedID = 0;

    private List<RectTransform> outlineRects;
    private List<Image> outlineImages;

    private void Awake()
    {
        scr = gameObject.AddComponent<Screen>();

        cursorRect = cursor.GetComponent<RectTransform>();
        cursorScript = cursor.GetComponent<CursorHandler>();

        buttonColDef = buttonBackground.color;

        RefreshScreen(GetLocation());
    }

    
    private void Update()
    {
        if (cursorScript.active)
        {
            Vector2 cursorPos = cursorRect.anchoredPosition;

            if (IsHovering(buttonRect, cursorPos))
            {
                if (!scr.changingColor)
                {
                    buttonBackground.color = buttonColHover;

                    if (Input.GetMouseButtonDown(0))
                    {
                        StopAllCoroutines();

                        if (currFlight.possible)
                        {
                            scr.ChangeColor(buttonBackground, buttonColClick, buttonColDef);
                            planetJumpHandler.Jump();
                        }
                        else
                            scr.ChangeColor(buttonBackground, buttonColInvalid, buttonColDef);
                    }
                }
            }
            else
            {
                buttonBackground.color = buttonColDef;

                int i = 0;
                foreach (RectTransform rect in outlineRects)
                {
                    if (locked && i == lockedID)
                    {
                        outlineImages[i].color = outlineLockColor;
                    }
                    else
                    {
                        if (Screen.IsHovering(rect, cursorPos))
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                locked = true;
                                lockedID = i;
                                SetFlight(new Flight(new Location(GetLocation().sys.GetSysObjs()[i])));
                            }

                            outlineImages[i].color = outlineSelColor;
                            break;
                        }
                        else
                            outlineImages[i].color = outlineDefColor;
                    }
                    i++;
                }

            }
        }
    }
    public void ChangeScreen(bool onScr)
    {
        onScreen.SetActive(onScr);
        offScreen.SetActive(!onScr);
    }

    public void RefreshScreen(Location loc)
    {
        foreach(Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        const float paddingPerc = 0.05f;

        Vector2 parentDiam = virtParentRect.sizeDelta;// new Vector2(450, 365);
        Vector2 parentPos = virtParentRect.anchoredPosition;// new Vector2(25, 110);

        List<SysObj> sysObjs = loc.sys.GetSysObjs();

        float spacePerObj = parentDiam.y / sysObjs.Count;
        float padding = spacePerObj * paddingPerc;

        outlineRects = new List<RectTransform>();
        outlineImages = new List<Image>();

        int i = 0;
        foreach(SysObj sysObj in sysObjs)
        {
            #region Parent

            GameObject obj = new GameObject(sysObj.type + " Bar");
            obj.transform.SetParent(parentObj.transform);

            RectTransform rect = obj.AddComponent<RectTransform>();
            Screen.RefreshRect(rect);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(parentDiam.x, spacePerObj - padding);
            rect.anchoredPosition = new Vector2(0, (spacePerObj + padding) * (sysObjs.Count - 1 - i)) + parentPos;

            Image img = obj.AddComponent<Image>();
            img.sprite = boxTexture;
            img.type = Image.Type.Sliced;
            img.color = outlineDefColor;

            outlineRects.Add(rect);
            outlineImages.Add(img);

            #endregion

            Vector2 textSize = new Vector2(0, rect.sizeDelta.y / 2);
            float textPadding = textSize.y * 0.13f;

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
            topText.alignment = TextAlignmentOptions.Center;
            topText.margin = new Vector4(0, textPadding, 0, 0);
            topText.overflowMode = TextOverflowModes.Ellipsis;
            topText.font = font;
            topText.fontSize = topRect.sizeDelta.y * topFontPerc;
            topText.text = topString;
            
            Screen.RefreshRectOffset(topRect);
            topRect.sizeDelta = textSize;

            #endregion

            #region Bottom text
            Flight flight = new Flight(new Location(sysObj));
            string botString;
            if (flight.distance > 0)
                botString = flight.details.distance + " PU from you, " + flight.details.fuel + " fuel.";
            else
                botString = "<u>You are here.</u>";

            const float botFontPerc = 0.5f;
            GameObject botObj = new GameObject("Bot");
            botObj.transform.SetParent(obj.transform);

            RectTransform botRect = botObj.AddComponent<RectTransform>();
            Screen.RefreshRect(botRect);
            botRect.anchorMin = new Vector2(0, 0);
            botRect.anchorMax = new Vector2(1, 0);
            botRect.pivot = new Vector2(0.5f, 0);

            TextMeshProUGUI botText = botObj.AddComponent<TextMeshProUGUI>();
            botText.alignment = TextAlignmentOptions.Center;
            botText.margin = new Vector4(0, 0, 0, textPadding);
            botText.overflowMode = TextOverflowModes.Ellipsis;
            botText.font = font;
            botText.fontSize = botRect.sizeDelta.y * botFontPerc;
            botText.text = botString;

            Screen.RefreshRectOffset(botRect);
            botRect.sizeDelta = textSize;

            #endregion

            i++;
        }
    }
}
