using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static World;

public class PlanetJumpHandler : MonoBehaviour
{
    public FlightHandler flightHandler;
    public static Flight currFlight;

    private void Awake()
    {
        SetFlight(new Flight(GetLocation()));
    }

    public bool Jump()
    {
        if (currFlight.possible)
        {
            flightHandler.Warp(currFlight);

            return true;
        }
        else
            return false;
    }

    public static void SetFlight(Flight flight)
    {
        currFlight = flight;
    }
}
