using UnityEngine;

/// <summary>
/// 情報記録ファイルの管理を行うクラス
/// </summary>
public class FileName 
{
    // 別のスクリプトからアクセスできるように static にする
    private static string _micDitection = "MicDetectionLog.txt";
    private static string _songTitleList = "SongTitleList.txt";

    // XML
    private static string _xmlGameData = "GameData.xml";

    /// <summary>
    /// [format # Time, MicName, Volume]
    /// 4.17, Mic-device1, 0.0207 
    /// </summary>
    public static string MicDitection { get => _micDitection; set => _micDitection = value; }

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
}
