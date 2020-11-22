using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static float maxFuel { get; private set; }
    public static float fuel {get; private set;}

    public static float fuelEfficency { get; private set; } //Units per lightyear.

    //public static float speed { get; private set; } //Lightyears per second.

    public static float acceleration { get; private set; } //ly per second ^2
    public static float topSpeed { get; private set; } //ly per second

    public static float scannerEff = 1f;//5.0f; //scan time for star; 3/1 scan time for planet

    private void Awake()
    {
        maxFuel = 100;
        fuel = maxFuel;

        fuelEfficency = 0.8f;

        acceleration = 0.2f;//0.08f;
        topSpeed = 1f;
        //speed = 1.25f;//0.25f;
    }

    public static void ChangeFuel(float amount)
    {
        fuel += amount;
    }
}
