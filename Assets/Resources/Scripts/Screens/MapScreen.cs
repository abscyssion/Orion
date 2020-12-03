using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : Screen
{
    public SysJumpScreen sysJumpScreen;

    public GameObject cursorTop;
    private RectTransform cursorTopRect;
    private CursorHandler cursorTopScript;

    public GameObject cursorBot;
    private RectTransform cursorBotRect;
    private CursorHandler cursorBotScript;

    public Animator lockAnim;

    public TextMeshProUGUI displayText;

    public RectTransform parentRect;
    public RectTransform starsContainer;

    private static Sprite[] starSprites;

    private static Image[,] starContainersImg;
    private static RectTransform[,] starRects;
    public GameObject selectHover; private RectTransform hoverRect;
    public RectTransform lockRect;
    public RectTransform shipCursorRect;

    private static Vector2 cellSize;
    private static Vector2 randomMagnitude;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float cellColorAlpha = 0;

    public RectTransform buttonRect;
    public Image buttonBackground;
    private Color buttonColDef;


    private Vector2Int lockPos;

    List<string> starAllTypes = new List<string>();

    private void Start()
    {
        Vector2Int mapSize = World.mapSize;

        cursorTopRect = cursorTop.GetComponent<RectTransform>();
        cursorTopScript = cursorTop.GetComponent<CursorHandler>();

        cursorBotRect = cursorBot.GetComponent<RectTransform>();
        cursorBotScript = cursorBot.GetComponent<CursorHandler>();

        starContainersImg = new Image[mapSize.x, mapSize.y];
        starRects = new RectTransform[mapSize.x, mapSize.y];

        hoverRect = selectHover.GetComponent<RectTransform>();

        cellSize = new Vector2
        {
            x = parentRect.rect.width / mapSize.x,
            y = parentRect.rect.height / mapSize.y
        };

        buttonColDef = buttonBackground.color;

        GenerateMap();

        DisplaySystemName(World.GetSystem());

        Vector2Int starCell = World.GetSystem().cellCoords;
        Vector2 starPos = starCell * cellSize + starRects[starCell.x, starCell.y].anchoredPosition;

        shipCursorRect.anchoredPosition = starPos;
        LockSystem(starCell, starPos);
    }

    public static Vector2 GetSystemPos(Vector2Int coords)
    {
        Vector2 systemPos = coords * cellSize + starRects[coords.x, coords.y].anchoredPosition;
        return systemPos;
    }

    bool mousePrev = false;
    Vector2Int cursorCellPrev = new Vector2Int();
    bool buttonChColor = false;
    private void Update()
    {
        if (cursorTopScript.active)
        {
            selectHover.SetActive(true);

            Vector2 cursorPos = cursorTopRect.anchoredPosition;

            Vector2Int cursorCell = new Vector2Int
            {
                x = (int)(cursorPos.x / cellSize.x),
                y = (int)(cursorPos.y / cellSize.y)
            };

            Vector2 selectPos = cursorCell * cellSize + starRects[cursorCell.x, cursorCell.y].anchoredPosition;
             
            if (cursorTopScript.changed)
            {
                hoverRect.anchoredPosition = selectPos;
            }

            bool mouse = Input.GetMouseButton(0);
            if (mouse && !mousePrev && cursorCell != cursorCellPrev)
            {
                if (!displayingSystemName)
                {
                    LockSystem(cursorCell, selectPos);

                    cursorCellPrev = cursorCell;
                }
            }
            mousePrev = mouse;
        }
        else
        {
            selectHover.SetActive(false);

            if (cursorBotScript.active)
            {
                Vector2 cursorPos = cursorBotRect.anchoredPosition;
                if (Screen.IsHovering(buttonRect, cursorPos))
                {
                    if (!buttonChColor)
                    {
                        buttonBackground.color = buttonColHover;

                        if (Input.GetMouseButtonDown(0))
                        {
                            //StopAllCoroutines();
                            StartCoroutine(ChangeButtonColor(buttonBackground, buttonColClick, buttonColDef));

                            ToggleSecDisplay();
                        }
                    }
                }
                else
                {
                    buttonBackground.color = buttonColDef;
                }
            }
        }
    }

    private void LockSystem(Vector2Int cursorCell, Vector2 cursorPos)
    {
        if (!Flight.jumping)
        {
            World.Sys destSys = World.GetSystem(cursorCell);

            SysJumpHandler.SetFlight(SysJumpHandler.GenerateFlight(destSys));
            DisplaySystemName(destSys);
            sysJumpScreen.RefreshScreen();

            lockAnim.SetTrigger("Trigger");
            lockRect.anchoredPosition = cursorPos;
        }
    }

    private bool displayingSystemName = false;
    private void DisplaySystemName(World.Sys sys)
    {
        string str = sys.name + " [" + (sys.cellCoords.x + 1) + ", " + (sys.cellCoords.y + 1) + "]";

        StartCoroutine(DisplayingSystemName(str));
    }

    private IEnumerator DisplayingSystemName(string str)
    {
        displayingSystemName = true;

        string stringOut = "";

        const float delay = 0.05f;

        foreach(char ch in str)
        {
            stringOut += ch;
            displayText.SetText(stringOut);

            yield return new WaitForSeconds(delay);
        }

        displayingSystemName = false;
    }

    private void GenerateMap()
    {
        GameObject[,] mapStars = new GameObject[World.mapSize.x, World.mapSize.y];

        Vector3 starContainerPos = new Vector3(0, 0, 0);
        for (int x = 0; x <= World.mapSize.x - 1; x++)
        {
            starContainerPos.x = cellSize.x * x;

            for (int y = 0; y <= World.mapSize.y - 1; y++)
            {
                starContainerPos.y = cellSize.y * y;

                float systemSec = World.GetSystem(new Vector2Int(x, y)).security;

                GameObject starContainer = GenerateStarContainer(x, y, starContainerPos, systemSec);
                starContainersImg[x, y] = starContainer.GetComponent<Image>();

                mapStars[x, y] = GenerateStar(x, y, starContainer);    

                randomMagnitude = new Vector2
                {
                    x = (cellSize.x / 2),
                    y = (cellSize.y / 2)
                };
            }
        }
    }

    private GameObject GenerateStar(int x, int y, GameObject starContainer)
    {
        GameObject star = new GameObject("Star");
        star.transform.SetParent(starContainer.transform);

        RectTransform starRect = star.AddComponent<RectTransform>();

        starRects[x, y] = starRect;

        Vector2 systemCoords = World.GetSystem(new Vector2Int(x, y)).localCoords;

        Vector2 localCoords = new Vector2
        {
            x = Mathf.Abs(systemCoords.x * 2),
            y = Mathf.Abs(systemCoords.y * 2),
        };

        Vector2 cellMultiplier = new Vector2
        {
            x = cellSize.x / World.cellSize.x,
            y = cellSize.y / World.cellSize.y
        };

        Vector2 starPos = new Vector2
        {
            x = localCoords.x * cellMultiplier.x,
            y = localCoords.y * cellMultiplier.y
        };

        Screen.RefreshRect(starRect);
        starRect.anchoredPosition = starPos;
        starRect.sizeDelta = new Vector2(10, 10);
        starRect.anchorMin = new Vector2(0, 0);
        starRect.anchorMax = new Vector2(0, 0);
        starRect.pivot = new Vector2(0.5f, 0.5f);

        //Sprite
        star.AddComponent<CanvasRenderer>();
        Image starImage = star.AddComponent<Image>();

        starImage.sprite = World.GetSystem(new Vector2Int(x, y)).star.sprite;

        return star;
    }

    private GameObject GenerateStarContainer(int x, int y, Vector3 starContainerPos, float systemSecurity)
    {
        GameObject starContainer = new GameObject("Star [" + (x + 1) + ", " + (y + 1) + "] Container");
        RectTransform starContainerRect = starContainer.AddComponent<RectTransform>();
        Image starContainerImg = starContainer.AddComponent<Image>();

        starContainer.transform.SetParent(starsContainer.transform);

        starContainerRect.anchorMin = new Vector2(0, 0);
        starContainerRect.anchorMax = new Vector2(0, 0);
        starContainerRect.pivot = new Vector2(0, 0);
        Screen.RefreshRect(starContainerRect);
        starContainerRect.localPosition = starContainerPos;
        starContainerRect.sizeDelta = cellSize;
  
        Color starContainerColor = Color.Lerp(Color.red, Color.green, systemSecurity); starContainerColor.a = cellColorAlpha;
        starContainerImg.color = starContainerColor;
        starContainerImg.enabled = false;

        return starContainer;
    }

    private IEnumerator ChangeButtonColor(Image image, Color colorCh, Color colorDef)
    {
        buttonChColor = true;

        image.color = colorCh;

        yield return new WaitForSeconds(buttonColorDelay);

        image.color = colorDef;

        buttonChColor = false;
    }

    bool secDisplayToggled = false;

    private void ToggleSecDisplay()
    {
        for (int x = 0; x <= World.mapSize.x - 1; x++)
        {
            for (int y = 0; y <= World.mapSize.y - 1; y++)
            {
                starContainersImg[x, y].enabled = !secDisplayToggled;
            }
        }

        secDisplayToggled = !secDisplayToggled;
    }
}
