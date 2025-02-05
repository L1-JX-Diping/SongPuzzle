using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using ColorUtility = UnityEngine.ColorUtility;
using UnityEngine.UI;
using UnityEditor;


public class LyricsController : MonoBehaviour
{
    // 注意: _textField もし名前変更するなら Inspector のところで TextMeshPro みたいなの Row1,2,3 入れなおして
    public TextMeshProUGUI[] _textField; // 歌詞を表示するTextMeshProUGUIオブジェクト（3行分）

    /* private な変数たち */
    // 色弱用 GREEN, *(=Heart) 色とマークの対応
    private Dictionary<Color, string> _avatarDict = new Dictionary<Color, string>();
    private string _lyricsFileName = ""; // 入力ファイル名（Assetsフォルダ内）
    private List<Line> _lyricsList = new List<Line>(); // 歌詞情報(表示開始時刻＋表示する歌詞)を格納するリスト
    private float _loadingTime = 0.8f; // time lag
    private int _currentLyricIndex = 0; // 現在の歌詞インデックス
    private float _clock = 3f; // Second per Beat
    private float _beat = 4; // 何拍子か？ Birthday song は 3 拍子
    private string _eofText = "GAME END.";
    private float _lineStartTime = 0f; // intro 前奏終了時刻 = 歌詞表示(のスクロール用時刻計算)開始時刻
    private int _indexForNumList = 0;
    private List<Color> _colorList = new List<Color>();

    /// <summary>
    /// Which song do you want to play? Set/Get the FILE NAME
    /// </summary>
    public string LyricsFileName { get => _lyricsFileName; set => _lyricsFileName = value; }

    /// <summary>
    /// LyricsList has the information of each line's StartTime/Text/
    /// </summary>
    public List<Line> LyricsList { get => _lyricsList; set => _lyricsList = value; }

    /// <summary>
    /// Get/Set Lag(LoadingTime); To adjust the song accompaniment with the lyrics display scrolling
    /// </summary>
    public float LoadingTime { get => _loadingTime; set => _loadingTime = value; }

    /// <summary>
    /// Clock: second per beat
    /// </summary>
    public float Clock { get => _clock; set => _clock = value; }

    [System.Serializable]
    public class Part
    {
        public float timing; // タイミング
        public string word; // 単語
        public Color color; // 割り当てられた色
        public string avatar; // 色弱用: 割り当てられた色に対応するマーク
    }

    [System.Serializable]
    public class Line
    {
        public float startTime; // 表示時刻（秒単位）
        public string text; // 歌詞内容
        public List<Part> partList = new List<Part>(); // 単語ごとの色情報
    }

    void Start()
    {
        SetLyricsFileName();
        SetColorList(); // color - avatar

        InitAvatarColor(); // 画面上にアバターの色を反映する
        LoadLyricsFile(); // 歌詞ファイルを読み込む

        ExportColorLog(); // 色分け情報を記録
        ExportPartDivision(); // パート分け情報を記録
        UpdateLyricsDisplay(); // 初期表示を更新
    }

    void Update()
    {
        // 現在の時刻に基づいて歌詞を更新
        float currentTime = Time.timeSinceLevelLoad;

        /* Display lyrics */
        int maxIndex = LyricsList.Count - 1;
        Line nextLine = LyricsList[_currentLyricIndex + 1];
        // 次の歌詞行に進むべきタイミングか確認
        if (_currentLyricIndex < maxIndex && currentTime >= nextLine.startTime - LoadingTime)
        {
            _currentLyricIndex++;
            UpdateLyricsDisplay();
        }

        /* Update Avatar Color */
        // Avatar のパートのときに光るなど変化つける
    }
    
    /// <summary>
    /// Set game data to use in this class
    /// </summary>
    private void SetLyricsFileName()
    {
        // set data to GameData class
        GameData.SetAllData();

        // get from the data set to GameData class
        string songTitle = GameData.SongTitle;
        _lyricsFileName = "Lyrics-" + songTitle + ".txt";
    }

    /// <summary>
    /// Get assigned colors and Set into _colorList
    /// </summary>
    private void SetColorList()
    {
        string[] lineList = Common.GetFileContents(FileName.PlayerRole);

        // format: PlayerName, ColorName, Avatar(Mark), Mic
        foreach (string line in lineList)
        {
            // separate by ","
            string[] playerRole = line.Split(',');

            /* set COLOR List */
            string colorName = playerRole[1].Trim(); // 2 列目: color name like "RED"
            Color color = Common.ToColor(colorName);
            _colorList.Add(color);

            /* set AVATAR dictionary */
            string avatar = playerRole[2].Trim(); // 3 行目: avatar like "Spade"
            // Add to Dictionary
            if (!_avatarDict.ContainsKey(color))
            {
                _avatarDict[color] = avatar;
            }
            else
            {
                Debug.LogWarning($"Duplicate color entry found: {colorName}, ignoring the second entry.");
            }
        }
        // for debug
        Debug.Log($"Set _markDict with {_avatarDict.Count} entries, _colorList with {_colorList.Count} entries.");
    }

