/// <summary>
/// 情報記録ファイルの管理を行うクラス
/// </summary>
public class FileName 
{
    // 別のスクリプトからアクセスできるように static にする
    private static string _songTitleList = "SongTitleList.txt";

    // XML
    private static string _xmlGameData = "GameData.xml";
    private static string _xmlMicLog = "MicLog.xml";
    private static string _xmlSong = "Song.xml";

    /// <summary>
    /// [format]
    /// One Call Away \n 
    /// Birthday Song \n 
    /// X'mas Song \n 
    /// </summary>
    public static string SongTitleList { get => _songTitleList; set => _songTitleList = value; }

    /// <summary>
    /// [format: xml] List<object>: Song title, Count(players), [Player1 name, Player2 name...]
    /// </summary>
    public static string XmlGameData { get => _xmlGameData; set => _xmlGameData = value; }

    /// <summary>
    /// Log of the detected microphones (include: time, mic, volume)
    /// </summary>
    public static string XmlMicLog { get => _xmlMicLog; set => _xmlMicLog = value; }

    /// <summary>
    /// 
    /// </summary>
    public static string XmlSong { get => _xmlSong; set => _xmlSong = value; }
}
