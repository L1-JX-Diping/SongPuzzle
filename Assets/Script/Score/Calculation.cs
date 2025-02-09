using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Calculation : MonoBehaviour
{
    // 
    public TextMeshProUGUI _scoreTextField;

    private Data _data = new Data();
    private Dictionary<string, Color> _micColorDict = new Dictionary<string, Color>(); // マイクと色の対応
    private List<Detection> _detectionList = new List<Detection>(); // 時刻ごとの検出情報
    //private List<Part> _correctParts = new List<Part>();

    private float _score = 0f;
    // 歌い出し許容誤差
    private float _threshold = 0.3f; // change mode

    void Start()
    {
        // init 
        LoadData();
        ReformData();

        CalculateScore();
        
        DisplayScore(_score);
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        _detectionList = (List<Detection>)Common.LoadXml(_detectionList.GetType(), FileName.XmlMicLog);
    }

    /// <summary>
    /// 
    /// </summary>
    void ReformData()
    {
        /* Set _micColorDict */
        MakeMicColorDict();

        /* Set correct parts */
        GetCorrectPartList();
    }

    /// <summary>
    /// 
    /// </summary>
    private void MakeMicColorDict()
    {
        List<Player> playerList = _data.Team.MemberList;

        foreach (Player player in playerList)
        {
            _micColorDict[player.Role.Mic] = player.Role.Color;
        }

        Debug.Log("MicColorInfo loaded successfully.");
    }

    /// <summary>
    /// 
    /// </summary>
    private List<Part> GetCorrectPartList()
    {
        List<Line> lyricsList = _data.Song.Lines;
        List<Part> correctParts = new List<Part>();

        foreach (Line line in lyricsList)
        {
            foreach (Part part in line.PartList)
            {
                correctParts.Add(part);
            }
        }

        Debug.Log("PartLog loaded successfully.");

        return correctParts;
    }

    /// <summary>
    /// 
    /// </summary>
    void CalculateScore()
    {
        List<Part> correctParts = GetCorrectPartList();
        int totalScore = 0;

        foreach (var part in correctParts)
        {
            // Robot パートは自動的に正解
            if (part.Player.Role.IsRobot == true)
            {
                totalScore++;
                continue;
            }

            // 該当時刻の最大音量マイクを取得
            Detection maxDetection = null;
            foreach (Detection detection in _detectionList)
            {
                // comparing time
                // using Abs method instead of the method "Mathf.Approximately(time1, time2)"
                if (Mathf.Abs(part.Timing - detection.Time) < _threshold)
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
                totalScore++;
            }
        }

        // score format: x / 100
        int maxScore = correctParts.Count;
        _score = (float)totalScore / maxScore * 100f;
        Debug.Log($"Score: {totalScore}/{maxScore} ({_score:F2}%)");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    private void DisplayScore(float score)
    {
        string displayText = "";
        //float score = Convert.ToSingle(sum) / 300 * 100;
        displayText += $"{score:00.00}\n";

        if (_scoreTextField != null)
        {
            _scoreTextField.text = displayText;
        }
        else
        {
            Debug.LogError("Text field is not assigned in the inspector.");
        }
    }

}