    void InitAvatarColor()
    {
        foreach (var info in _avatarDict)
        {
            Color color = info.Key;
            string avatar = info.Value;
            Debug.Log($"from _markDict: {Common.ToColorName(color)}, {avatar}");
            ReflectColorToAvatar(avatar, color); // Heart, Color.green
        }
        // did not be used
        // Club 
        ReflectColorToAvatar("Club", Color.gray);
    }

    void ReflectColorToAvatar(string avatarName, Color assignedColor)
    {
        //objName = "Avatar1" 
        // Canvas内の objName オブジェクトを探す
        GameObject avatarObject = GameObject.Find(avatarName);

        // オブジェクトが見つかった場合
        if (avatarObject != null)
        {
            // Imageコンポーネントを取得
            Image avatarImage = avatarObject.GetComponent<Image>();

            if (avatarImage != null)
            {
                // ImageのColorプロパティに色を設定
                avatarImage.color = assignedColor;
                Debug.Log($"Color {assignedColor} applied to {avatarName}.");
            }
            else
            {
                Debug.LogError($"{avatarName} does not have an Image component.");
            }
        }
        else
        {
            Debug.LogError($"{avatarName} object not found in the scene.");
        }
    }

    private void UpdateLyricsDisplay()
    {
        // 真ん中の行を更新するためのインデックス
        int middleLineIndex = 1;

        for (int i = 0; i < _textField.Length; i++)
        {
            // 表示する歌詞行を決定（前後1行 + 現在行）
            int lyricIndex = _currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < LyricsList.Count)
            {
                // テキストを色付きで構築
                string coloredText = "";
                foreach (Part part in LyricsList[lyricIndex].partList)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(part.color);
                    coloredText += $"<color=#{hexColor}>{part.avatar}{part.word}</color> ";
                }

                _textField[i].text = coloredText.Trim();

                // 真ん中の行は不透明、それ以外は半透明
                if (i == middleLineIndex)
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 1f); // 不透明
                }
                else
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 0.2f); // 半透明
                }
            }
            else
            {
                // 歌詞がない場合は空白に設定
                _textField[i].text = "";
            }
        }
    }


    void LoadLyricsFile()
    {
        string path = Path.Combine(Application.dataPath, LyricsFileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"LRC file not found: {path}");
            return;
        }

        string[] lyricsLineList = File.ReadAllLines(path);
        CreateLyricsList(lyricsLineList);

        // Debug
        DebugToConsole();

        Debug.Log($"Loaded {LyricsList.Count} lyrics from {LyricsFileName}");
    }

    private void DebugToConsole()
    {
        Debug.Log("_lyricsList: \n");
        foreach (Line line in LyricsList)
        {
            Debug.Log(line.startTime + ", " + line.text);
        }
    }

    private void CreateLyricsList(string[] lyricsLineList)
    {
        // パート色割り当て用
        List<int> randomNumList = CreateRandomNumList(3);

        // 前奏 intro 部分用
        LyricsList.Add(new Line { startTime = 0.0f, text = "" });

        // meta info part (1行目) の処理
        // bpm と intro を取得
        if (lyricsLineList.Length > 0 && lyricsLineList[0].StartsWith("#"))
        {
            string metaLine = lyricsLineList[0];
            // 曲の speed 情報
            int bpm = ParseMetaLine(metaLine, "bpm");
            _beat = ParseMetaLine(metaLine, "beat");
            int introEndBeat = ParseMetaLine(metaLine, "intro");
            Clock = 60f / (float)bpm; // clock を計算
            // 歌詞スクロール計算の開始時刻
            _lineStartTime = introEndBeat * Clock; // lyricsStartTime
            Debug.Log($"Parsed BPM: {bpm} beats/min, beat: {_beat} count/bar, intro/startTime(init): {introEndBeat} beats, Clock Interval: {Clock:F2} seconds");
        }
        else
        {
            Debug.LogError("Meta information not found in the first line.");
            return;
        }

        // lyrics part (2 行目以降) の処理
        // 歌詞の表示開始時間情報付き lyricsList を作成
        for (int i = 1; i < lyricsLineList.Length; i++)
        {
            string lyricsInfo = lyricsLineList[i];
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
            // 表示「行」の小節数 `2` を bar に保存
            int barCount = int.Parse(match.Groups[1].Value);

            foreach (string timeRatio in match.Groups[2].Value.Split(','))
            {
                // パート開始タイミング `[0,1,3,4]` をリストに変換
                ratioList.Add(int.Parse(timeRatio));
            }
            // 残りの文字列 "Happy birthday to you" を歌詞として取得
            string lyrics = match.Groups[3].Value.Trim();

            // この歌詞行について part 情報をセット
            List<Part> partInfoList = SetPartInfoListForThisLine(randomNumList, ratioList, barCount, lyrics);

            // lyricsList に追加
            LyricsList.Add(new Line
            {
                startTime = _lineStartTime,
                text = lyrics,
                partList = partInfoList
            });

            // 次の行の開始時刻計算
            _lineStartTime += _beat * barCount * Clock; // 6拍 (3拍子 * 2小節) * 0.5秒/拍 = 3秒
        }

        // 終了メッセージを追加
        //float endTime = lyricsStartTime + lines.Length * clock;
        LyricsList.Add(new Line
        {
            startTime = _lineStartTime,
            text = _eofText
        });
        LyricsList.Add(new Line
        {
            startTime = _lineStartTime + 2f,
            text = ""
        });

    }

    private List<Part> SetPartInfoListForThisLine(List<int> randomNumList, List<int> ratioList, int barCount, string lyrics)
    {
        /* LyricLineInfo の partList 情報生成*/
        List<Part> partInfoList = new List<Part>();
        //partInfoList = new List<LyricPartInfo>();

        // この行の歌詞を単語ごとに分割
        string[] wordList = lyrics.Split(' ');
        int index = 0;
        foreach (float timeRatio in ratioList)
        {
            float haku = _beat * barCount; // この行の総拍数
            float addSecond = timeRatio / haku;
            float partStartTime = _lineStartTime + addSecond;
            //Debug.Log($"ratioList:{timeRatio}, partStartTime:{partStartTime}");

            // generate part List
            string word = wordList[index];
            Color color = _colorList[randomNumList[_indexForNumList]];
            string markName = _avatarDict[color];
            string mark = Common.AvatarToLetter(markName); // markName to mark (例: ♠)
            Debug.Log($"add _markDict: {Common.ToColorName(color)}, {mark}");

            // part 情報格納
            Part part = new Part
            {
                timing = partStartTime,
                word = word,
                color = color,
                avatar = mark
            };

            partInfoList.Add(part);
            index++;
            _indexForNumList++;
        }

        return partInfoList;
    }

    List<int> CreateRandomNumList(int num)
    {
        List<int> resultList = new List<int>();
        List<int> candidateList = GenerateCandidateList(num);
        int maxLength = 200; // 作成するリストの長さ（必要に応じて変更）
        int maxRepeats = 2; // 同じ数字が連続できる最大回数

        while (resultList.Count < maxLength)
        {
            int randomIndex = Random.Range(0, candidateList.Count);
            int selectedNum = candidateList[randomIndex];

            // 連続回数をチェック
            if (resultList.Count >= maxRepeats &&
                resultList[resultList.Count - 1] == selectedNum &&
                resultList[resultList.Count - 2] == selectedNum)
            {
                // 条件を満たさない場合は再選択
                continue;
            }

            // 条件を満たす場合、リストに追加
            resultList.Add(selectedNum);
        }

        return resultList;
    }

    List<int> GenerateCandidateList(int num)
    {
        List<int> candidateList = new List<int>();

        // 0から(num-1)までの整数をリストに追加
        for (int i = 0; i < num; i++)
        {
            candidateList.Add(i);
        }

        return candidateList;
    }

    int ParseMetaLine(string metaLine, string key)
    {
        // 指定されたキーの値を正規表現で取得
        var match = Regex.Match(metaLine, $@"{key}\[(\d+)\]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int value))
        {
            return value;
        }

        Debug.LogWarning($"Failed to parse {key} from: {metaLine}");
        return 0;
    }

    void ExportColorLog()
    {
        string logPath = Path.Combine(Application.dataPath, "ForHumanCheck.txt");
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            writer.WriteLine("Lyrics Color Log:");
            foreach (Line thisLine in LyricsList)
            {
                if (thisLine.text == "" || thisLine.text == _eofText)
                {
                    continue;
                }

                // XX.XX という形式で開始時刻をファイルに書き込み
                writer.WriteLine($"[{thisLine.startTime:00.00}]");

                foreach (Part part in thisLine.partList)
                {
                    // パートの歌詞と色を出力
                    writer.WriteLine($"  \"{part.timing}: {part.word}\" - {Common.AvatarToLetter(part.avatar)} {Common.ToColorName(part.color)}, {part.color}");
                }
            }
        }
        Debug.Log($"Color log saved to {logPath}");
    }

    void ExportPartDivision()
    {
        string logPath = Path.Combine(Application.dataPath, FileName.CorrectPart);
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            //writer.WriteLine("Part Log:");
            foreach (Line thisLine in LyricsList)
            {
                if (thisLine.text == "" || thisLine.text == _eofText)
                {
                    continue;
                }

                // XX.XX という形式で開始時刻をファイルに書き込み
                //writer.WriteLine($"[{thisLine.startTime:00.00}] {thisLine.text}");

                foreach (Part part in thisLine.partList)
                {
                    // 誰のパートか？開始時間は？
                    string name = Common.ToColorName(part.color);
                    writer.WriteLine($"{part.timing:00.00}, {name}");
                }
            }
        }
        Debug.Log($"Color log saved to {logPath}");
    }

}
