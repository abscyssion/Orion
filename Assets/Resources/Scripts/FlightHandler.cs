using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flight
{
    public static bool jumping = false;

    public Flight(float distance, float topSpeed, float acceleration, float fuelEfficency)
    {
        this.distance = distance;

        timeAcc = topSpeed / acceleration;
        float accDist = topSpeed / 2 / timeAcc;

        timeCru = (distance - accDist * 2) / topSpeed;

        timeTotal = timeAcc * 2 + timeCru;

        fuel = fuelEfficency * distance;

        if (fuel <= Ship.fuel)
            possible = true;
        else
            possible = false;

        details = new FlightDetails(this.distance, timeTotal, fuel);
    }

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
            distance = Mathf.Round(distanceF).ToString();

            int minutes = (int)timeF / 60;
            int seconds = (int)timeF % 60;
            if (minutes > 0)
                time = minutes + " min " + seconds + "s";
            else
                time = seconds + "s";

            fuel = Mathf.Round(fuelF).ToString();
        }

        public string distance { get; private set; }
        public string time { get; private set; }
        public string fuel { get; private set; }
    }
    #endregion
}
public class FlightHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
