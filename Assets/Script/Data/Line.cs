using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line 
{
    private float _timing; // the TIME when a line of lyrics appears, measured in seconds
    private string _text; // the LYRIC of this line
    private List<Part> _partList = new List<Part>(); // division information

    public float Timing { get => _timing; set => _timing = value; }
    
    public string Text { get => _text; set => _text = value; }

    public List<Part> PartList { get => _partList; set => _partList = value; }
}
