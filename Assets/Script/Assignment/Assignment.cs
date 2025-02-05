using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement;
using TMPro; // Scene の切り替えしたい場合に必要な宣言

public class Assignment : MonoBehaviour
{
    private List<string> _micList = new List<string>(); // 検出されたマイク名
    private List<Player> _playerRoleList = new List<Player>(); // 割り当てたマイクと色の情報
    private Dictionary<string, string> _avatarColorDict = new Dictionary<string, string>();
    // 候補となるアバター(今は順番に割り当て)
    // random 割り当てしなきゃ
    private List<string> _avatarList = new List<string> { "Heart", "Spade", "Diamond", "Club" }; // Heart, Spade, Diamond, Club の順で固定
    // to access game data
    private string _songTitle = "";
    private int _playerCount = 0;
    private List<string> _playerList = new List<string>();

    // Home シーンで x = 2 人で遊ぶを選んだら GameInfo.txt にそれが出力される
    // ここでは GameInfo.txt 読み込んで x 個のマイクを検出し、色を割り当てる

    [System.Serializable]
    public class Player
    {
        public string name;
        public Color color; // 割り当てた色
        public string micDevice; // マイク名
        public string avatar; // mark 
    }

    void Start()
    {
        // Get and Set game data registered in previous page from file
        SetGameData();

        // color assignment 
        AssignRoleToPlayers();

        // 画面にマイクと色の対応を表示する
        WriteIntoScreen();

        // パートの色とマイクの対応を保存
        SavePlayerRoleToFile();

        // Avatar(mark) と Color(string) の対応を保存
        //SaveAvatarDictToFile();

        // ボタンが押されたらこれを実行 Switch Scene
        //GameObject.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    /// <summary>
    /// Set game data to use in this class
    /// </summary>
    private void SetGameData()
    {
        // set data to GameData class
        GameData.SetAllData();

        // get from the data set to GameData class
        _songTitle = GameData.SongTitle;
        _playerCount = GameData.PlayerCount;
        _playerList = GameData.PlayerList;

        // for debug
        Debug.Log($"### SongTitle, PlayerCount get from GameData.cs:\n {_songTitle}, {_playerCount} \n");
        Debug.Log($"### PlayerList created: \n");
        foreach (string playerName in _playerList) Debug.Log($"{playerName} "); 
    }

    /// <summary>
    /// If button clicked, switch scene
    /// </summary>
    void ButtonClicked()
    {
        // ゲーム画面へ GO
        SwitchScene();
    }

    void SwitchScene()
    {
        // Gameシーン "DisplayLyrics" を開く
        SceneManager.LoadScene("DisplayLyrics");
    }

    private void WriteIntoScreen()
    {
        /* display mic(assignment) information */
        for (int index = 0; index < _playerCount; index++)
        {
            DisplayAssignment(index);
        }
    }

    /// <summary>
    /// Display to screen (index: players' index)
    /// </summary>
    /// <param name="index"></param>
    private void DisplayAssignment(int index)
    {
        string objName = "";
        string avatarName = "";
        Text micColorField;
        Text micNameField;

        // get current player name
        string playerName = _playerList[index]; // Player1, Player2... or Robot
        int playerNo = index + 1; // playerNo: from 1, index: from 0

        // Set text field to display assignment information
        objName = "Color" + playerNo.ToString();
        avatarName = "Avatar" + playerNo.ToString();
        micColorField = GameObject.Find(objName).GetComponent<Text>();

        // Set text field to display assignment information
        objName = "MicName" + playerNo.ToString();
        micNameField = GameObject.Find(objName).GetComponent<Text>();

        if (micColorField != null && micNameField != null)
        {
            // Get and Set assignment information for this player
            Player player = _playerRoleList[index];
            Color color = player.color;
            string colorName = Common.ToColorName(color);

            // Display mic DEVICE name
            micNameField.text = playerName + ": \n [mic]: " + player.micDevice;

            // Display assigned COLOR name
            micColorField.text = colorName; // display text (color name)
            micColorField.color = color; // Change text color

            // Change AVATAR color displayed on the screen
            ReflectToAvatar(avatarName, color);
            if (!_avatarColorDict.ContainsKey(colorName)) _avatarColorDict[colorName] = _avatarList[index];
        }
        else
        {
            Debug.LogError($"Textbox {micColorField} or {micNameField} is not assigned.");
        }
    }

    /// <summary>
    /// Set Players' role (MIC to use, AVATAR, COLOR displayed on the game screen)
    /// </summary>
    private void AssignRoleToPlayers()
    {
        // detect mic devices connected to PC and set them to _micList 
        SetMicList();

        /* Random Color Assignment */
        // 候補となる色の中から mic(player) の数だけ色を選ぶ
        List<Color> colors = RandomSelectColor(Common.AvailableColors, _playerCount);

        // For debug
        Debug.Log($"Selected colors:\n");
        foreach (Color color in colors)
        {
            Debug.Log($"{Common.ToColorName(color)}");
        }

        // Set player role
        for (int i = 0; i < _playerCount; i++)
        {
            // 構造体に追加
            _playerRoleList.Add(new Player
            {
                name = _playerList[i],
                micDevice = _micList[i],
                color = colors[i],
                avatar = _avatarList[i]
            });

            // for debug
            Debug.Log($"Assigned color {colors[i]} to {_micList[i]}");
        }
    }

    /// <summary>
    /// Set _micList by detecting mic devices
    /// </summary>
    private void SetMicList()
    {
        // the CASE that NO mic detected (without default mic of PC)
        if (Microphone.devices.Length == 1)
        {
            // おそらくパソコン本体のマイク
            Debug.Log($"No USB mic detected.\n{Microphone.devices[0]} is detected.");

            for (int i = 0; i < _playerCount; i++)
            {
                string micName = "Mic" + (i + 1).ToString() + "(Just singing)";
                _micList.Add(micName);
            }

            // for debug
            Debug.Log($"No microphones detected.\n{_micList.Count} content of _micList :\n");
            foreach (string name in _micList) Debug.Log($"{name}");

            return;
        }

        // the CASE that mic detected
        IfMicDetected();
    }

    /// <summary>
    /// If there are more than one of the mics connected to APP
    /// </summary>
    private void IfMicDetected()
    {
        // Get mic devices that connected to PC with USB
        foreach (string deviceName in Microphone.devices)
        {
            // PC 本体のマイクは含めない 
            if (deviceName == "マイク配列 (Realtek(R) Audio)") continue;

            // USB で接続されたマイクのみ追加
            _micList.Add(deviceName);
        }
        Debug.Log($"Detected {_micList.Count} microphones.");

        // If (the number of MIC devices < the number of PLAYERs registered in the previous page)
        // Set "Robot part" for the number of the distinction between these two
        // e.g. playerCount = 3, 2 mic detected --> There is one robot part exist
        while (_micList.Count < _playerCount)
        {
            string micName = "Mic" + (_micList.Count + 1).ToString() + "(Robot part)";
            _micList.Add(micName);
        }
        Debug.Log($"There are {_playerCount} players and {_micList.Count} mics are assigned to each player.");
    }


    /// <summary>
    /// Save information to file
    /// </summary>
    private void SavePlayerRoleToFile()
    {
        string filePath = Path.Combine(Application.dataPath, FileName.PlayerRole);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Player player in _playerRoleList)
            {
                // Player1, Red, Heart, Mic1 
                writer.WriteLine($"{player.name}, {Common.ToColorName(player.color)}, {player.avatar}, {player.micDevice}");
            }
        }

