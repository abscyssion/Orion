using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarVisScreen : MonoBehaviour
{
    private class VisImage
    {
        public RectTransform rect;
        public string name;
        public Color color;
        public Sprite sprite;
        public Vector2 size;
        public Vector2 pos;
    }
    
    private World world;

    public GameObject infoScreen;
    public GameObject mainScreen;

    public RectTransform canvasRect;
    public Transform systemParent;

    public Sprite starSprite;
    public Sprite[] planetSprites;

    public Color[] starColors;
    public Vector2[] starSizes;

    public Color[] planetColors;
    public Vector2[] planetSizes;

    private VisImage star;
    private VisImage[] planets;
    private bool orbiting = false;

    private void Awake()
    {
        world = GameObject.Find("Logic").GetComponent<World>();

        DrawVisualisation(World.GetSystem());
    }

    const float baseOrbitSpeed = 800; 
    private void Update()
    {
        if (orbiting)
        {
            foreach (VisImage sattelite in planets)
            {
                float orbitSpeed = baseOrbitSpeed / sattelite.pos.x;

                //Debug.Log("[" + sattelite.name + "]: " + orbitSpeed);

                sattelite.rect.RotateAround(star.rect.position, Vector3.right, Time.deltaTime * orbitSpeed);
            }
        }

        
    }

    private void DrawVisualisation(World.Sys sys)
    {
        Vector2 canvasDiam = canvasRect.sizeDelta;

        star = new VisImage();
        planets = new VisImage[sys.planets.Count];

        #region Init
        //Star
        star.name = sys.star.name;
        star.color = starColors[sys.star.id];
        star.sprite = starSprite;
        star.size = starSizes[sys.star.id];

        //Planets
        for(int i = 0; i <= sys.planets.Count - 1; i++)
        {
            int id = sys.planets[i].id;
            VisImage planet = new VisImage
            {
                name = sys.planets[i].name,
                color = planetColors[id],
                sprite = planetSprites[id],
                size = planetSizes[id]
            };

            planets[i] = planet;
        }
        #endregion

        #region Positions
        float starPadding = (star.size.x / 2) * 1.5f;

        star.pos = new Vector2 { x = 0, y = 0 };

        float planetsTotalWidth = 0;
        foreach (VisImage planet in planets)
            planetsTotalWidth += planet.size.x;
        float canvasTotal = (canvasDiam.x / 2) - starPadding;

        float planetsTotalSpace = canvasTotal - planetsTotalWidth;

        float planetsSpace = planetsTotalSpace / planets.Length;


        for(int i = 0; i <= planets.Length - 1; i++)
        {
            float visX = ((planets[i].size.x + planetsSpace) * i) + starPadding;

            if (visX > canvasDiam.x / 2)
                visX = (canvasDiam.x / 2) - planets[i].size.x;

            planets[i].pos = new Vector2
            {
                x = visX,
                y = 0
            };
        }

        #endregion

        #region Render
        CreateVisualisation(star);

        foreach (VisImage planet in planets)
        {
            CreateVisualisation(planet);

            //Set a random orbit point
            planet.rect.RotateAround(star.rect.position, Vector3.right, Random.Range(0, 360));
        }
        #endregion

        orbiting = true;
    }

    void CreateVisualisation(VisImage vis)
    {
        GameObject visObj = new GameObject();
        visObj.transform.SetParent(systemParent);
        visObj.transform.name = vis.name;

        RectTransform visRect = visObj.AddComponent<RectTransform>();
        visRect.localScale = new Vector3(1, 1, 1);
        visRect.localPosition = new Vector3(0, 0, 0);
        visRect.localEulerAngles = new Vector3(0, 0, 0);
        visRect.anchorMin = new Vector2 { x = 0.5f, y = 0.5f };
        visRect.anchorMax = new Vector2 { x = 0.5f, y = 0.5f };
        visRect.pivot = new Vector2 { x = 0.5f, y = 0.5f };

        visRect.sizeDelta = vis.size;
        visRect.anchoredPosition = vis.pos;
        vis.rect = visRect;

        visObj.AddComponent<CanvasRenderer>();
        Image visImg = visObj.AddComponent<Image>();
        visImg.sprite = vis.sprite;
        visImg.color = vis.color;
    }


    bool visScr = false;
    public void ChangeScreen()
    {
        if(!visScr)
        {
            mainScreen.SetActive(true);
            infoScreen.SetActive(false);
        }
        else
        {
            mainScreen.SetActive(false);
            infoScreen.SetActive(true);
        }

        visScr = !visScr;
    }
}
