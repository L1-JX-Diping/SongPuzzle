using System.Collections.Generic;

public class Song 
{
    //public int id;
    private string _title;
    // selected song parameter
    private int _beat = 4; // ‰½”q‚©H Birthday song ‚Í 3 ”q
    private int _bpm = 60; // 
    private List<Line> _lines;
    private Lyrics _lyrics;

    private int _intro;

    public string Title { get => _title; set => _title = value; }

    public float beatSec
    {
        get => 60f / (float)BPM;
    }

    public int Beat { get => _beat; set => _beat = value; }

    public int BPM { get => _bpm; set => _bpm = value; }

    public List<Line> Lines { get => _lines; set => _lines = value; }

    public int Intro { get => _intro; set => _intro = value; }

    public Lyrics Lyrics { get => _lyrics; set => _lyrics = value; }
}
