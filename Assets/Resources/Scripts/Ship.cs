using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static float maxFuel { get; private set; }
    public static float fuel {get; private set;}

    public static float fuelEfficency { get; private set; } //Units per lightyear.
    //public static float planetFuelEfficency { get; private set; } //Units per 0.1 AU.
    
    /* Distance units.
     * SU - system unit = 4 light years
     * PU - planet unit = 20 SU 
     */

    public static float jumpAcceleration { get; private set; } //ly per second ^2
    public static float jumpTopSpeed { get; private set; } //ly per second
/*
    public static float planetJumpAcceleration { get; private set; } //0.1 AU per second ^2
    public static float planetJumpTopSpeed { get; private set; } //0.1 AU km per second*/

    public static float scannerEff = 5.0f; //scan time for star; 3/1 scan time for planet
    private void Awake()
    {
        maxFuel = 100;
        fuel = maxFuel;

        fuelEfficency = 0.8f;
        //planetFuelEfficency = 0.7f;

        jumpAcceleration = 0.2f;
        jumpTopSpeed = 1f;
/*
        planetJumpAcceleration = 0.5f;
        planetJumpTopSpeed = 1.2f;*/

    }

    public static void ChangeFuel(float amount)
    {
        fuel += amount;
    }
}
