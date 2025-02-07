using UnityEngine;

[System.Serializable]
public class Part  
{
    private float _timing; // time
    private string _word; // a part of lyric
    private Player _player; // 
    //private Role _role; 

    public float Timing { get => _timing; set => _timing = value; }

    public string Word { get => _word; set => _word = value; }

    public Player Player { get => _player; set => _player = value; }
}

