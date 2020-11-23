using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StarScanScreen : Screen
{
    public GameObject cursor;
        private RectTransform cursorRect;
        private CursorHandler cursorScript;

    public GameObject topContainer;
    public Image topOutline;

    public TextMeshProUGUI typeText;
    public TextMeshProUGUI tempText;

    public TextMeshProUGUI descTypeText;
    public TextMeshProUGUI descText;

    public RectTransform buttonRect;

    public Image buttonBgContainerImg;
        private const float buttonColChDelay = 0.15f;
        private Color buttonColDef;
        [SerializeField] private Color buttonColHover;
        [SerializeField] private Color buttonColPress;
        [SerializeField] private Color buttonColWrong;
    public GameObject buttonBackground;
        private Image buttonBgImg;
            [SerializeField] private Color buttonBgColMin;
            [SerializeField] private Color buttonBgColMax;
        private RectTransform buttonBgRect;
            [SerializeField] private float buttonBgWidthMax = 0;
    public TextMeshProUGUI buttonText;

    private StarVisScreen starVisScreen;

    public bool systemScanned { get; private set; }
    private bool systemScanning;

    private void Awake()
    {
        cursorRect = cursor.GetComponent<RectTransform>();
        cursorScript = cursor.GetComponent<CursorHandler>();

        buttonBgImg = buttonBackground.GetComponent<Image>();
        buttonBgRect = buttonBackground.GetComponent<RectTransform>();

        buttonColDef = buttonBgContainerImg.color;

        starVisScreen = GameObject.Find("Star Visualisation Screen").GetComponent<StarVisScreen>();
    }

    private void Update()
    {
        if (cursorScript.active)
        {
            if (!systemScanning && Screen.IsHovering(buttonRect, cursorRect.anchoredPosition))
            {
                if (!buttonChColor)
                {
                    buttonBgContainerImg.color = buttonColHover;

                    if(Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(ChangeButtonColor(buttonBgContainerImg, buttonColPress, buttonColDef));

                        if (!Scan())
                            StartCoroutine(ChangeButtonColor(buttonBgContainerImg, buttonColWrong, buttonColDef));

                    }
                }
            }
            else
            {
                buttonBgContainerImg.color = buttonColDef;
            }
        }
    }

    private bool Scan()
    {
        if (!systemScanned && !systemScanning && !JumpHandler.jumping)
        {
            StartCoroutine(Scanning(Ship.scannerEff));
            return true;
        }
        else
            return false;
    }
    private IEnumerator Scanning(float time)
    {
        systemScanning = true;
        buttonText.text = "SCANNING";

        float buttonBgWidth = 0;

        float timeElapsed = 0;
        float timePerc = 0;
        while(timeElapsed < time)
        {
            timePerc = timeElapsed / time;

            buttonBgWidth = timePerc * buttonBgWidthMax;
            buttonBgRect.sizeDelta = new Vector2(buttonBgWidth, 0);

            buttonBgImg.color = Color.Lerp(buttonBgColMin, buttonBgColMax, timePerc);

            timeElapsed += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        starVisScreen.DrawVisualisations(World.GetSystem());
        starVisScreen.ChangeScreen(true);   

        Star currStar = World.GetSystem().star;

        topContainer.SetActive(true);
        topOutline.enabled = false;

        Write(currStar.type, typeText);
        yield return new WaitForSeconds(1.1f);
        Write(currStar.temperature + "K", tempText);
        yield return new WaitForSeconds(0.8f);
        Write(currStar.type, descTypeText);
        yield return new WaitForSeconds(0.6f);
        Write(currStar.description, descText);

        systemScanned = true;

        systemScanning = false;
        buttonText.text = "SCANNED";
    }
    private void Write(string text, TextMeshProUGUI tmp)
    {
        StartCoroutine(Writing(text, tmp));
    }

    private IEnumerator Writing(string text, TextMeshProUGUI tmp)
    {
        const float delay = 0.05f;

        string textOut = "";
        for(int i = 0; i <= text.Length-1; i++)
        {
            yield return new WaitForSeconds(delay);
            textOut += text[i];
            tmp.text = textOut;
        }
    }

    public void ResetScanner()
    {
        systemScanned = false;

        buttonText.text = "SCAN";
        buttonBgImg.color = buttonBgColMin;
        buttonBgRect.sizeDelta = new Vector2(0, 0);

        typeText.text = "";
        tempText.text = "";
        descTypeText.text = "";
        descText.text = "";

        topContainer.SetActive(false);
        topOutline.enabled = true;

        starVisScreen.DestroyVisualisations();
        starVisScreen.ChangeScreen(false);
    }

    bool buttonChColor;
    private IEnumerator ChangeButtonColor(Image image, Color colorCh, Color colorDef)
    {
        buttonChColor = true;

        image.color = colorCh;

        yield return new WaitForSeconds(buttonColChDelay);

        image.color = colorDef;

        buttonChColor = false;
    }
}
