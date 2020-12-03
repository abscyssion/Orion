using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFlight : Flight
{
    public ObjFlight(World.PlanetObj destPlanet, float distance) : base(distance) //Constructor
    {
       
    }

    public World.PlanetObj destPlanet;
}

public class PlanetJumpHandler : MonoBehaviour
{
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

        public SysObj(World.PlanetObj planet)
        {
            name = planet.name;
            type = planet.type;
            description = planet.description;
            orbit = planet.orbit;
            id = planet.id;
        }

        public SysObj(World.StarObj star)
        {
            name = star.name;
            type = star.type;
            description = star.description;
            orbit = 0;
            id = 0;
        }

/*        public T GetObj<T>()
        {
            if (typeof(T) == objType)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            else
                return default;
        }*/
    }

    public static ObjFlight currFlight;
    private static List<SysObj> sysObjects;
    private static int sysObjId;


    private void Awake()
    {
        sysObjId = 0;

        ResetSysObjects();
    }

    public static void ResetSysObjects()
    {
        sysObjects = new List<SysObj>();

        World.Sys sys = World.GetSystem();

        sysObjects.Add(new SysObj(sys.star));
        foreach(World.PlanetObj planet in sys.planets)
        {
            sysObjects.Add(new SysObj(planet));
        }
    }

    public static void SetSysObj(int id)
    {
        if(id < sysObjects.Count)
        {
            sysObjId = id;
        }
    }

    public static SysObj GetSysObj()
    {
        return sysObjects[sysObjId];
    }

    public static SysObj GetSysObj(int id)
    {
        return sysObjects[id];
    }

    public static List<SysObj> GetSysObjects()
    {
        return sysObjects;
    }
}
