using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Xml;
[System.Serializable]
public class ColorLog
{
    public List<ColorLogLine> ColorLogLines = new List<ColorLogLine>();
}
[System.Serializable]
public class ColorLogLine
{
    public int LineIndex;
    public List<ColorLogPart> Parts = new List<ColorLogPart>();
}
[System.Serializable]
public class ColorLogPart
{
    public string Text;
    public string Color;
}
public class DisplayLyrics : MonoBehaviour
{
    public TextMeshProUGUI textComponent;   // �\���p��TextMeshProUGUI
    public string colorLogFileName = "Assets/ColorLog.json"; // �F���̃��O�t�@�C��
    private ColorLog colorLog;             // JSON�f�[�^��ێ�
    private int currentLineIndex = 0;      // ���ݕ\�����̐擪�s
    private Color[] colors = { Color.red, Color.green, Color.blue }; // �g�p����3�F
    private string[] colorNames = { "RED", "GREEN", "BLUE" }; // �F���Ή�
    private int colorIndex = 0;
    void Start()
    {
        // �ŏ��̐F�����_������
        colorIndex = Random.Range(0, colors.Length);
        // �̂��o���͂��̐l���炾����Ă��m�点������

        // Step 1: �����_���F�������L�^
        GenerateColorLog();
        // Step 2: �F�������[�h
        LoadColorLog();
        // Step 3: �X�N���[���\�����J�n
        StartCoroutine(ScrollLyricsCoroutine());
    }

    /// <summary>
    /// �P�ꂲ�ƂɐF�����i�p�[�g�����j
    /// �����瓯���F���A�����Ă�����
    /// ��������Ɠ����F�A����������B�B�B
    /// ����ϕs�̗p�ł�����
    /// </summary>
    //void GenerateColorLog()
    //{
    //    string inputFileName = "Lyrics.xml"; // �ǂݍ��މ̎��t�@�C��
    //    string path = Path.Combine(Application.dataPath, inputFileName);
    //    if (!File.Exists(path))
    //    {
    //        Debug.LogError($"File not found: {path}");
    //        return;
    //    }
    //    colorLog = new ColorLog();
    //    XmlDocument xmlDoc = new XmlDocument();
    //    xmlDoc.Load(path);
    //    XmlNodeList lines = xmlDoc.SelectNodes("/Lyrics/Line");
    //    for (int i = 0; i < lines.Count; i++)
    //    {
    //        ColorLogLine logLine = new ColorLogLine { LineIndex = i + 1 };
    //        string lineText = lines[i].InnerText;
    //        string[] parts = lineText.Split(' ');
    //        foreach (string part in parts)
    //        {
    //            int colorIndex = Random.Range(0, colors.Length);
    //            ColorLogPart logPart = new ColorLogPart
    //            {
    //                Text = part.Trim(),
    //                Color = colorNames[colorIndex]
    //            };
    //            logLine.Parts.Add(logPart);
    //        }
    //        colorLog.ColorLogLines.Add(logLine);
    //    }
    //    // �F����JSON�Ƃ��ĕۑ�
    //    string json = JsonUtility.ToJson(colorLog, true);
    //    File.WriteAllText(colorLogFileName, json);
    //    Debug.Log($"Color log saved to {colorLogFileName}");
    //}

    /// <summary>
    /// �J���}�Ńp�[�g���������t�@�C������͂ɂƂ����ꍇ
    /// </summary>
    void GenerateColorLog()
    {
        string inputFileName = "Orders.xml"; // �ǂݍ��މ̎��t�@�C��
        string path = Path.Combine(Application.dataPath, inputFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return;
        }
        colorLog = new ColorLog();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlNodeList lines = xmlDoc.SelectNodes("/Lyrics/Line");
        for (int i = 0; i < lines.Count; i++)
        {
            ColorLogLine logLine = new ColorLogLine { LineIndex = i + 1 };
            string lineText = lines[i].InnerText;
            string[] parts = lineText.Split(',');
            int lastColorIndex = -1;
            foreach (string part in parts)
            {
                while (lastColorIndex == colorIndex)
                {
                    colorIndex = Random.Range(0, colors.Length);
                }
                ColorLogPart logPart = new ColorLogPart
                {
                    Text = part.Trim(),
                    Color = colorNames[colorIndex]
                };
                logLine.Parts.Add(logPart);
                // ���̐F���L���i���̃p�[�g�A�������F���蓖�ĂȂ��悤�j
                lastColorIndex = colorIndex;
            }
            colorLog.ColorLogLines.Add(logLine);
        }
        // �F����JSON�Ƃ��ĕۑ�
        string json = JsonUtility.ToJson(colorLog, true);
        File.WriteAllText(colorLogFileName, json);
        Debug.Log($"Color log saved to {colorLogFileName}");
    }

