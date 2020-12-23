using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static World;

public class SystemFlight : Flight
{
    public SystemFlight(Sys destSys, float distance) : base(distance, Ship.sysJumpTopSpeed, Ship.sysJumpAcceleration, Ship.sysFuelEfficency) //Constructor
    {
        this.destSys = destSys;
        this.possible = possible && destSys != GetSystem();
    }

    public Sys destSys { get; private set; }
}

public class SysJumpHandler : MonoBehaviour
{
/*    public Transform outsideTransform;
    private const int outsideMaxDist = -25000; //The star and shit.*/

    public SysJumpScreen jumpScreen;

    public static SystemFlight currFlight;

    private static GameObject[] engineMuzzles;
    private static Vector2[] muzzleScalesDef;
    private static SysNavScreen[] navScreens;

    public RectTransform shipCursor;
    public Animator shipAnim;
    public Image shipSpinImage;

    private StarScanScreen starScanScreen;

    private float maxMuzzScale = 2;
    private float minMuzzScale;

    public ParticleSystem warpParticles;
    [SerializeField]
    private float maxParticleSpeed = 0;
    private float minParticleSpeed = 0;

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
        for(int i = 0; i <= navScreenObjects.Length - 1; i++)
        {
            navScreens[i] = navScreenObjects[i].GetComponent<SysNavScreen>();
        }

        warpParticles.gameObject.SetActive(false);   

        starScanScreen = GameObject.Find("Star Scan Screen").GetComponent<StarScanScreen>();

        SetText(true, "NOW VISITING:");
        SetText(false, World.GetSystem().name);
    }
   
    public static SystemFlight GenerateFlight(Sys sys)
    {
        Vector2 distVec = sys.GlobalPos() - GetSystem().GlobalPos();
        float distance = Mathf.Abs(distVec.x) + Mathf.Abs(distVec.y);

        return new SystemFlight(sys, distance);
    }

    public static void SetFlight(SystemFlight systemFlight)
    {
        currFlight = systemFlight;
    }

    public bool Jump()
    {
        if (currFlight != null)
        {
            if (currFlight.possible)
            {
                Ship.ChangeFuel(-currFlight.fuel);

                SetSystem(currFlight.destSys.cellCoords);

                Screen.ChangeScreensAll(false);
                StartCoroutine(Jumping());

                SetText(false, currFlight.destSys.name);

                starScanScreen.ResetScanner();

                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    private IEnumerator Jumping()
    {
        //Vector2 shipEndMapPos = MapScreen.GetSystemPos(currFlight.destSys.cellCoords);

        Flight.jumping = true;

        warpParticles.gameObject.SetActive(true);

/*        shipSpinImage.enabled = true;
        shipAnim.SetTrigger("Start");*/

        const float delay = 0.01f;

        float timeTotal = currFlight.timeTotal;
        float timeAcc = currFlight.timeAcc;
        float timeCru = currFlight.timeCru;
        float acceleration = Ship.sysJumpAcceleration;
        float jumpVelPeak = Ship.sysJumpTopSpeed;

        float jumpDist = currFlight.distance;

        /*Vector2 shipStartMapPos = shipCursor.anchoredPosition;*/

        float jumpTimeLeft = timeTotal;
        float jumpTime = 0f;

        float jumpVelCurr = 0;
        float jumpVelPerc = 0;

        float flightPerc = 0;
        while (jumpTimeLeft > 0.0f)
        {
            SetText(true, "NOW DEPARTING (" + FormatTime(jumpTimeLeft) + "):");
            
            //Accelerating.
            if (jumpTime <= timeAcc) 
            {
                jumpVelCurr = jumpTime * acceleration;
            }
            //Cruising.
            else if (jumpTime > timeAcc && jumpTime <= timeCru) 
            {
                jumpVelCurr = jumpVelPeak;
            }
            //Decelerating.
            else if (jumpTime > (timeTotal - timeAcc)) 
            {
                jumpVelCurr = jumpTimeLeft * acceleration;
            }

            jumpVelPerc = jumpVelCurr / jumpVelPeak;

/*            Vector2 shipCursorPos = new Vector2
            {
                x = Mathf.Lerp(shipStartMapPos.x, shipEndMapPos.x, flightPerc),
                y = Mathf.Lerp(shipStartMapPos.y, shipEndMapPos.y, flightPerc)
            };*/

            //shipCursor.anchoredPosition = shipCursorPos;

            SetMuzzleLength(jumpVelPerc);
            SetWarpParticles(jumpVelPerc);

            jumpTimeLeft -= delay;
            jumpTime += delay;
            
            yield return new WaitForSeconds(delay);
        }

        warpParticles.gameObject.SetActive(false);

        Flight.jumping = false;

        Screen.ChangeScreensAll(true);
        jumpScreen.RefreshScreen();

        SetText(true, "DEST. REACHED:");

        shipAnim.SetTrigger("End");

        yield return new WaitForSeconds(5.0f);

        SetText(true, "NOW VISITING:");
    }

    #region Visuals
    private void SetMuzzleLength(float percent)
    {
        float z = minMuzzScale + maxMuzzScale * percent;
        for(int i = 0; i <= engineMuzzles.Length - 1; i++)
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

    private void SetShipIcon(float percent)
    {

    }

    private static void SetText(bool top, string str)
    {
        if (top)
        {
            for (int i = 0; i <= navScreens.Length - 1; i++)
            {
                navScreens[i].SetTopText(str);
            }
        }
        else
        {
            for (int i = 0; i <= navScreens.Length - 1; i++)
            {
                navScreens[i].SetBotText(str);
            }
        }
    }

    private static string FormatTime(float time)
    {
        /*        int units = 4; // = formatted len - ':'

                int len = seconds.ToString().Length;
                units = Mathf.Clamp(units, 1, len);
                units++;

                string time = seconds.ToString();
                string timeFormatted = "";

                for (int i = 0; i <= units - 1; i++)
                {

                        if (time[i] != ',')
                        {
                            timeFormatted += time[i];
                        }
                        else
                        {
                            timeFormatted += ':';
                        }

                }

                return timeFormatted;
        */

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