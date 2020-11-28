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

public class ObjJumpHandler : MonoBehaviour
{
    public class SysObj
    {
        public float orbit;
        public int id;
        public Type objType;
        public object obj;

        /*        public static SysObj ToSysObj(Star star)
        {
            SysObj sysObject = new SysObj();

            sysObject.orbit = 0f;
            sysObject.sysObj = star;
            sysObject.sysObjType = sysObject.GetType();

            return sysObject;
        }
        public static SysObj ToSysObj(PlanetObj planet)
        {
            SysObj sysObject = new SysObj();

            sysObject.orbit = planet.orbit;
            sysObject.sysObj = planet;


            return sysObject;
        }*/

        public SysObj(World.PlanetObj planet)
        {
            orbit = planet.orbit;
            id = planet.id;
            objType = typeof(World.PlanetObj);
            obj = planet;
        }

        public SysObj(Star star)
        {
            orbit = 0;
            id = 0;
            objType = typeof(Star);
            obj = star;
        }

        public T GetObj<T>()
        {
            if (typeof(T) == objType)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            else
                return default;
        }
    }

    public static ObjFlight currFlight;
    private static List<SysObj> sysObjects;
    private static int sysObjId;


    private void Start()
    {
        sysObjId = 0;

        ResetSysObjects();

        Debug.Log(GetSysObj().orbit);
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
}
