using UnityEngine;

/// <summary>
/// Role of Player: name, color, avatar, mic
/// </summary>
[System.Serializable]
public class Player 
{
    private int _id;
    private string _name;
    private Role _role;

    public int Id { get => _id; set => _id = value; }

    public string Name { get => _name; set => _name = value; }
    
    public Role Role { get => _role; set => _role = value; }
}
