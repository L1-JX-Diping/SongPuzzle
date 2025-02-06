using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Calculation : MonoBehaviour
{
    public TextMeshProUGUI _scoreDisplayText;

    private Data _data = new Data();

    private Dictionary<string, Color> _micColorDict = new Dictionary<string, Color>(); // マイクと色の対応
    //private List<Part> _partInfoList = new List<Part>(); // 時刻と色・マイクの正解情報
    private List<Detection> _detectionList = new List<Detection>(); // 時刻ごとの検出情報
    private List<Part> _correctParts = new List<Part>();

    private int _totalScore = 0; // 合計スコア
    //private int _maxScore = 0;   // 最大スコア (100点満点換算用)

    void Start()
    {
        LoadData();
        
        LoadMicDetectionLog();
        CalculateScore();
    }

    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        SetData();
        SetCorrectPart();
    }

    void SetData()
    {
        /* Set _micColorDict */
        List<Player> playerList = _data.Team.MemberList;

        foreach (Player player in playerList)
        {
            _micColorDict[player.Role.Mic] = player.Role.Color;
        }

        Debug.Log("MicColorInfo loaded successfully.");

        /* Set correct parts */
        List<Line> lyricsList = _data.Song.Lyrics;

        foreach (Line line in lyricsList)
        {
            foreach (Part part in line.PartList)
            {
                _correctParts.Add(part);
            }
        }
        
        Debug.Log("PartLog loaded successfully.");
    }

    void SetCorrectPart()
    {
    }

    void LoadMicDetectionLog()
    {
        string[] lineList = Common.GetTXTFileLineList(FileName.MicDitection);

        foreach (string line in lineList)
        {
            if (line.StartsWith("#")) continue; // コメント行をスキップ

            string[] parts = line.Split(',');
            if (parts.Length == 3 && float.TryParse(parts[0].Trim(), out float time) &&
                float.TryParse(parts[2].Trim(), out float volume))
            {
                string mic = parts[1].Trim();
                _detectionList.Add(new Detection 
                { 
                    Timing = time, 
                    Mic = mic, 
                    Volume = volume 
                });
            }
        }

        Debug.Log("MicDetectionLog loaded successfully.");
    }

    void CalculateScore()
    {
        _totalScore = 0;
        int maxScore = _correctParts.Count;

        foreach (var part in _correctParts)
        {
            // Robotパートは自動的に正解
            if (part.Player.Role.Mic == "Robot")
            {
                _totalScore++;
                continue;
            }

            // 該当時刻の最大音量マイクを取得
            Detection maxDetection = null;
            foreach (Detection detection in _detectionList)
            {
                if (Mathf.Approximately(detection.Timing, part.Timing))
                {
                    if (maxDetection == null || detection.Volume > maxDetection.Volume)
                    {
                        maxDetection = detection;
                    }
                }
            }

            // 最大音量マイクが正解と一致するか確認
            if (maxDetection != null && maxDetection.Mic == part.Player.Role.Mic)
            {
                _totalScore++;
            }
        }

        // スコアを100点満点に換算
        float percentageScore = (float)_totalScore / maxScore * 100f;
        Debug.Log($"Score: {_totalScore}/{maxScore} ({percentageScore:F2}%)");
        DisplayScore(percentageScore);
    }

    void DisplayScore(float result)
    {
        string displayText = "";
        //float result = Convert.ToSingle(sum) / 300 * 100;
        displayText += $"{result:00.00}\n";

        if (_scoreDisplayText != null)
        {
            _scoreDisplayText.text = displayText;
        }
        else
        {
            Debug.LogError("ScoreDisplayText is not assigned in the inspector.");
        }
    }

}
