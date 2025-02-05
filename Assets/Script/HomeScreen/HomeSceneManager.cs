using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement; // Scene の切り替えしたい場合に必要な宣言

public class HomeSceneManager : MonoBehaviour
{
    /* public な変数 (Inspector でアタッチが必要) */
    // シーン内の Dropdown をアタッチ (Visual Studio の ComboBox みたいなもの)
    public Dropdown _dropdownSongTitle;
    public Dropdown _dropdownPlayerCount;
    
    void Start()
    {
        // Set select song field 
        SetDropdown();
        //Dropdown _dropdownSongTitle = GameObject.Find("Dropdown-SelectSong").GetComponent<Dropdown>();

        // ボタンが押されたらこれを実行
        GameObject.Find("ButtonNext").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    private void SetDropdown()
    {
        // ファイルパスを取得
        string filePath = Path.Combine(Application.dataPath, FileName.SongTitleList);

        // ファイルを読み込み、Dropdownに追加
        if (File.Exists(filePath))
        {
            string[] songList = File.ReadAllLines(filePath); // ファイル内容を行単位で読み込む
            SetDropdownSongTitles(songList); // Dropdownに追加
            SetDropdownPlayerCount();

            Debug.Log($"Loaded {songList.Length} songs from {filePath}");
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }
    }

    void ButtonClicked()
    {
        // Game する歌の名前と人数を保存
        SaveGameInfo();

        // 選択された歌が Birthday song なら準備できてるのでゲーム画面へ GO
        // それ以外の歌なら準備中・・・画面へ go
        SwitchScene();
    }

    void SwitchScene()
    {
        string songTitle = GetSongTitle();
        if (songTitle == "Birthday Song")
        {
            // "Birthday Song" っであれば
            // Mic-Color 対応をユーザに見せるシーン "Assignment" を開く
            SceneManager.LoadScene("Assignment");
        }
        else
        {
            // この歌はまだ準備中だよと知らせるシーン "ComingSoon" を開く
            SceneManager.LoadScene("ComingSoon");
        }
    }

    string GetSongTitle()
    {
        // 現在選択されているアイテムのインデックス
        int selectedItemIndex = _dropdownSongTitle.value;

        // 現在選択されているアイテムのテキスト
        string songTitle = _dropdownSongTitle.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {songTitle} (Index: {selectedItemIndex})");

        return songTitle;
    }

    string GetPlayerNum()
    {
        // 現在選択されているアイテムのインデックス
        int selectedItemIndex = _dropdownPlayerCount.value;
        // 現在選択されているアイテムのテキスト
        string playerNum = _dropdownPlayerCount.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {playerNum} (Index: {selectedItemIndex})");

        return playerNum;
    }

    void SetDropdownSongTitles(string[] titles)
    {
        // Dropdownの既存の項目をクリア
        _dropdownSongTitle.options.Clear();

        // 新しい項目を追加
        foreach (string title in titles)
        {
            _dropdownSongTitle.options.Add(new Dropdown.OptionData(title));
        }

        // 初期値を最初の項目に設定
        _dropdownSongTitle.value = 0;

        // Dropdownを更新
        _dropdownSongTitle.RefreshShownValue();
    }
    void SetDropdownPlayerCount()
    {
        // Dropdownの既存の項目をクリア
        _dropdownPlayerCount.options.Clear();
        List<int> numList = new List<int> { 1, 2, 3 };
        
        // Dropdown にアイテムを追加
        foreach (int num in numList)
        {
            string numStr = num.ToString();
            _dropdownPlayerCount.options.Add(new Dropdown.OptionData(numStr));
        }

        // 初期値を最初の項目に設定
        _dropdownPlayerCount.value = 0;

        // Dropdownを更新
        _dropdownPlayerCount.RefreshShownValue();
    }

    void SaveGameInfo()
    {
        // 記録ファイルのパスを取得
        string filePath = Path.Combine(Application.dataPath, FileName.MetaData);

        // Get game data registered
        string songTitle = GetSongTitle();
        string playerCountStr = GetPlayerNum();
        // Create playerList (default: Player1, Player2, ...)
        List<string> playerList = SetPlayerList(Common.ToInt(playerCountStr));

        // write into file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 1 行目に歌の名前を記録
            writer.WriteLine(songTitle);

            // 2 行目に参加人数を記録
            writer.WriteLine(playerCountStr);

            // 3 行目に players' name を Player1, Player2, Player3 の形式で記録
            string content = string.Join(", ", playerList);
            writer.WriteLine(content);

            // for debug
            Debug.Log($"Song title and player number saved:\n{songTitle}\n{playerCountStr}\n{content}");
        }
    }

    /// <summary>
    /// Set List (Default: player1, player2 ...)
    /// </summary>
    private List<string> SetPlayerList(int playerCount)
    {
        List<string> playerList = new List<string>();
        int playerNo = 1; // set from Player1

        // create player name (default)
        for (int i = 0; i < playerCount; i++, playerNo++)
        {
            string playerName = "Player" + playerNo.ToString();
            playerList.Add(playerName);
        }
        return playerList;
    }


    // Update is called once per frame
    void Update()
    {
    }
}