        Debug.Log($"Color information saved to {filePath}");
    }

    /// <summary>
    /// Set Color to Avatar
    /// </summary>
    /// <param name="avatarName"></param>
    /// <param name="color"></param>
    void ReflectToAvatar(string avatarName, Color color)
    {
        // avatarName = "Avatar1" 
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
                avatarImage.color = color;
                Debug.Log($"Color {color} applied to {avatarName}.");
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

    /// <summary>
    /// Selecting X colors from the input List of colors (x = count, the 2nd parameter)
    /// </summary>
    /// <param name="colorsForSelect"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private List<Color> RandomSelectColor(List<Color> availableColors, int count)
    {
        // to collect Selected Colors 結果を格納するリスト
        List<Color> result = new List<Color>();

        // 選択する色数 count が候補色の要素数 availableColors.Count を超えないようにする
        count = Mathf.Min(count, availableColors.Count);

        // Select X colors (X = count)
        for (int i = 0; i < count; i++)
        {
            // while 文使う場合の終了条件
            //if (selectedColors.Count == count) break; // count 個選択できたら任務完了

            // ランダムに 1 つ選択
            int index = Random.Range(0, availableColors.Count);
            result.Add(availableColors[index]);

            // 選択済みの色を候補色リストから削除（重複を避ける）
            availableColors.RemoveAt(index);
        }

        return result;
    }


    /// <summary>
    /// Robot part
    /// </summary>
    private void DisplayRobotPartAssignment()
    {
        // 名前で UI オブジェクトを探す
        Text mic3ColorField = GameObject.Find("Color3").GetComponent<Text>();
        Text mic3NameField = GameObject.Find("MicName3").GetComponent<Text>();

        if (mic3ColorField != null)
        {
            Color targetColor = Color.white;

            // _colorNameListから未使用の色を検索
            foreach (Color color in Common.AvailableColors)
            {
                if (color != _playerRoleList[0].color && color != _playerRoleList[1].color)
                {
                    targetColor = color; // 未使用の色を設定
                    break; // 最初に見つけた未使用の色でループを終了
                }
            }
            string colorName = Common.ToColorName(targetColor);
            // text 
            mic3ColorField.text = colorName; // パート色表示
            // text color
            mic3ColorField.color = targetColor;
            // avatar name (mark)
            ReflectToAvatar("Avatar3", targetColor);
            //_markColorDict
            if (!_avatarColorDict.ContainsKey(colorName)) _avatarColorDict[colorName] = _avatarList[2];
        }
        else
        {
            Debug.LogError($"Text field {mic3ColorField} is not assigned.");
        }
        if (mic3NameField != null)
        {
            mic3NameField.text = "Robot Part"; // 機械の担当パート, 満点扱い
        }
        else
        {
            Debug.LogError($"Text field {mic3NameField} is not assigned.");
        }
    }

    ///// <summary>
    ///// 要らなくなるかも
    ///// </summary>
    //private void SaveAvatarDictToFile()
    //{
    //    string filePath = Path.Combine(Application.dataPath, FileName.AvatarColorPairing);

    //    using (StreamWriter writer = new StreamWriter(filePath))
    //    {
    //        foreach (var avatar in _avatarColorDict)
    //        {
    //            writer.WriteLine($"{avatar.Key}, {avatar.Value}");
    //        }
    //    }

    //    Debug.Log($"Color information saved to {filePath}");
    //}

}
