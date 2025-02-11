using System.Threading;

/// <summary>
/// 
/// </summary>
public class Lyrics 
{
    private string _text = "";
    private Role _role;

    /// <summary>
    /// Lyrics text
    /// </summary>
    public string Text { get => _text; set => _text = value; }

    public Role Role { get => _role; set => _role = value; }
}
