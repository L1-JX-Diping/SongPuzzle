using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement; // Scene の切り替えしたい場合に必要な宣言


public class DisplayLyrics : MonoBehaviour
{
    // 注意: _textField もし名前変更するなら Inspector のところで TextMeshPro みたいなの Row1,2,3 入れなおして
    public TextMeshProUGUI[] _textField; // 歌詞を表示するTextMeshProUGUIオブジェクト（3行分）

    Data _data = new Data();
    private int _currentLyricIndex = 0; // 現在の歌詞インデックス
    private float _loadingTime = 0.8f; // time lag
    // Load _lyricsList // 歌詞情報(表示開始時刻＋表示する歌詞)を格納するリスト
    private List<Line> _lyricsLines = new List<Line>(); 
    // Load _playerList 
    private List<Player> _playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        Division division = new Division();
        division.DoDivision();

        // get division data
        LoadAndSetData();

        InitAvatarColor(); // 画面上にアバターの色を反映する

        UpdateLyricsDisplay(); // 初期表示を更新
    }

    // Update is called once per frame
    void Update()
    {
        // 現在の時刻に基づいて歌詞を更新
        float currentTime = Time.timeSinceLevelLoad;

        /* Update(Scroll) lyrics displayed on the screen */
        int maxIndex = _lyricsLines.Count - 1;
        if (_currentLyricIndex >= maxIndex)
        {
            SwitchScene();
            return;
        }
        Line nextLine = _lyricsLines[_currentLyricIndex + 1];

        // 次の歌詞行に進むべきタイミングか確認
        if (_currentLyricIndex < maxIndex && currentTime >= nextLine.Timing - _loadingTime)
        {
            _currentLyricIndex++;
            UpdateLyricsDisplay();
        }

        /* Update Avatar Color */
        // Avatar のパートのときに光るなど変化つける
    }

    /// <summary>
    /// 
    /// </summary>
    private void SwitchScene()
    {
        SceneManager.LoadScene("Score");

        //if (true)
        //{
        //    // if finish to play this song, swith scene to "Score"
        //    SceneManager.LoadScene("Score");
        //}
        //else
        //{
        //    // 
        //    SceneManager.LoadScene("Home");
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadAndSetData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);

        _lyricsLines = _data.Song.Lines;
        _playerList = _data.Team.MemberList;

        // for Debug
        foreach (Line line in _lyricsLines)
        {
            Debug.Log($"Load _lyricsList: {line.Timing}: {line.Text}");
        }
        foreach (Player player in _playerList)
        {
            Debug.Log($"Load _playerList: {player.Name} is expected to sing {Common.ToColorName(player.Role.Color)} parts.");
        }
    }

    /// <summary>
    /// Update screen: set color to avatar when this scene load
    /// </summary>
    private void InitAvatarColor()
    {
        foreach (Player player in _playerList)
        {
            Color color = player.Role.Color; // like Color.green, Color.yellow...
            string avatar = player.Role.Avatar; // like "Heart", "Spade"...
            Debug.Log($"set _markDict: {Common.ToColorName(color)}, {avatar}");
            ReflectOnScreen(avatar, color); 
        }
        // did not be used
        // Club 
        ReflectOnScreen("Club", Color.gray);
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateLyricsDisplay()
    {
        // 真ん中の行を更新するためのインデックス
        int middleLineIndex = 1;
        for (int i = 0; i < _textField.Length; i++)
        {
            // 表示する歌詞行を決定（前後1行 + 現在行）
            int lyricIndex = _currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < _lyricsLines.Count)
            {
                // テキストを色付きで構築
                string coloredText = "";
                foreach (Part part in _lyricsLines[lyricIndex].PartList)
                {
                    Role role = part.Player.Role;
                    string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
                    coloredText += $"<color=#{hexColor}>{Common.AvatarToLetter(role.Avatar)}{part.Word}</color> ";
                }

                _textField[i].text = coloredText.Trim();

                // handle the level of transparent
                // 真ん中の行は不透明、それ以外は半透明
                if (i == middleLineIndex)
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 1f); // 不透明 opaque
                }
                else
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 0.2f); // 半透明 translucent
                }
            }
            else
            {
                // 歌詞がない場合は空白に設定
                _textField[i].text = "";
            }
        }
    }

    /// <summary>
    /// Set color to object place on the screen
    /// </summary>
    /// <param name="avatarName"></param>
    /// <param name="assignedColor"></param>
    void ReflectOnScreen(string avatarName, Color assignedColor)
    {
        // Canvas内の objName オブジェクトを探す
        GameObject obj = GameObject.Find(avatarName);

        // オブジェクトが見つかった場合
        if (obj != null)
        {
            // Imageコンポーネントを取得
            Image objImage = obj.GetComponent<Image>();

            if (objImage != null)
            {
                // ImageのColorプロパティに色を設定
                objImage.color = assignedColor;
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


}
