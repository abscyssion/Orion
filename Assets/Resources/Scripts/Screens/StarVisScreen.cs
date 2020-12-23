using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarVisScreen : Screen
{
    private class VisImage
    {
        public RectTransform rect;
        public string type;
        public int id;
        public Color color;
        public Sprite sprite;
        public float size; 
        public float posX; public float virtPosX;
        public bool field;
    }
    
    private World world;

    [SerializeField] private GameObject onScreen;
    [SerializeField] private GameObject offScreen;

    public RectTransform canvasRect;
    public Transform systemParent;

    public Sprite starSprite;
    public Sprite planetSprite;
    public Sprite fieldSprite;
    public Sprite orbitSprite;

    private List<Image> imagesAll;

    private VisImage star;
    private List <VisImage> planets;
    private bool displaying = false;

    private void Awake()
    {
        world = GameObject.Find("Logic").GetComponent<World>();

        imagesAll = new List<Image>();

/*        DrawVisualisations(World.GetSystem());
        ChangeScreen();*/
    }

    const float baseOrbitSpeed = 800; 
    private void Update()
    {
        if (displaying)
        {
            foreach (VisImage sattelite in planets)
            {
                if (!sattelite.field)
                {
                    float orbitSpeed = baseOrbitSpeed / sattelite.posX;
                    sattelite.rect.RotateAround(star.rect.position, Vector3.right, Time.deltaTime * orbitSpeed);
                }
            }
        }
    }

    public void DestroyVisualisations()
    {
        displaying = false;

        foreach (Transform child in systemParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void DrawVisualisations(World.Sys sys)
    {
        Vector2 canvasDiam = canvasRect.sizeDelta;

        star = new VisImage();
        planets = new List<VisImage>();

        #region Init
        //Star
        World.StarObj starObj = sys.star;

        star.type = starObj.type;
        star.id = starObj.id;
        star.color = starObj.visColor;
        star.sprite = starSprite;
        star.size = starObj.visSize;

        //Planets
        foreach(World.PlanetObj planet in sys.planets)
        {
            Sprite sprite;
            if (!planet.field)
                sprite = planetSprite;
            else
                sprite = fieldSprite;

            VisImage planetVis = new VisImage
            {
                type = planet.type,
                id = planet.id,
                color = planet.visColor,
                sprite = sprite,
                field = planet.field
            };

            planets.Add(planetVis);
        }
        #endregion

        #region Positions
        float paddingFromStar = (star.size / 2) * 1.5f;

        star.posX = 0;

        float planetsTotalWidth = 0;
        foreach (VisImage planet in planets)
            planetsTotalWidth += planet.size;
        float canvasTotal = (canvasDiam.x / 2) - paddingFromStar;

        float planetsTotalSpace = canvasTotal - planetsTotalWidth;

        float planetsSpace = planetsTotalSpace / planets.Count;


        for(int i = 0; i <= planets.Count - 1; i++)
        {
            VisImage planet = planets[i];

            float virtPosX = ((planet.size + planetsSpace) * i) + paddingFromStar;
            planet.virtPosX = virtPosX;

            if (virtPosX > canvasDiam.x / 2)
                virtPosX = (canvasDiam.x / 2) - planet.size;

            if (!planet.field)
                planet.posX = virtPosX;
            else
                planet.posX = 0;
        }

        #endregion

        #region Sizes
        for(int i = 0; i <= planets.Count - 1; i++)
        {
            VisImage planetVis = planets[i];
            World.PlanetObj planet = World.GetSystem().planets[i];

            if (!planetVis.field)
                planetVis.size = planet.visSize;
            else
                planetVis.size = planetVis.virtPosX * 2;
        }
        #endregion

        #region Render
        DrawVisualisation(star);

        foreach (VisImage planet in planets)
        {
            DrawVisualisation(planet);
           
            if(!planet.field)
                DrawOrbit(planet);

            //Set a random orbit point
            planet.rect.RotateAround(star.rect.position, Vector3.right, Random.Range(0, 360));
        }
        #endregion

        displaying = true;
    }

    private void DrawVisualisation(VisImage vis)
    {
        GameObject visObj = new GameObject(vis.type);
        visObj.transform.SetParent(systemParent);

        RectTransform visRect = visObj.AddComponent<RectTransform>();
        Screen.RefreshRect(visRect);
        visRect.anchorMin = new Vector2 { x = 0.5f, y = 0.5f };
        visRect.anchorMax = new Vector2 { x = 0.5f, y = 0.5f };
        visRect.pivot = new Vector2 { x = 0.5f, y = 0.5f };

        visRect.sizeDelta = new Vector2 { x = vis.size, y = vis.size};
        visRect.anchoredPosition = new Vector2 { x = vis.posX, y = 0 };
        vis.rect = visRect;

        visObj.AddComponent<CanvasRenderer>();
        Image visImg = visObj.AddComponent<Image>();
        visImg.sprite = vis.sprite;
        visImg.color = vis.color;

        imagesAll.Add(visImg);
    }

    private void DrawOrbit(VisImage vis)
    {
        float virtPosX = vis.virtPosX;
        string name = vis.type + "'s Orbit";

        float correction = 1.05f;

        Vector2 size = new Vector2
        {
            x = virtPosX * 2 * correction,
            y = virtPosX * 2 * correction,
        };

        GameObject orbObj = new GameObject();
        orbObj.transform.SetParent(systemParent);
        orbObj.transform.name = name;

        RectTransform orbRect = orbObj.AddComponent<RectTransform>();
        orbRect.localScale = new Vector3(1, 1, 1);
        orbRect.localPosition = new Vector3(0, 0, 0);
        orbRect.localEulerAngles = new Vector3(0, 0, 0);
        orbRect.anchorMin = new Vector2 { x = 0.5f, y = 0.5f };
        orbRect.anchorMax = new Vector2 { x = 0.5f, y = 0.5f };
        orbRect.pivot = new Vector2 { x = 0.5f, y = 0.5f };

        orbRect.sizeDelta = size;

        orbRect.anchoredPosition = new Vector2
        {
            x = 0,
            y = 0
        };

        float orbitDarkenBy = 0.15f; float orbitAlpha = 0.25f;
        Color orbColor = vis.color - new Color(orbitDarkenBy, orbitDarkenBy, orbitDarkenBy);
        orbColor.a = orbitAlpha;

        Image orbImg = orbObj.AddComponent<Image>();
        orbImg.sprite = orbitSprite;
        orbImg.color = orbColor;
    }


    public void ChangeScreen(bool onScr)
    {
        onScreen.SetActive(onScr);
        offScreen.SetActive(!onScr);
    }

    private IEnumerator FadeInVis()
    {
        List<Color> colorsBefore = new List<Color>();
        List<Color> colorsAfter = new List<Color>();

        foreach(Image image in imagesAll)
        {
            Color colorBefore = image.color;
            colorsBefore.Add(colorBefore);

            Color colorAfter = colorBefore; colorAfter.a = 0;
            colorsAfter.Add(colorAfter);

            image.color = colorAfter;
        }

        const float delay = 0.05f;
        float percent = 0.0f;
        while(percent < 1.0f)
        {
            for(int i = 0; i <= imagesAll.Count - 1; i++)
            {
                imagesAll[i].color = Color.Lerp(colorsAfter[i], colorsBefore[i],  percent);
            }

            percent += 0.05f;
            yield return new WaitForSeconds(delay);
        }
    }
}
