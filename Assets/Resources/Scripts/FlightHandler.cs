using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flight
{
    public Flight(float distance)
    {
        timeAcc = Ship.topSpeed / Ship.acceleration;
        float accDist = Ship.topSpeed / 2 / timeAcc;

        timeCru = (distance - accDist * 2) / Ship.topSpeed;

        timeTotal = timeAcc * 2 + timeCru;

        fuel = Ship.fuelEfficency * distance;

        if (fuel <= Ship.fuel)
            possible = true;
        else
            possible = false;
    }

    public float distance { get; protected set; }
    public float timeAcc { get; protected set; }
    public float timeCru { get; protected set; }
    public float timeTotal { get; protected set; } //In seconds.
    public float fuel { get; protected set; }
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

    public FlightDetails GetFlightDetails()
    {
        FlightDetails flightDetails = new FlightDetails(distance, timeTotal, fuel);

        return flightDetails;
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
