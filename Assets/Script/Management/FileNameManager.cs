using UnityEngine;

/// <summary>
/// ���L�^�t�@�C���̊Ǘ����s���N���X
/// </summary>
public class FileNameManager : MonoBehaviour
{
    // �ʂ̃X�N���v�g����A�N�Z�X�ł���悤�� static �ɂ���
    private static string _metaData = "GameInfo.txt";
    private static string _micAssignment = "MicColorInfo.txt";
    private static string _markColorPair = "MarkColorDict.txt";
    private static string _correctPart = "PartLog.txt";
    private static string _micDitection = "MicDetectionLog.txt";

    // �v���p�e�B��ʂ��ăt�@�C�������擾�E�ݒ�
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
