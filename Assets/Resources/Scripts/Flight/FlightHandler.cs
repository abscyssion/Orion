using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static World;

public class Flight
{
    public Flight(Location loc)
    {
        destLocation = loc;

        var currLoc = GetLocation();

        distance = 0;

        Vector2 sysDistVec = loc.sys.GlobalPos() - currLoc.sys.GlobalPos();
        distance += Mathf.Abs(sysDistVec.x) + Mathf.Abs(sysDistVec.y); //System distance.

        distance += Mathf.Abs(currLoc.sysObj.orbit - loc.sysObj.orbit); //Sys. obj. distance.

        timeAcc = Ship.jumpTopSpeed / Ship.jumpAcceleration;
        float accDist = Ship.jumpTopSpeed / 2 / timeAcc;

        timeCru = (distance - accDist * 2) / Ship.jumpTopSpeed;

        timeTotal = timeAcc * 2 + timeCru;

        fuel = Ship.fuelEfficency * distance;

        if (distance > 0 && fuel <= Ship.fuel)
            possible = true;
        else
            possible = false;

        details = new FlightDetails(this.distance, timeTotal, fuel);
    }

    public Location destLocation { get; protected set; }
    public float distance { get; protected set; }
    public float timeAcc { get; protected set; }
    public float timeCru { get; protected set; }
    public float timeTotal { get; protected set; } //In seconds.
    public float fuel { get; protected set; }

    public FlightDetails details { get; private set; }
    public bool possible { get; protected set; }

    #region Flight Details (Strings)
    public struct FlightDetails
    {
        public FlightDetails(float distanceF, float timeF, float fuelF)
        {
            distanceSU = Mathf.Round(distanceF).ToString();
            distancePU = Mathf.Round(Ship.ConvertDist(distanceF,"pu")).ToString();

            int minutes = (int)timeF / 60;
            int seconds = (int)timeF % 60;
            if (minutes > 0)
                time = minutes + " min " + seconds + "s";
            else
                time = seconds + "s";

            fuel = Mathf.Round(fuelF).ToString();
        }

        public string distanceSU { get; private set; } //in SU. PU = 1/20 SU
        public string distancePU { get; private set; }
        public string time { get; private set; }
        public string fuel { get; private set; }
    }
    #endregion

}

public class FlightHandler : MonoBehaviour
{
    private static GameObject[] engineMuzzles;
    private static Vector2[] muzzleScalesDef;
    private static SysNavScreen[] navScreens;

    private float maxMuzzScale = 2;
    private float minMuzzScale;

    public ParticleSystem warpParticles;
    [SerializeField]
    private float maxParticleSpeed;
    private float minParticleSpeed = 0;

    public SysJumpScreen sysJumpScreen;
    public PlanetJumpScreen planetJumpScreen;

    public static bool jumping = false;

    private void Awake()
    {
        engineMuzzles = GameObject.FindGameObjectsWithTag("Engine Muzzle");
        muzzleScalesDef = new Vector2[engineMuzzles.Length];
        for (int i = 0; i <= engineMuzzles.Length - 1; i++)
        {
            muzzleScalesDef[i] = engineMuzzles[i].transform.localScale;
        }

        minMuzzScale = engineMuzzles[0].transform.localScale.z;

        GameObject[] navScreenObjects = GameObject.FindGameObjectsWithTag("Nav Screen"); navScreens = new SysNavScreen[navScreenObjects.Length];
        for (int i = 0; i <= navScreenObjects.Length - 1; i++)
        {
            navScreens[i] = navScreenObjects[i].GetComponent<SysNavScreen>();
        }

        warpParticles.gameObject.SetActive(false);

        SetTopText("NOW VISITING:");
        SetBotText(GetLocation());
    }

    public void Jump(Flight flight)
    {
        /* 
         * The philosophy of jumping:
         * 
         * This class is the main one.
         * 
         * The classes derived act as controlers of this class, and of their screens
         */

        if (flight != null)
        {
            if (flight.possible && !jumping)
            {
                Ship.ChangeFuel(-flight.fuel);
                Screen.ChangeScreensAll(false);
                SetLocation(flight.destLocation);

                StartCoroutine(Jumping(flight));

                SetTopText("NOW DEPARTING:");
                SetBotText(flight.destLocation);
            }
        }
    }

    private IEnumerator Jumping(Flight flight)
    {
        jumping = true;

        warpParticles.gameObject.SetActive(true);

        const float delay = 0.01f;

        float timeTotal = flight.timeTotal;
        float timeAcc = flight.timeAcc;
        float timeCru = flight.timeCru;

        float jumpDist = flight.distance;

        float jumpTimeLeft = timeTotal;
        float jumpTime = 0f;

        float jumpVelCurr = 0;
        float jumpVelPerc = 0;

        while (jumpTimeLeft > 0.0f)
        {
            SetTopText("NOW DEPARTING (" + FormatTime(jumpTimeLeft) + "):");

            //Accelerating.
            if (jumpTime <= timeAcc)
            {
                jumpVelCurr = jumpTime * Ship.jumpAcceleration;
            }
            //Cruising.
            else if (jumpTime > timeAcc && jumpTime <= timeCru)
            {
                jumpVelCurr = Ship.jumpTopSpeed;
            }
            //Decelerating.
            else if (jumpTime > (timeTotal - timeAcc))
            {
                jumpVelCurr = jumpTimeLeft * Ship.jumpAcceleration;
            }

            jumpVelPerc = jumpVelCurr / Ship.jumpTopSpeed;

            SetMuzzleLength(jumpVelPerc);
            SetWarpParticles(jumpVelPerc);

            jumpTimeLeft -= delay;
            jumpTime += delay;

            yield return new WaitForSeconds(delay);
        }

        warpParticles.gameObject.SetActive(false);

        jumping = false;

        Screen.ChangeScreensAll(true);
        
        planetJumpScreen.RefreshScreen();
        

        SetTopText("DEST. REACHED:");

        yield return new WaitForSeconds(5.0f);

        SetTopText("NOW VISITING:");
    }
    #region Visuals
    protected static void SetTopText(string str)
    {
        for (int i = 0; i <= navScreens.Length - 1; i++)
        {
            navScreens[i].SetTopText(str);
        }
    }

    protected static void SetBotText(Location loc)
    {
        for (int i = 0; i <= navScreens.Length - 1; i++)
        {
            navScreens[i].SetBotText(loc.sys.name + " - " + loc.sysObj.name);
        }
    }

    private void SetMuzzleLength(float percent)
    {
        float z = minMuzzScale + maxMuzzScale * percent;
        for (int i = 0; i <= engineMuzzles.Length - 1; i++)
        {
            Vector3 scale = muzzleScalesDef[i]; scale.z = z;
            engineMuzzles[i].transform.localScale = scale;
        }
    }

    private void SetWarpParticles(float percent)
    {
        float speed = minParticleSpeed + maxParticleSpeed * percent;

        var particlesMain = warpParticles.main;
        particlesMain.startSpeed = speed;
    }

    private static string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        float seconds = (int)time % 60;

        string str;

        if (minutes > 0)
            str = minutes + "min, " + seconds + "s";
        else
            str = seconds + "s";

        return str;
    }
    #endregion
}