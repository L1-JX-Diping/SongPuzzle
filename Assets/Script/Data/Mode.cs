/// <summary>
/// Mode control of game: Easy or Hard?
/// </summary>
public class Mode 
{
    private float _timingThreshold; // to calculate score

    /// <summary>
    /// Threshold for a lag of the time player started to sing
    /// </summary>
    public float TimingThreshold { get => _timingThreshold; set => _timingThreshold = value; }
}
