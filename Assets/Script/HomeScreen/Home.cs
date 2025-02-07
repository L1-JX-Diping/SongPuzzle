using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾

public class Home : MonoBehaviour
{
    /* public �ȕϐ� (Inspector �ŃA�^�b�`���K�v) */
    // �V�[������ Dropdown ���A�^�b�` (Visual Studio �� ComboBox �݂����Ȃ���)
    public Dropdown _dropdownSongTitle;
    public Dropdown _dropdownPlayerCount;
    
    void Start()
    {
        // Set select song field 
        SetDropdown();

        // Register
        // At the start of the game, if a player has not registered, they can select 'Play as Anonymous'.

        //Dropdown _dropdownSongTitle = GameObject.Find("Dropdown-SelectSong").GetComponent<Dropdown>();

        // �{�^���������ꂽ�炱������s
        GameObject.Find("ButtonNext").GetComponent<Button>().onClick.AddListener(ButtonClicked);
        GameObject.Find("ViewSample").GetComponent<Button>().onClick.AddListener(ShowDivision);
    }

    private void SetDropdown()
    {
        // �t�@�C���p�X���擾
        string filePath = Path.Combine(Application.dataPath, FileName.SongTitleList);

        // �t�@�C����ǂݍ��݁ADropdown�ɒǉ�
        if (File.Exists(filePath))
        {
            string[] songList = File.ReadAllLines(filePath); // �t�@�C�����e���s�P�ʂœǂݍ���
            SetDropdownSongTitles(songList); // Dropdown�ɒǉ�
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
        // Play ����̖̂��O�Ɛl����ۑ�
        SaveDataToXML();

        // �I�����ꂽ�̂� Birthday song �Ȃ珀���ł��Ă�̂ŃQ�[����ʂ� GO
        // ����ȊO�̉̂Ȃ珀�����E�E�E��ʂ� go
        SwitchScene();
    }

    private void ShowDivision()
    {
        // Play ����̖̂��O�Ɛl����ۑ�
        SaveDataToXML();

        SceneManager.LoadScene("ShowDivision");
    }

    private void SwitchScene()
    {
        string songTitle = GetSongTitle();
        if (songTitle == "Birthday Song")
        {
            // "Birthday Song" ���ł����
            // Mic-Color �Ή������[�U�Ɍ�����V�[�� "Assignment" ���J��
            SceneManager.LoadScene("Assignment");
        }
        else
        {
            // ���̉̂͂܂�����������ƒm�点��V�[�� "ComingSoon" ���J��
            SceneManager.LoadScene("ComingSoon");
        }
    }

    string GetSongTitle()
    {
        // ���ݑI������Ă���A�C�e���̃C���f�b�N�X
        int selectedItemIndex = _dropdownSongTitle.value;

        // ���ݑI������Ă���A�C�e���̃e�L�X�g
        string songTitle = _dropdownSongTitle.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {songTitle} (Index: {selectedItemIndex})");

        return songTitle;
    }

    string GetPlayerCount()
    {
        // ���ݑI������Ă���A�C�e���̃C���f�b�N�X
        int selectedItemIndex = _dropdownPlayerCount.value;
        // ���ݑI������Ă���A�C�e���̃e�L�X�g
        string count = _dropdownPlayerCount.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {count} (Index: {selectedItemIndex})");

        return count;
    }

    void SetDropdownSongTitles(string[] titles)
    {
        // Dropdown�̊����̍��ڂ��N���A
        _dropdownSongTitle.options.Clear();

        // �V�������ڂ�ǉ�
        foreach (string title in titles)
        {
            _dropdownSongTitle.options.Add(new Dropdown.OptionData(title));
        }

        // �����l���ŏ��̍��ڂɐݒ�
        _dropdownSongTitle.value = 0;

        // Dropdown���X�V
        _dropdownSongTitle.RefreshShownValue();
    }
    void SetDropdownPlayerCount()
    {
        // Dropdown�̊����̍��ڂ��N���A
        _dropdownPlayerCount.options.Clear();
        List<int> numList = new List<int> { 1, 2, 3 };
        
        // Dropdown �ɃA�C�e����ǉ�
        foreach (int num in numList)
        {
            string numStr = num.ToString();
            _dropdownPlayerCount.options.Add(new Dropdown.OptionData(numStr));
        }

        // �����l���ŏ��̍��ڂɐݒ�
        _dropdownPlayerCount.value = 0;

        // Dropdown���X�V
        _dropdownPlayerCount.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
