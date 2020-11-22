using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Star")]
public class Star : ScriptableObject
{
    public int id;

    public string type;
    public string description;

    public int maxTemperature;
    public int minTemperature;
    public int temperature;

    public Sprite sprite;

    public int visSize;
    public Color visualisationColor;
}
