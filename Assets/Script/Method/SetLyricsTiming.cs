using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// output LyricsDivision.xml
/// </summary>
public class SetLyricsTiming 
{
    // access to game data
    Data _data = new Data();

    /* private な変数たち */
    private string _lyricsFileName = ""; // 入力ファイル名（Assetsフォルダ内）
    // for loading from file
    private List<Player> _playerList = new List<Player>();
    // for creating and exporting to file
    private List<Line> _lyrics = new List<Line>(); // lyrics information 
    // 
    private float _beatSec = 3f; // (Seconds per Beat): beatSec = 60f / (float)BPM
    private int _signature = 4; // 何拍子か？ Birthday song は 3 拍子
    private string _eofText = "GAME END.";

    /// <summary>
    /// Divide lyrics into parts randomly and Save the data after dividing
    /// </summary>
    public List<Line> DoDivisionWithTiming()
    {
        // initiallize game data such as _playerRole
        LoadData();

        LoadLyricsFile(); // Load lyrics 

        // Save information to files
        SaveData();

        return _lyrics;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SaveData()
    {
        _data.Song.Lines = _lyrics;
        Common.ExportToXml(_data, FileName.XmlGameData); // update song lirics division
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);

        _playerList = _data.Team.MemberList;

        string songTitleNoSpace = _data.Song.Title.Replace(" ", "");
        _lyricsFileName = "Lyrics-" + songTitleNoSpace + ".txt";
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadLyricsFile()
    {
        string[] lyricsLineList = Common.GetTXTFileContents(_lyricsFileName);

        // Lyrics division
        CreateLyricsList(lyricsLineList);

        // Debug
        //DebugToConsole();

    }

    private void DebugToConsole()
    {
        Debug.Log("Lyrics: \n");
        foreach (Line line in _lyrics)
        {
            Debug.Log(line.Timing + ", " + line.Text);
        }

        Debug.Log($"Loaded {_lyrics.Count} lyrics from {_lyricsFileName}");
    }

    private void CreateLyricsList(string[] lineList)
    {
        float lineTiming = 0f;

        // 前奏 intro 部分用
        _lyrics.Add(new Line 
        { 
            Timing = 0.0f, 
            Text = "" 
        });

        // meta info part (1行目) の処理
        // bpm と intro を取得
        if (lineList.Length > 0 && lineList[0].StartsWith("#"))
        {
            string metaLine = lineList[0];
            // 曲の speed 情報
            int bpm = ParseMetaLine(metaLine, "bpm");
            _signature = ParseMetaLine(metaLine, "beat");
            int introEndBeat = ParseMetaLine(metaLine, "intro");
            _beatSec = 60f / (float)bpm; // clock を計算
            // 歌詞スクロール計算の開始時刻
            lineTiming = introEndBeat * _beatSec; // lyricsStartTime
            Debug.Log($"Parsed BPM: {bpm} beats/min, beat: {_signature} count/bar, intro/startTime(init): {introEndBeat} beats, clock Interval: {_beatSec:F2} seconds");
        }
        else
        {
            Debug.LogError("Meta information not found in the first line.");
            return;
        }

        // lyrics part (2 行目以降) の処理
        // 歌詞の表示開始時間情報付き lyricsList を作成
        for (int i = 1; i < lineList.Length; i++)
        {
            string lyricsInfo = lineList[i];
            // Line ごとに更新
            List<int> ratioList = new List<int>();

            // 正規表現を使用してデータを抽出 
            // 例: 2[0,1,3,4]Happy birthday to you
            Regex regex = new Regex(@"(\d+)\[([0-9,]+)\](.*)");
            Match match = regex.Match(lyricsInfo);
            if (!match.Success)
            {
                Debug.LogError("match unsuccessful. Please write lyrics information to input file like \"2[0,1,3,4]Happy birthday to you\"");
                continue;
            }

            /* match.Success なら
             * 小節数: bar と 時刻比率List: ratioList を抽出 */
            // 表示行の小節数 `2` を bar に保存
            //int barCount = int.Parse(match.Groups[1].Value);
            int barCount = Common.ToInt(match.Groups[1].Value);

            foreach (string timeRatio in match.Groups[2].Value.Split(','))
            {
                // パート開始タイミング `[0,1,3,4]` をリストに変換
                //ratioList.Add(int.Parse(timeRatio));
                ratioList.Add(Common.ToInt(timeRatio));
            }
            // 残りの文字列 "Happy birthday to you" を歌詞として取得
            string lyrics = match.Groups[3].Value.Trim();

            // この歌詞行について part 情報をセット
            List<Part> partList = SetPartListForThisLine(ratioList, barCount, lyrics, lineTiming);

            // Add to lyricsList 
            _lyrics.Add(new Line
            {
                Timing = lineTiming,
                Text = lyrics,
                PartList = partList
            });

            // 次の行の開始時刻計算
            lineTiming += _signature * barCount * _beatSec; // 6拍 (3拍子 * 2小節) * 0.5秒/拍 = 3秒
        }

        // 終了メッセージを追加
        //float endTime = lyricsStartTime + lines.Length * clock;
        _lyrics.Add(new Line
        {
            Timing = lineTiming,
            Text = _eofText
        });
        _lyrics.Add(new Line
        {
            Timing = lineTiming + 2f,
            Text = ""
        });

    }

    private List<Part> SetPartListForThisLine(List<int> ratioList, int barCount, string lyrics, float lineStartTime)
    {
        // パート色割り当て用
        List<int> order = Common.GetRandomRoleOrder(_playerList.Count, 2);
        int index = 0;

        /* LyricLineInfo の partList 情報生成*/
        List<Part> partList = new List<Part>();

        // この行の歌詞を単語ごとに分割
        string[] wordList = lyrics.Split(' ');
        int i = 0;
        foreach (float timeRatio in ratioList)
        {
            /* calculate time to begin singing */
            float haku = _signature * barCount; // この行の総拍数
            float timeGap = timeRatio / haku;
            float partStartTime = lineStartTime + timeGap;
            //Debug.Log($"ratioList:{timeRatio}, partStartTime:{partStartTime}");

            /* generate part List */
            // lyrics(word) of this part
            string word = wordList[i];
            // if index out of range --> order をもう一度始めから回す
            if (index > order.Count) index = 0;
            // select a player from _playerList 
            Player player= _playerList[order[index]];

            //Debug.Log($"add _markDict: {Common.ToColorName(color)}, {mark}");

            // part 情報格納
            Part part = new Part
            {
                Timing = partStartTime,
                Text = word,
                Player = player
            };

            partList.Add(part);
            i++;
            index++;
        }

        return partList;
    }

    int ParseMetaLine(string metaLine, string key)
    {
        // 指定されたキーの値を正規表現で取得
        Match match = Regex.Match(metaLine, $@"{key}\[(\d+)\]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
        {
            return value;
        }

        Debug.LogWarning($"Failed to parse {key} from: {metaLine}");
        return 0;
    }

}