    /// <summary>
    /// �P����Ńp�[�g���������t�@�C������͂ɂƂ����ꍇ
    /// </summary>
    //void GenerateColorLog()
    //{
    //    string inputFileName = "Lyrics.xml"; // �ǂݍ��މ̎��t�@�C��
    //    string path = Path.Combine(Application.dataPath, inputFileName);
    //    if (!File.Exists(path))
    //    {
    //        Debug.LogError($"File not found: {path}");
    //        return;
    //    }
    //    colorLog = new ColorLog();
    //    XmlDocument xmlDoc = new XmlDocument();
    //    xmlDoc.Load(path);
    //    XmlNodeList lines = xmlDoc.SelectNodes("/Lyrics/Line");
    //    for (int i = 0; i < lines.Count; i++)
    //    {
    //        ColorLogLine logLine = new ColorLogLine { LineIndex = i + 1 };
    //        string lineText = lines[i].InnerText;
    //        string[] parts = lineText.Split(' ');
    //        int lastColorIndex = -1;
    //        foreach (string part in parts)
    //        {
    //            while (lastColorIndex == colorIndex)
    //            {
    //                colorIndex = Random.Range(0, colors.Length);
    //            }
    //            ColorLogPart logPart = new ColorLogPart
    //            {
    //                Text = part.Trim(),
    //                Color = colorNames[colorIndex]
    //            };
    //            logLine.Parts.Add(logPart);
    //            // ���̐F���L���i���̃p�[�g�A�������F���蓖�ĂȂ��悤�j
    //            lastColorIndex = colorIndex;
    //        }
    //        colorLog.ColorLogLines.Add(logLine);
    //    }
    //    // �F����JSON�Ƃ��ĕۑ�
    //    string json = JsonUtility.ToJson(colorLog, true);
    //    File.WriteAllText(colorLogFileName, json);
    //    Debug.Log($"Color log saved to {colorLogFileName}");
    //}
    void LoadColorLog()
    {
        if (File.Exists(colorLogFileName))
        {
            string json = File.ReadAllText(colorLogFileName);
            colorLog = JsonUtility.FromJson<ColorLog>(json);
            Debug.Log("Color log loaded.");
        }
        else
        {
            Debug.LogError($"Color log file not found: {colorLogFileName}");
        }
    }
    IEnumerator ScrollLyricsCoroutine()
    {
        while (currentLineIndex < colorLog.ColorLogLines.Count)
        {
            // ���݂̍s�Ǝ��̍s���擾
            ColorLogLine line1 = colorLog.ColorLogLines[currentLineIndex];
            ColorLogLine line2 = currentLineIndex + 1 < colorLog.ColorLogLines.Count
                ? colorLog.ColorLogLines[currentLineIndex + 1]
                : null;
            // �e�L�X�g���\�z
            int rowNum = currentLineIndex + 1;
            // 
            string displayedText = rowNum.ToString() + BuildTextWithColors(line1, 1f);
            if (line2 != null)
            {
                displayedText += "\n" + (rowNum + 1).ToString() + BuildTextWithColors(line2, 0.3f);
            }
            // �\���e�L�X�g���X�V
            textComponent.text = displayedText;
            // 3.5�b���Ƃɕ\���̎��X�V
            currentLineIndex++;
            yield return new WaitForSeconds(3.5f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="line"></param> �����������s�̒��g�ifrom ColorLog �t�@�C���j
    /// <param name="colorAlpha"></param> �F�̔Z���w�� 0�`1
    /// <returns></returns>
    string BuildTextWithColors(ColorLogLine line, float colorAlpha)
    {
        string text = "";
        // ColorLog �t�@�C������擾�������̂P�s�̒��Ńp�[�g�������ƂɐF�ݒ蔽�f
        foreach (ColorLogPart part in line.Parts)
        {
            // ColorLog ����F�����擾���AColor �^�ɕϊ�
            Color color = ParseColor(part.Color);
            Debug.Log("color rgb: " + color.r + color.g + color.b);
            Debug.Log("color a: " + color.a);
            // �\������Z��
            Color finalColor = SetAlpha(color, colorAlpha);
            Debug.Log("color rgb: " + finalColor.r + finalColor.g + finalColor.b);
            Debug.Log("color a: " + finalColor.a);
            // Color �^�� 16 �i�J���[�R�[�h�ɕϊ����� <color> �^�O��}��
            string colorCode = ColorToCode(SetAlpha(color, colorAlpha));
            text += $"<color={colorCode}>{part.Text}</color> ";
        }
        return text.Trim();
    }

    /// <summary>
    /// �J���[�l�[������J���[�^��
    /// </summary>
    /// <param name="colorName"></param>
    /// <returns></returns>
    Color ParseColor(string colorName)
    {
        switch (colorName)
        {
            case "RED": return Color.red;
            case "GREEN": return Color.green;
            case "BLUE": return Color.blue;
            default: return Color.white;
        }
    }

    /// <summary>
    /// �\���̎��̐F�̔Z������
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    // ColorToHex : Unity��Color �^�� HTML �`���� 16 �i�J���[�R�[�h�ɕϊ�
    string ColorToCode(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
}