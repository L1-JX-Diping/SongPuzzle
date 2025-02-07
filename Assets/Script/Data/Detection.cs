
public class Detection 
{
    private float _timing; // 時刻
    private string _mic; // 検出されたマイク
    private float _volume; // 検出された音量

    public float Time { get => _timing; set => _timing = value; }

    public string Mic { get => _mic; set => _mic = value; }

    public float Volume { get => _volume; set => _volume = value; }
}
