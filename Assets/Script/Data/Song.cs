using System.Collections.Generic;

public class Song 
{
    //public int id;
    private string _title;
    //public string description;
    // selected song parameter
    private float _clock = 3f; // Second per Beat
    private float _beat = 4; // ‰½”q‚©H Birthday song ‚Í 3 ”q
    private int _bpm;
    private List<Line> _lyrics;

    public string Title { get => _title; set => _title = value; }

    public float Clock { get => _clock; set => _clock = value; }

    public float Beat { get => _beat; set => _beat = value; }

    public int Bpm { get => _bpm; set => _bpm = value; }

    public List<Line> Lyrics { get => _lyrics; set => _lyrics = value; }
}
