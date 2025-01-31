using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarkColorReader : MonoBehaviour
{
    // ��񌹃t�@�C����
    private string _markColorFileName = "MarkColorDict.txt";
    // �}�[�N�ƐF�̑Ή����i�[ Dictionary
    private Dictionary<string, Color> _markToColor = new Dictionary<string, Color>();
    private Dictionary<Color, string> _colorToMark = new Dictionary<Color, string>();

    void Start()
    {
        LoadMarkColorDict();

        // �m�F�̂��߂Ƀ��O���o��
        foreach (var entry in _markToColor)
        {
            Debug.Log($"Mark: {entry.Key}, Color: {entry.Value}");
        }
    }

    void LoadMarkColorDict()
    {
        // �t�@�C���p�X���擾
        string filePath = Path.Combine(Application.dataPath, _markColorFileName);

        // �t�@�C�������݂��Ȃ��ꍇ
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        // �t�@�C�����s���Ƃɓǂݍ���
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // �J���}�ŕ������ă}�[�N�ƐF���擾
            string[] parts = line.Split(',');

            if (parts.Length == 2)
            {
                string mark = parts[0].Trim(); // �}�[�N
                string colorName = parts[1].Trim(); // �F
                Color color = NameToColor(colorName);

                // Dictionary�ɒǉ� _colorToMark 
                if (!_colorToMark.ContainsKey(color))
                {
                    _colorToMark[color] = mark;
                }
                else
                {
                    Debug.LogWarning($"Duplicate color found: {color}, ignoring the second entry.");
                }

                // Dictionary�ɒǉ� _markToColor 
                if (!_markToColor.ContainsKey(mark))
                {
                    _markToColor[mark] = color;
                }
                else
                {
                    Debug.LogWarning($"Duplicate mark found: {mark}, ignoring the second entry.");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }

        Debug.Log($"Loaded {_markToColor.Count} entries from {filePath}");
    }

    /// <summary>
    /// GET color name from color(Type: Color) 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    string ColorToName(Color color)
    {
        if (color == Color.red) return "RED";
        if (color == Color.green) return "GREEN";
        if (color == Color.yellow) return "YELLOW";
        if (color == Color.blue) return "BLUE";

        return "UNKNOWN";
    }

    /// <summary>
    /// GET color from color name (Type: string)
    /// </summary>
    /// <param name="colorName"></param>
    /// <returns></returns>
    Color NameToColor(string colorName)
    {
        if (colorName == "RED") return Color.red;
        if (colorName == "GREEN") return Color.green;
        if (colorName == "YELLOW") return Color.yellow;
        if (colorName == "BLUE") return Color.blue;

        return Color.white;
    }
}
