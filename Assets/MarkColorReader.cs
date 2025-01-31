using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarkColorReader : MonoBehaviour
{
    // 情報源ファイル名
    private string _markColorFileName = "MarkColorDict.txt";
    // マークと色の対応を格納 Dictionary
    private Dictionary<string, Color> _markToColor = new Dictionary<string, Color>();
    private Dictionary<Color, string> _colorToMark = new Dictionary<Color, string>();

    void Start()
    {
        LoadMarkColorDict();

        // 確認のためにログを出力
        foreach (var entry in _markToColor)
        {
            Debug.Log($"Mark: {entry.Key}, Color: {entry.Value}");
        }
    }

    void LoadMarkColorDict()
    {
        // ファイルパスを取得
        string filePath = Path.Combine(Application.dataPath, _markColorFileName);

        // ファイルが存在しない場合
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        // ファイルを行ごとに読み込み
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // カンマで分割してマークと色を取得
            string[] parts = line.Split(',');

            if (parts.Length == 2)
            {
                string mark = parts[0].Trim(); // マーク
                string colorName = parts[1].Trim(); // 色
                Color color = NameToColor(colorName);

                // Dictionaryに追加 _colorToMark 
                if (!_colorToMark.ContainsKey(color))
                {
                    _colorToMark[color] = mark;
                }
                else
                {
                    Debug.LogWarning($"Duplicate color found: {color}, ignoring the second entry.");
                }

                // Dictionaryに追加 _markToColor 
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
