using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement;
using TMPro; // Scene の切り替えしたい場合に必要な宣言

public class Assignment : MonoBehaviour
{
    private List<string> _microphoneNameList = new List<string>(); // 検出されたマイク名
    private List<PlayerAssignment> _assignmentInfoList = new List<PlayerAssignment>(); // 割り当てたマイクと色の情報
    private Dictionary<string, string> _avatarColorDict = new Dictionary<string, string>();
    private List<string> _colorNameList = new List<string> { "GREEN", "RED", "YELLOW" }; // 使用する 3 色
    private List<string> _avatarList = new List<string> { "Heart", "Spade", "Diamond" }; // Heart, Spade, Diamond, Club の順で固定

    //// インスペクターで手動で割り当てる場合に使用
    //public Text _textboxMic1Color;
    //public Text _textboxMic2Color;

    // Home シーンで x = 2 人で遊ぶを選んだら GameInfo.txt にそれが出力される
    // ここでは GameInfo.txt 読み込んで x 個のマイクを検出し、色を割り当てる

    [System.Serializable]
    public class PlayerAssignment
    {
        public string colorName; // 割り当てた色
        public string microphone; // マイク名
        public string mark; // mark 
    }

    void Start()
    {
        // color assignment 
        AssignColorsToMicrophones();

        // 画面にマイクと色の対応を表示する
        WriteIntoTextbox();

        // パートの色とマイクの対応を保存
        SavePlayerAssignmentToFile();
        // Avatar(mark) と Color(string) の対応を保存
        SaveAvatarDictToFile();

        // ボタンが押されたらこれを実行 Save file / Switch Scene
        //GameObject.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

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

    private void WriteIntoTextbox()
    {
        // Get game meta data
        string songTitle = GameData.GetSongTitle();
        int playerCount = GameData.GetPlayerCount();
        // for debug
        Debug.Log($"### PlayerCount get from GameData.cs: {playerCount}");

        /* mic(assignment) information for real players */
        for (int index = 0; index < playerCount; index++)
        {
            DisplayAssignment(index);
        }

        /* mic(assignment) information for robot */
        // 名前で UI オブジェクトを探す
        Text mic3ColorField = GameObject.Find("Color3").GetComponent<Text>();
        Text mic3NameField = GameObject.Find("MicName3").GetComponent<Text>();
        
        if (mic3ColorField != null)
        {
            string colorName = "";
            string assignedColor0 = _assignmentInfoList[0].colorName;
            string assignedColor1 = _assignmentInfoList[1].colorName;

            // _colorNameListから未使用の色を検索
            foreach (string name in _colorNameList)
            {
                if (name != assignedColor1 && name != assignedColor0)
                {
                    colorName = name; // 未使用の色を設定
                    break; // 最初に見つけた未使用の色でループを終了
                }
            }
            Color color = Common.NameToColor(colorName);
            mic3ColorField.text = colorName; // Color-Mic3 にパート色表示
            mic3ColorField.color = color;
            ReflectToAvatar("Avatar3", color);
            //_markColorDict
            if (!_avatarColorDict.ContainsKey(colorName)) _avatarColorDict[colorName] = _avatarList[2];
        }
        else
        {
            Debug.LogError("Textbox Color-Mic1 is not assigned.");
        }
        if (mic3NameField != null)
        {
            mic3NameField.text = "Robot Part"; // Name-Mic3 は機械が担当, 自動で満点換算
        }
        else
        {
            Debug.LogError("Textbox Name-Mic1 is not assigned.");
        }

    }

    private void DisplayAssignment(int index)
    {
        int playerNo = index + 1; // playerNo: from 1, index: from 0
        string playerName = "Robot"; // Player1, Player2... or Robot
        string objName = "";
        string avatarName = "";
        Text micColorField;
        Text micNameField;

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
            PlayerAssignment player = _assignmentInfoList[index];
            string colorName = player.colorName;
            Color color = Common.NameToColor(colorName);

            // Display mic DEVICE name
            micNameField.text = player.microphone;

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

    private void AssignColorsToMicrophones()
    {
        // マイクデバイスを取得
        foreach (string deviceName in Microphone.devices)
        {
            // パソコン本体のマイクは含めない
            // USB で接続されたマイクのみリストに追加したい
            if (deviceName == "マイク配列 (Realtek(R) Audio)")
            {
                continue;
            }

            _microphoneNameList.Add(deviceName);
        }

        if (_microphoneNameList.Count == 0)
        {
            //string deviceName = "No_Mic";
            //_microphoneNameList.Add(deviceName);

            // if (playerCount == 2) {}
            _microphoneNameList[0] = "Mic1(just singing)";
            _microphoneNameList[1] = "Mic2(just singing)";
            Debug.Log("No microphones detected.");
            return;
        }

        Debug.Log($"Detected {_microphoneNameList.Count} microphones.");

        // 色をランダムに割り当て

        // 候補となる数字 index of color
        List<int> numberList = new List<int> { 0, 1, 2 };

        // 結果を格納するリスト
        List<int> indexList = SelectTwoRandomIndexes(numberList);

        // 結果をログ出力
        Debug.Log($"Selected indexes: {indexList[0]}, {indexList[1]}");

        // 色を割り当て
        for (int i = 0; i < _microphoneNameList.Count; i++)
        {
            string assignedColor = _colorNameList[indexList[i]]; // 色を順番に割り当て

            // 構造体に追加
            _assignmentInfoList.Add(new PlayerAssignment
            {
                microphone = _microphoneNameList[i],
                colorName = assignedColor,
                mark = _avatarList[i]
            });

            Debug.Log($"Assigned color {assignedColor} to {_microphoneNameList[i]}");
        }
    }

    List<int> SelectTwoRandomIndexes(List<int> numberList)
    {
        List<int> result = new List<int>();

        // シャッフルして上位2つを取得
        for (int i = numberList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (numberList[i], numberList[randomIndex]) = (numberList[randomIndex], numberList[i]);
        }

        result.Add(numberList[0]);
        result.Add(numberList[1]);

        return result;
    }
    
    private void SaveAvatarDictToFile()
    {
        string filePath = Path.Combine(Application.dataPath, FileName.AvatarColorPairing);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var avatar in _avatarColorDict)
            {
                writer.WriteLine($"{avatar.Key}, {avatar.Value}");
            }
        }

        Debug.Log($"Color information saved to {filePath}");
    }

    private void SavePlayerAssignmentToFile()
    {
        string filePath = Path.Combine(Application.dataPath, FileName.PlayerAssignment);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (PlayerAssignment colorInfo in _assignmentInfoList)
            {
                writer.WriteLine($"{colorInfo.colorName}, {colorInfo.microphone}, {colorInfo.mark}");
                // RED, マイク (Logi C615 HD WebCam)
                // みたいな形式で保存される
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

}
