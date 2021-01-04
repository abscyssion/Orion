using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static float maxFuel { get; private set; }
    public static float fuel {get; private set;}

    public static float fuelEfficency { get; private set; } //Units per SU.
    public static float jumpAcceleration { get; private set; } //SU per second ^2
    public static float jumpTopSpeed { get; private set; } //SU per second

    public static readonly float puPerSu = 20f;

    public static float scannerEff = 5.0f; //scan time for star; 3/1 scan time for planet
    private void Awake()
    {
        maxFuel = 100;
        fuel = maxFuel;

        fuelEfficency = 0.8f;

        jumpAcceleration = 0.2f;
        jumpTopSpeed = 1f;
    }

    public static void ChangeFuel(float amount)
    {
        fuel += amount;
    }

    public static float ConvertDist(float dist, string unit)
    {
        if (unit.ToLower() == "pu")
            return dist * puPerSu;
        else if (unit.ToLower() == "su")
            return dist / puPerSu;
        else
            return default;
    }
}
