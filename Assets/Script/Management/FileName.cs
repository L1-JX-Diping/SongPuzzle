using UnityEngine;

/// <summary>
/// 情報記録ファイルの管理を行うクラス
/// </summary>
public class FileName : MonoBehaviour
{
    // 別のスクリプトからアクセスできるように static にする
    private static string _metaData = "GameInfo.txt";
    private static string _playerAssignment = "PlayerAssignment.txt";
    private static string _avatarColorPair = "MarkColorDict.txt";
    private static string _correctPart = "PartLog.txt";
    private static string _micDitection = "MicDetectionLog.txt";
    private static string _songTitleList = "SongTitleList.txt";

    // プロパティを通じてファイル名を取得・設定
    /// <summary>
    /// [format # SongTitle \n PlayerCount] 
    /// Birthday Song \n 
    /// 2
    /// </summary>
    public static string MetaData { get => _metaData; set => _metaData = value; }

    /// <summary>
    /// [format # ColorName, MicName, AvatarName]
    /// GREEN, Mic-device1, Heart
    /// </summary>
    public static string PlayerAssignment { get => _playerAssignment; set => _playerAssignment = value; }

    /// <summary>
    /// [format # ColorName, AvatarName]
    /// GREEN, Heart
    /// </summary>
    public static string AvatarColorPairing { get => _avatarColorPair; set => _avatarColorPair = value; }

    /// <summary>
    /// [format # Time, ColorName]
    /// 04.00, RED
    /// </summary>
    public static string CorrectPart { get => _correctPart; set => _correctPart = value; }

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
}
