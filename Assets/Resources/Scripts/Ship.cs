using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static float maxFuel { get; private set; }
    public static float fuel {get; private set;}

    public static float sysFuelEfficency { get; private set; } //Units per lightyear.
    public static float planetFuelEfficency { get; private set; } //Units per 0.1 AU.

    //public static float speed { get; private set; } //Lightyears per second.

    public static float sysJumpAcceleration { get; private set; } //ly per second ^2
    public static float sysJumpTopSpeed { get; private set; } //ly per second

    public static float planetJumpAcceleration { get; private set; } //0.1 AU per second ^2
    public static float planetJumpTopSpeed { get; private set; } //0.1 AU km per second

    public static float scannerEff = 5.0f; //scan time for star; 3/1 scan time for planet
    private void Awake()
    {
        maxFuel = 100;
        fuel = maxFuel;

        sysFuelEfficency = 0.8f;
        planetFuelEfficency = 0.7f;

        sysJumpAcceleration = 0.2f;
        sysJumpTopSpeed = 1f;

        planetJumpAcceleration = 0.5f;
        planetJumpTopSpeed = 1.2f;

    }

    public static void ChangeFuel(float amount)
    {
        fuel += amount;
    }
}
