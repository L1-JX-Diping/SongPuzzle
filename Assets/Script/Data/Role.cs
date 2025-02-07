using System;
using UnityEngine;

public class Role 
{
    //private Player _player;
    private bool _isRobot = true;
    private Color _color;
    private string _avatar;
    private string _mic; // ƒ}ƒCƒN–¼

    //public Player Player { get => _player; set => _player = value; }

    /// <summary>
    /// 
    /// </summary>
    public Color Color { get => _color; set => _color = value; }

    /// <summary>
    /// 
    /// </summary>
    public string Avatar { get => _avatar; set => _avatar = value; }

    /// <summary>
    /// 
    /// </summary>
    public string Mic { get => _mic; set => _mic = value; }

    /// <summary>
    /// Parts which assigned mics include the term "Robot": Robot part
    /// </summary>
    public bool IsRobot { get => _isRobot; set => _isRobot = value; }
}
