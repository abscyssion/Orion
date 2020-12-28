using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    private const string mainDir = @"Assets/Resources/Scripts Content";

    public StarType[] starsAll = null;
    public PlanetType[] planetsAll = null;

    public static Vector2Int mapSize = new Vector2Int(10, 10);

    public static Vector2Int virtCellSize = new Vector2Int(10, 10);
    private const float marginPrc = 0.3f;

    private const int minPlanets = 2;
    private const int maxPlanets = 5;

    private static Sys[,] systems;

    public static Vector2Int currSysId { get; private set; }

    private static Location locationCurr;

    public static int currSysObjId { get; private set; } // 0 - the star

    public class Sys
    {
        public string name;
        public float security;

        public Vector2 localCoords;
        public Vector2Int cellCoords;

        public Star star;
        public List<Planet> planets;
    

        public List<SysObj> GetSysObjs()
        {
            List<SysObj> sysObjs = new List<SysObj>();

            sysObjs.Add(new SysObj(star));
            foreach(Planet planet in planets)
            {
                sysObjs.Add(new SysObj(planet));
            }

            return sysObjs;
        }

        public Vector2 GlobalPos()
        {
            Vector2 globalPos = (cellCoords * virtCellSize) + localCoords;
            return globalPos;
        }

        public string FormatSec()
        {
            string str = System.Math.Round(this.security, 1).ToString();
            string strFinal = "";

            foreach(char ch in str)
            {
                if(ch != ',')
                {
                    strFinal += ch;
                }
                else
                {
                    strFinal += '.';
                }
            }
            return strFinal;
        }
    }

    public class SysObj
    {
        public string name; //Given name of the star, planet;
        public string type;
        public string description;

        /*
         * bool canMine;
         * bool canGasExcavate;
         * 
         */

        public float orbit;
        public int id;

        public SysObj(Planet planet)
        {
            name = planet.name;
            type = planet.type;
            description = planet.description;
            orbit = planet.orbit;
            id = planet.id;
        }

        public SysObj(Star star)
        {
            name = star.name;
            type = star.type;
            description = star.description;
            orbit = 0;
            id = 0;
        }
    }

    public class Star : StarType
    {
        public Star(StarType star)
        {
            type = star.type;
            description = star.description;

            maxTemperature = star.maxTemperature;
            minTemperature = star.minTemperature;

            sprite = star.sprite;

            visSize = star.visSize;
            visColor = star.visColor;
        }

        new public string name;
        public int temperature;
    }

    public class Planet : PlanetType
    {
        Planet() { }

        public Planet(PlanetType planet)
        {
            type = planet.type;
            description = planet.description;

            canHaveAtmosphere = planet.canHaveAtmosphere;
            canHaveMoons = planet.canHaveMoons;
            canSupportLife = planet.canSupportLife;
            field = planet.field;

            visSize = planet.visSize;
            visColor = planet.visColor;
        }

        new public string name;
        public float orbit;
        //moons
    }

    public class Location
    {
        public Sys sys { get; private set; }
        public SysObj sysObj { get; private set; }

        public Location(Sys sysSet, SysObj sysObjSet)
        {
            sys = sysSet;
            sysObj = sysObjSet;
        }

        public Location(Sys sysSet)
        {
            sys = sysSet;
            sysObj = sysSet.GetSysObjs()[0];
        }

        public Location(SysObj sysObjSet)
        {
            sys = GetLocation().sys;
            sysObj = sysObjSet;
        }
    }

    void Awake()
    {
        systems = new Sys[mapSize.x, mapSize.y];
        GenerateWorld();

        var sys = GetSystem(0, 0);
        SetLocation(new Location(sys, sys.GetSysObjs()[0]));
    }

    private void GenerateWorld()
    {
        string[,] systemNames = new string[mapSize.x, mapSize.y];
        float[,] systemSecs = new float[mapSize.x, mapSize.y];
        Star[,] systemStars = new Star[mapSize.x, mapSize.y];
        List<Planet>[,] systemPlanets = new List<Planet>[mapSize.x, mapSize.y]; //Array of lists.
        Vector2[,] systemCoords = new Vector2[mapSize.x, mapSize.y];


        //Get system names from a file.
        #region System Names
        var systemNamesFilePath = mainDir + "/systemNames.txt";
        var systemNamesFile = new StreamReader(systemNamesFilePath);

        List<int> systemNamesIdsAll = new List<int>();

        int nameCount = 0;
        while (systemNamesFile.ReadLine() != null) //Count all names.
        {
            systemNamesIdsAll.Add(nameCount);
            nameCount++;
        }

        int[,] systemNamesIds = new int[mapSize.x, mapSize.y];

        for (int x = 0; x <= mapSize.x - 1; x++) //Generate random system name ids.
        {
            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                int random = UnityEngine.Random.Range(0, systemNamesIdsAll.Count - 1);
                int systemNameId = systemNamesIdsAll[random];
                systemNamesIdsAll.RemoveAt(random);

                systemNamesIds[x, y] = systemNameId;
            }
        }

        systemNamesFile.DiscardBufferedData();
        systemNamesFile.BaseStream.Seek(0, SeekOrigin.Begin);
        systemNamesFile.BaseStream.Position = 0;

        string line;
        int j = 0;

        while ((line = systemNamesFile.ReadLine()) != null) //Check if the current line is a valid system name.
        {
            for (int x = 0; x <= mapSize.x - 1; x++) 
            {
                for (int y = 0; y <= mapSize.y - 1; y++)
                {
                    if (systemNamesIds[x, y] == j)
                    {
                        systemNames[x, y] = line;
                        goto endOfCheck;
                    }
                }
            }
            endOfCheck:
            j++;
        }

        systemNamesFile.Close();
        #endregion

        //Generate world security levels using Perlin Noise.
        #region Security
        const float mapScale = 20f;

        Vector2 perlinSeeds = GetSecPerlinSeed(mapScale);

        for (int x = 0; x <= mapSize.x - 1; x++)
        {
            float xFloat = (float)x / mapSize.x * mapScale + perlinSeeds.x;

            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                float yFloat = (float)y / mapSize.y * mapScale + perlinSeeds.y;
                systemSecs[x, y] = Mathf.PerlinNoise(xFloat, yFloat);
            }
        }

        #endregion

        //Generate stars of the systems.
        #region Stars
        for (int x = 0; x <= mapSize.x - 1; x++)
        {
            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                int randomId = UnityEngine.Random.Range(0, starsAll.Length - 1);

                Star star = new Star(starsAll[randomId]);
                star.temperature = UnityEngine.Random.Range(star.minTemperature, star.maxTemperature);

                star.name = systemNames[x, y];

                systemStars[x, y] = star;
            }
        }
        #endregion

        //Generate planets of the systems.
        #region Planets
        int planetsCount = UnityEngine.Random.Range(minPlanets, maxPlanets);

        const float randMinOrbit = 0.2f; const float randMaxOrbit = 2f; //in PU

        float orbitSum = 0.5f;
        for (int x = 0; x <= mapSize.x - 1; x++)
        {
            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                List <Planet> planetList = new List<Planet>();
                List<float> orbits = new List<float>();

                for (int i = 0; i <= planetsCount - 1; i++)
                {
                    int randomId = UnityEngine.Random.Range(0, planetsAll.Length - 1);

                    Planet planet = new Planet(planetsAll[randomId]);
                    orbitSum += UnityEngine.Random.Range(randMinOrbit, randMaxOrbit);
                    planet.orbit = orbitSum;

                    const int digits = 4;
                    string name = systemNames[x, y][0].ToString() + systemNames[x, y][1].ToString();
                    for(int k = 0; k <= digits - 1; k++)
                    {
                        int rand = UnityEngine.Random.Range(0, 9);
                        name += rand.ToString();
                    }

                    planet.name = name;
                    planetList.Add(planet);
                }

                orbitSum = 0.5f;
                systemPlanets[x, y] = planetList;
            }
        }
        #endregion

        //Place the systems on a map.
        #region Coords
        const float marginPrc = 0.1f;

        Vector2 margin = new Vector2
        {
            x = virtCellSize.x / 2 - (virtCellSize.x / 2 * marginPrc),
            y = virtCellSize.y / 2 - (virtCellSize.y / 2 * marginPrc)
        };

        for (int x = 0; x <= mapSize.x - 1; x++)
        {
            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                Vector2 systemCoord = new Vector2
                {
                    x = Mathf.Clamp(UnityEngine.Random.Range(-(virtCellSize.x / 2f), (virtCellSize.x / 2f)), -margin.x, margin.x),
                    y = Mathf.Clamp(UnityEngine.Random.Range(-(virtCellSize.y / 2f), (virtCellSize.y / 2f)), -margin.y, margin.y)
                };

                print(x + ", " + y + ": " + systemCoord);

                systemCoords[x, y] = systemCoord;
            }
        }
        #endregion

        //Save to system objects array.
        #region Save
        for (int x = 0; x <= mapSize.x - 1; x++)
        {
            for (int y = 0; y <= mapSize.y - 1; y++)
            {
                Sys sys = new Sys
                {
                    name = systemNames[x, y],
                    security = systemSecs[x, y],
                    star = systemStars[x, y],
                    planets = systemPlanets[x, y],
                    localCoords = systemCoords[x, y],
                    cellCoords = new Vector2Int(x, y)
                };

                systems[x, y] = sys;
            }
        }
        #endregion
    }

    public static void SetLocation(Location location)
    {
        locationCurr = location;
    }

    public static Location GetLocation()
    {
        return locationCurr;
    }

    private static Vector2 GetSecPerlinSeed(float mapScale) //Gets a seed that generates high sec in the first systems.
    {
        int x, y;

        float seedX, seedY;

        float floatX, floatY;

        float[] perlinValues = new float[3];

        while(true)
        {
            seedX = UnityEngine.Random.Range(0f, 1000000f);
            seedY = UnityEngine.Random.Range(0f, 1000000f);

            x = 0; y = 0;                             //[0,0]
            floatX = (float)x / mapSize.x * mapScale + seedX;
            floatY = (float)y / mapSize.y * mapScale + seedY;
            perlinValues[0] = Mathf.PerlinNoise(floatX, floatY);

            x = 1; y = 0;                             //[1,0]
            floatX = (float)x / mapSize.x * mapScale + seedX;
            floatY = (float)y / mapSize.y * mapScale + seedY;
            perlinValues[1] = Mathf.PerlinNoise(floatX, floatY);

            x = 0; y = 1;                             //[0,1]
            floatX = (float)x / mapSize.x * mapScale + seedX;
            floatY = (float)y / mapSize.y * mapScale + seedY;
            perlinValues[2] = Mathf.PerlinNoise(floatX, floatY);

            if (perlinValues[0] >= 0.8f && perlinValues[1] >= 0.7f && perlinValues[2] >= 0.7f)
                    break;
        }

        Vector2 perlinSeeds = new Vector2
        {
            x = seedX,
            y = seedY
        };

        return perlinSeeds;
    }

    public static Sys GetSystem(Vector2Int id)
    {
        return systems[id.x, id.y];
    }
    
    public static Sys GetSystem(int x, int y)
    {
        return systems[x, y];
    }
}

