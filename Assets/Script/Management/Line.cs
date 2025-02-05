using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line 
{
    public float timing; // the TIME when a line of lyrics appears, measured in seconds
    public string text; // the LYRIC of this line
    public List<Part> partList = new List<Part>(); // division information
}
