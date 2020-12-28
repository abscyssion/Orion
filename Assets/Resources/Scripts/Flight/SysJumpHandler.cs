using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static World;

public class SysJumpHandler : MonoBehaviour
{
    public FlightHandler flightHandler;

    public SysJumpScreen sysJumpScreen;
    public PlanetJumpScreen planetJumpScreen;

    public RectTransform shipCursor;
    public Animator shipAnim;
    public Image shipSpinImage;

    public static StarScanScreen starScanScreen;

    public static Flight currFlight { get; private set; }

    private void Awake()
    {
        starScanScreen = GameObject.Find("Star Scan Screen").GetComponent<StarScanScreen>(); //change
        SetFlight(new Flight(GetLocation()));
    }

    public bool Jump()
    {
        if (currFlight.possible)
        {
            starScanScreen.ResetScanner();
            planetJumpScreen.RefreshScreen(currFlight.destLocation);
            flightHandler.Jump(currFlight);
            return true;
        }
        else
            return false;
    }

    public static void SetFlight(Flight flight)
    {
        currFlight = flight;
    }

    #region ChangingShipIcon()
    //Vector2 shipEndMapPos = MapScreen.GetSystemPos(currFlight.destSys.cellCoords);

    /*        shipSpinImage.enabled = true;
            shipAnim.SetTrigger("Start");*/
    /*Vector2 shipStartMapPos = shipCursor.anchoredPosition;*/

    /*            Vector2 shipCursorPos = new Vector2
            {
                x = Mathf.Lerp(shipStartMapPos.x, shipEndMapPos.x, flightPerc),
                y = Mathf.Lerp(shipStartMapPos.y, shipEndMapPos.y, flightPerc)
            };*/

    //shipCursor.anchoredPosition = shipCursorPos;


    //shipAnim.SetTrigger("End");

    #endregion

    private void SetShipIcon(float percent)
    {

    }
}