/// <summary>
/// 
/// </summary>
public class Part  
{
    private float _timing; // time
    private string _text; // a part of lyric
    private Role _role;
    private bool _isLineStart = false;

    /* HAVE TO DELETE */
    private Player _player; // have to delete

    /// <summary>
    /// Time to begin singing
    /// </summary>
    public float Timing { get => _timing; set => _timing = value; }

    /// <summary>
    /// part of lyrics to sing
    /// </summary>
    public string Text { get => _text; set => _text = value; }

    /// <summary>
    /// the start of a line of lyrics
    /// </summary>
    public bool IsLineStart { get => _isLineStart; set => _isLineStart = value; }

    /// <summary>
    /// Role 
    /// </summary>
    public Role Role { get => _role; set => _role = value; }

    public Player Player { get => _player; set => _player = value; }
}

