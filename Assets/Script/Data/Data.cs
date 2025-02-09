/// <summary>
/// Save Game Data
/// </summary>
public class Data 
{
    // game meta data
    private Team _team;
    private Song _song;
    private Role _role;
    private float _score;
    private Mode _mode;

    /// <summary>
    /// the song to play
    /// </summary>
    public Song Song { get => _song; set => _song = value; }

    /// <summary>
    /// team information, DEFAULT: Team0 "Play as Anonymous" 
    /// (includes player(member) list)
    /// </summary>
    public Team Team { get => _team; set => _team = value; }

    /// <summary>
    /// role assignment in THIS turn (game by game)
    /// </summary>
    //public Role Role { get => _role; set => _role = value; }

    public float Score { get => _score; set => _score = value; }

    public Mode Mode { get => _mode; set => _mode = value; }
}

