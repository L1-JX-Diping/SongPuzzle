using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement; // Scene の切り替えしたい場合に必要な宣言

public class Home : MonoBehaviour
{
    /* public な変数 (Inspector でアタッチが必要) */
    // シーン内の Dropdown をアタッチ (Visual Studio の ComboBox みたいなもの)
    public Dropdown _dropdownSongTitle;
    public Dropdown _dropdownPlayerCount;
    
    void Start()
    {
        // Set select song field 
        SetDropdown();

        // Register
        // At the start of the game, if a player has not registered, they can select 'Play as Anonymous'.

        //Dropdown _dropdownSongTitle = GameObject.Find("Dropdown-SelectSong").GetComponent<Dropdown>();

        // ボタンが押されたらこれを実行
        GameObject.Find("ButtonNext").GetComponent<Button>().onClick.AddListener(ButtonClicked);
        GameObject.Find("ViewSample").GetComponent<Button>().onClick.AddListener(ShowDivision);
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
    
    private void SaveDataToXML()
    {
        Song song = new Song();
        Team team = new Team();

        /* Get game data registered */
        song.Title = GetSongTitle();
        string playerCountStr = GetPlayerCount();
        int count = Common.ToInt(playerCountStr);
        // Create playerList 
        // ***Update*** Register players
        List<Player> playerList = SetPlayerListForTeam0(count);

        // default: 'Play as Anonymous'
        team.ID = 0;
        team.CountMembers = count;
        team.MemberList = playerList;

        /* Set to gameData list */
        Data data = new Data();
        data.Song = song;
        data.Team = team;

        // 
        Common.ExportToXml(data, FileName.XmlGameData); // create data (registered at Home scene)
    }

    /// <summary>
    /// Set List (Default: player1, player2 ...)
    /// </summary>
    private List<Player> SetPlayerListForTeam0(int playerCount)
    {
        List<Player> playerList = new List<Player>();
        int playerNo = 1; // set from Player1

        // create player name (default)
        for (int i = 0; i < playerCount; i++, playerNo++)
        {
            string playerName = "Player" + playerNo.ToString();
            Player player = new Player();
            // Only add player name in this class
            player.Name = playerName;
            playerList.Add(player);
        }
        return playerList;
    }

    /// <summary>
    /// if Button Clicked: save data and switch scene
    /// </summary>
    private void ButtonClicked()
    {
        // Play する歌の名前と人数を保存
        SaveDataToXML();

        // 選択された歌が Birthday song なら準備できてるのでゲーム画面へ GO
        // それ以外の歌なら準備中・・・画面へ go
        SwitchScene();
    }

    private void ShowDivision()
    {
        // Play する歌の名前と人数を保存
        SaveDataToXML();

        SceneManager.LoadScene("ShowDivision");
    }

    private void SwitchScene()
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

    string GetPlayerCount()
    {
        // 現在選択されているアイテムのインデックス
        int selectedItemIndex = _dropdownPlayerCount.value;
        // 現在選択されているアイテムのテキスト
        string count = _dropdownPlayerCount.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {count} (Index: {selectedItemIndex})");

        return count;
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

    // Update is called once per frame
    void Update()
    {
    }
}
