using UnityEngine;

public class Role 
{
    private Player _player;
    private Color _color;
    private string _avatar;
    private string _mic; // ƒ}ƒCƒN–¼

    public Player Player { get => _player; set => _player = value; }

    public Color Color { get => _color; set => _color = value; }

    public string Avatar { get => _avatar; set => _avatar = value; }

    public string Mic { get => _mic; set => _mic = value; }
}
