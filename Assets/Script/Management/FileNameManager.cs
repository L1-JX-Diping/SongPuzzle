using UnityEngine;

/// <summary>
/// 情報記録ファイルの管理を行うクラス
/// </summary>
public class FileNameManager : MonoBehaviour
{
    // 別のスクリプトからアクセスできるように static にする
    private static string _metaData = "GameInfo.txt";
    private static string _micAssignment = "MicColorInfo.txt";
    private static string _markColorPair = "MarkColorDict.txt";
    private static string _correctPart = "PartLog.txt";
    private static string _micDitection = "MicDetectionLog.txt";

    // プロパティを通じてファイル名を取得・設定
    /// <summary>
    /// [format]
    /// </summary>
    public static string Meta { get => _metaData; set => _metaData = value; }

    /// <summary>
    /// [format] ColorName, MicName, AvatarName
    /// GREEN, Mic-device1, Heart
    /// </summary>
    public static string MicAssignment { get => _micAssignment; set => _micAssignment = value; }

    /// <summary>
    /// [format] # ColorName, AvatarName
    /// GREEN, Heart
    /// </summary>
    public static string AvatarColorPairing { get => _markColorPair; set => _markColorPair = value; }

    /// <summary>
    /// [format] # Time, ColorName
    /// 04.00, RED
    /// </summary>
    public static string CorrectPart { get => _correctPart; set => _correctPart = value; }

    /// <summary>
    /// [format] # Time, MicName, Volume
    /// 4.17, Mic-device1, 0.0207 
    /// </summary>
    public static string MicDitection { get => _micDitection; set => _micDitection = value; }
}
