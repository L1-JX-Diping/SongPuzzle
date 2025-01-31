using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class Test2 : MonoBehaviour
{
    public TextMeshProUGUI[] _textField; // �̎���\������TextMeshProUGUI�I�u�W�F�N�g�i3�s���j
    public string _lyricsFileName = "Lyrics-BirthdaySong.txt"; // ���̓t�@�C�����iAssets�t�H���_���j
    private List<LyricLineInfo> lyricsList = new List<LyricLineInfo>(); // �̎������i�[���郊�X�g
    private float _loadingTime = 0.8f;
    private int currentLyricIndex = 0; // ���݂̉̎��C���f�b�N�X
    private float lyricsStartTime = 0f; // �̂̎n�܂�̎���
    private float clock = 3f; // Second per Beat
    private float beat = 4; // �����q���H
    private float lineStartTime = 0f; // intro �O�t�I������ = �̎��\��(�̃X�N���[���p�����v�Z)�J�n����
    private Color[] colorList = { Color.red, Color.green, Color.yellow }; // �g�p����3�F

    [System.Serializable]
    public class LyricPartInfo
    {
        public string word; // �P��
        public Color color; // ���蓖�Ă�ꂽ�F
    }

    [System.Serializable]
    public class LyricLineInfo
    {
        public float startTime; // �\�������i�b�P�ʁj
        public string text; // �̎����e
        public List<LyricPartInfo> parts = new List<LyricPartInfo>(); // �P�ꂲ�Ƃ̐F���
    }

    void Start()
    {
        LoadLyricsFile(); // �t�@�C����ǂݍ���
        AssignRandomColors(); // �P�ꂲ�ƂɃ����_���ɐF�����蓖��
        ExportColorLog(); // �F���������L�^
        UpdateLyricsDisplay(); // �����\�����X�V
        //lineStartTime = _loadingTime;
    }

    void Update()
    {
        // ���݂̎����Ɋ�Â��ĉ̎����X�V
        float currentTime = Time.timeSinceLevelLoad;

        // ���̉̎��s�ɐi�ނׂ��^�C�~���O���m�F
        if (currentLyricIndex < lyricsList.Count - 1 && currentTime >= lyricsList[currentLyricIndex + 1].startTime - _loadingTime)
        {
            currentLyricIndex++;
            UpdateLyricsDisplay();
        }
    }

    void LoadLyricsFile()
    {
        string path = Path.Combine(Application.dataPath, _lyricsFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"LRC file not found: {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);

        // 1�s�ڂ��� bpm �� intro ���擾
        if (lines.Length > 0 && lines[0].StartsWith("#"))
        {
            string metaLine = lines[0];
            // �Ȃ� speed ���
            int bpm = ParseMetaLine(metaLine, "bpm");
            beat = ParseMetaLine(metaLine, "beat");
            int introEndBeat = ParseMetaLine(metaLine, "intro");
            clock = 60f / (float)bpm; // clock ���v�Z
            // �̎��X�N���[���v�Z�J�n����
            lyricsStartTime = introEndBeat * clock;
            Debug.Log($"Parsed BPM: {bpm} beats/min, beat: {beat} count/bar, intro/startTime(init): {introEndBeat} beats, Clock Interval: {clock:F2} seconds");
        }
        else
        {
            Debug.LogError("Meta information not found in the first line.");
            return;
        }

        //* 2 �s�ڈȍ~���̎��Ƃ��ď��� *//
        for (int i = 1; i < lines.Length; i++)
        {
            string lyricsPart = lines[i];
            // Line ���ƂɍX�V
            List<int> ratioList = new List<int>();
            List<float> timeList = new List<float>();

            // ���K�\�����g�p���ăf�[�^�𒊏o // ��: 2[0,1,3,4]Happy birthday to you
            Regex regex = new Regex(@"(\d+)\[([0-9,]+)\](.*)");
            Match match = regex.Match(lyricsPart);
            if (match.Success)
            {
                // ����: bar �� �����䗦List: ratioList �𒊏o
                int bar = int.Parse(match.Groups[1].Value); // `2` �� bar �ɕۑ�

                foreach (string timeRatio in match.Groups[2].Value.Split(','))
                {
                    ratioList.Add(int.Parse(timeRatio)); // `[0,1,3,4]` �����X�g�ɕϊ�
                }
                string lyrics = match.Groups[3].Value.Trim(); // �c��̕�������̎��Ƃ��Ď擾

                // ���ʂ��m�F
                //Debug.Log("bar: " + bar + ", ratioList: [" + string.Join(", ", ratioList) + "]" + ", lyrics: " + lyrics);

                // 2 �s�ڂ������ꏈ��: �̎� 1 �s�� 
                if (i == 1)
                {
                    // �V�[���J�n�Ɠ����ɕ\�������� startTime = 0.0f
                    lyricsList.Add(new LyricLineInfo { startTime = 0.0f, text = lyrics });
                    lineStartTime = lyricsStartTime;
                    // For Riri
                    //timeList = CalcTimeList(ratioList);

                    Debug.Log("continue;");
                    continue;
                }

                Debug.Log("No continue;");
                // 3 �s�ڈȍ~ // ���̉̎��s�̊J�n����
                lineStartTime += beat * bar * clock; // 3���q * 2���� --> 6���q * 0.5�b/���q = 3�b
                // For Riri // timeList �v�Z
                //timeList = CalcTimeList(ratioList);

                // lyricsList �ɒǉ�
                lyricsList.Add(new LyricLineInfo { startTime = lineStartTime, text = lyrics });
            }
        }

        // �I�����b�Z�[�W��ǉ�
        //float endTime = lyricsStartTime + lines.Length * clock;
        lyricsList.Add(new LyricLineInfo { startTime = lineStartTime + 3f, text = "" });
        lyricsList.Add(new LyricLineInfo { startTime = lineStartTime + 3f, text = "GAME END." });

        Debug.Log($"Loaded {lyricsList.Count} lyrics from {_lyricsFileName}");
    }

    List<float> CalcTimeList(List<int> ratioList)
    {
        // startTime �X�V (���̍s�̉̎��\���J�n�����v�Z)
        int index = 0;
        List<float> timeList = new List<float>();
        foreach (int ratio in ratioList)
        {
            // ratio [/]
            timeList[index] = lineStartTime + clock * (float)ratio;
            Debug.Log($"timeList[{index}]: {timeList[index]}");
            index++;
        }
        return timeList;
    }

    int ParseMetaLine(string metaLine, string key)
    {
        // �w�肳�ꂽ�L�[�̒l�𐳋K�\���Ŏ擾
        var match = System.Text.RegularExpressions.Regex.Match(metaLine, $@"{key}\[(\d+)\]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
        {
            return value;
        }

        Debug.LogWarning($"Failed to parse {key} from: {metaLine}");
        return 0;
    }

    void AssignRandomColors()
    {
        foreach (var line in lyricsList)
        {
            string[] wordList = line.text.Split(' '); // �P�ꂲ�Ƃɕ���
            foreach (var word in wordList)
            {
                int randomIndex = Random.Range(0, colorList.Length); // �����_���ŐF��I��
                line.parts.Add(new LyricPartInfo { word = word, color = colorList[randomIndex] });
            }
        }
    }

    void ExportColorLog()
    {
        string logPath = Path.Combine(Application.dataPath, "LyricsColorLog.txt");
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            writer.WriteLine("Lyrics Color Log:");
            foreach (var line in lyricsList)
            {
                writer.WriteLine($"[{line.startTime:00.00}]");
                foreach (var part in line.parts)
                {
                    string colorName = ColorToName(part.color);
                    writer.WriteLine($"  \"{part.word}\" - {colorName}");
                }
            }
        }
        Debug.Log($"Color log saved to {logPath}");
    }

    string ColorToName(Color color)
    {
        if (color == Color.red) return "RED";
        if (color == Color.green) return "GREEN";
        if (color == Color.yellow) return "YELLOW";
        return "UNKNOWN";
    }

    void UpdateLyricsDisplay()
    {
        // �^�񒆂̍s���X�V���邽�߂̃C���f�b�N�X
        int middleLineIndex = 1;

        for (int i = 0; i < _textField.Length; i++)
        {
            // �\������̎��s������i�O��1�s + ���ݍs�j
            int lyricIndex = currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < lyricsList.Count)
            {
                // �e�L�X�g��F�t���ō\�z
                string coloredText = "";
                foreach (var part in lyricsList[lyricIndex].parts)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(part.color);
                    coloredText += $"<color=#{hexColor}>{part.word}</color> ";
                }

                _textField[i].text = coloredText.Trim();

                // �^�񒆂̍s�͕s�����A����ȊO�͔�����
                if (i == middleLineIndex)
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 1f); // �s����
                }
                else
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 0.2f); // ������
                }
            }
            else
            {
                // �̎����Ȃ��ꍇ�͋󔒂ɐݒ�
                _textField[i].text = "";
            }
        }
    }
}