using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DisplayLyrics : MonoBehaviour
{
    // 注意: _textField もし名前変更するなら Inspector のところで TextMeshPro みたいなの Row1,2,3 入れなおして
    public TextMeshProUGUI[] _textField; // 歌詞を表示するTextMeshProUGUIオブジェクト（3行分）
    private int _currentLyricIndex = 0; // 現在の歌詞インデックス
    private float _loadingTime = 0.8f; // time lag
    private float _lineStartTime = 0f; // intro 前奏終了時刻 = 歌詞表示(のスクロール用時刻計算)開始時刻
    // Load _lyricsList 
    private List<Line> _lyricsList = new List<Line>(); // 歌詞情報(表示開始時刻＋表示する歌詞)を格納するリスト
    // Load _playerList 
    private List<Player> _playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        // get division data
        LoadAndSetData();

        InitAvatarColor(); // 画面上にアバターの色を反映する

        UpdateLyricsDisplay(); // 初期表示を更新
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadAndSetData()
    {
        _lyricsList = (List<Line>)Common.LoadXml(_lyricsList.GetType(), FileName.XmlLyricsDivision);
        _playerList = (List<Player>)Common.LoadXml(_playerList.GetType(), FileName.XmlPlayerRole);
    }

    // Update is called once per frame
    void Update()
    {
        // 現在の時刻に基づいて歌詞を更新
        float currentTime = Time.timeSinceLevelLoad;

        /* Display lyrics */
        int maxIndex = _lyricsList.Count - 1;
        Line nextLine = _lyricsList[_currentLyricIndex + 1];
        // 次の歌詞行に進むべきタイミングか確認
        if (_currentLyricIndex < maxIndex && currentTime >= nextLine.timing - _loadingTime)
        {
            _currentLyricIndex++;
            UpdateLyricsDisplay();
        }

        /* Update Avatar Color */
        // Avatar のパートのときに光るなど変化つける
    }

    /// <summary>
    /// Update screen: set color to avatar when this scene load
    /// </summary>
    private void InitAvatarColor()
    {
        foreach (Player player in _playerList)
        {
            Color color = player.color; // like Color.green, Color.yellow...
            string avatar = player.avatar; // like "Heart", "Spade"...
            Debug.Log($"from _markDict: {Common.ToColorName(color)}, {avatar}");
            ReflectOnScreen(avatar, color); 
        }
        // did not be used
        // Club 
        ReflectOnScreen("Club", Color.gray);
    }

    private void UpdateLyricsDisplay()
    {
        // 真ん中の行を更新するためのインデックス
        int middleLineIndex = 1;

        for (int i = 0; i < _textField.Length; i++)
        {
            // 表示する歌詞行を決定（前後1行 + 現在行）
            int lyricIndex = _currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < _lyricsList.Count)
            {
                // テキストを色付きで構築
                string coloredText = "";
                foreach (Part part in _lyricsList[lyricIndex].partList)
                {
                    Player player = part.player;
                    string hexColor = ColorUtility.ToHtmlStringRGB(player.color);
                    coloredText += $"<color=#{hexColor}>{player.avatar}{part.word}</color> ";
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
