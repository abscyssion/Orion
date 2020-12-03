using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SysJumpScreen : Screen
{
    private SysJumpHandler sysJumpHandler;

    public GameObject jumpingScreen;

    public GameObject infoScreen;
    public GameObject infoSubScreen;
    public GameObject currVisSubScreen;

    public GameObject cursor;
        private RectTransform cursorRect;
        private CursorHandler cursorScript;

    public TextMeshProUGUI systemText;

    public TextMeshProUGUI distText;
    public TextMeshProUGUI timeText;

    public TextMeshProUGUI securityText;

    public TextMeshProUGUI fuelTopText;
    public TextMeshProUGUI fuelBotText;
    public TextMeshProUGUI fuelStatusText;
        public Image fuelStatusOutline;
        public Image fuelStatusBg;

    public RectTransform buttonRect;
    public Image buttonBackground;
    private Color buttonColDef;

    private void Awake()
    {
        cursorRect = cursor.GetComponent<RectTransform>();
        cursorScript = cursor.GetComponent<CursorHandler>();

        sysJumpHandler = GameObject.Find("Logic").GetComponent<SysJumpHandler>();
        buttonColDef = buttonBackground.color;
    }

    bool changingColor = false;
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

                        if (sysJumpHandler.Jump())
                            StartCoroutine(ChangeButtonColor(buttonBackground, buttonColClick, buttonColDef));
                        else
                            StartCoroutine(ChangeButtonColor(buttonBackground, buttonColInvalid, buttonColDef));
                    }
                }
            }
            else
            {
                buttonBackground.color = buttonColDef;
            }
        }
    }

    bool jumping = false;
    public void ToggleScreens()
    {
        if(!jumping)
        {
            jumpingScreen.SetActive(true);
            infoScreen.SetActive(false);
        }
        else
        {
            jumpingScreen.SetActive(false);
            infoScreen.SetActive(true);
        }

        jumping = !jumping;
    }

    private IEnumerator ChangeButtonColor(Image image, Color colorCh, Color colorDef)
    {
        changingColor = true;

        image.color = colorCh;

        yield return new WaitForSeconds(buttonColorDelay);

        image.color = colorDef;

        changingColor = false;
    }

    public void RefreshScreen()
    {
        SystemFlight flight = SysJumpHandler.currFlight;
        World.Sys destSys = flight.destSys;

        string systemString = destSys.name;
        systemText.SetText(systemString);

        if (destSys.cellCoords == World.GetSystem().cellCoords)
        {
            infoSubScreen.SetActive(false);
            currVisSubScreen.SetActive(true);
        }
        else
        {
            infoSubScreen.SetActive(true);
            currVisSubScreen.SetActive(false);


            string distString = flight.details.distance + " light years.";
            distText.SetText(distString);

            string timeString = flight.details.time + " total.";
            timeText.SetText(timeString);

            string securityString = destSys.FormatSec() + " " + SecurityText(destSys.security);
            securityText.SetText(securityString);

            string fuelTopString = Mathf.Round(Ship.fuel) + "/" + Ship.maxFuel;
            fuelTopText.SetText(fuelTopString);

            string fuelBotString = "-" + flight.details.fuel;
            fuelBotText.SetText(fuelBotString);

            string fuelStatusString;
            Color fuelStatusColor;

            if (Ship.fuel >= flight.fuel)
            {
                fuelStatusString = "OK";
                fuelStatusColor = Color.green;
                fuelBotText.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                fuelStatusString = "BAD";
                fuelStatusColor = Color.red;
                fuelBotText.color = new Color(1, 0, 0, 0.5f);
            }

            fuelStatusText.SetText(fuelStatusString);
            fuelStatusText.color = fuelStatusColor;
            fuelStatusOutline.color = fuelStatusColor;
            fuelStatusColor.a = 0.3f;
            fuelStatusBg.color = fuelStatusColor;
        }
    }

    private string SecurityText(float sec)
    {
        string str = "";

        if(sec >= 0.75f)
        {
            str = "(High Sec)";
        }
        else if(sec < 0.75f && sec >= 0.5f)
        {
            str = "(Medium Sec)";
        }
        else if(sec < 0.5f && sec >= 0.25f)
        {
            str = "Low Sec";
        }
        else if(sec < 0.25f)
        {
            str = "(Dedsec)";
        }

        return str;
    }
}
