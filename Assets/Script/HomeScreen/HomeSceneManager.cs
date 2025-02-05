using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾

public class HomeSceneManager : MonoBehaviour
{
    /* public �ȕϐ� (Inspector �ŃA�^�b�`���K�v) */
    // �V�[������ Dropdown ���A�^�b�` (Visual Studio �� ComboBox �݂����Ȃ���)
    public Dropdown _dropdownSongTitle;
    public Dropdown _dropdownPlayerCount;
    
    void Start()
    {
        // Set select song field 
        SetDropdown();
        //Dropdown _dropdownSongTitle = GameObject.Find("Dropdown-SelectSong").GetComponent<Dropdown>();

        // �{�^���������ꂽ�炱������s
        GameObject.Find("ButtonNext").GetComponent<Button>().onClick.AddListener(ButtonClicked);
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

    void ButtonClicked()
    {
        // Game ����̖̂��O�Ɛl����ۑ�
        SaveGameInfo();

        // �I�����ꂽ�̂� Birthday song �Ȃ珀���ł��Ă�̂ŃQ�[����ʂ� GO
        // ����ȊO�̉̂Ȃ珀�����E�E�E��ʂ� go
        SwitchScene();
    }

    void SwitchScene()
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

    string GetPlayerNum()
    {
        // ���ݑI������Ă���A�C�e���̃C���f�b�N�X
        int selectedItemIndex = _dropdownPlayerCount.value;
        // ���ݑI������Ă���A�C�e���̃e�L�X�g
        string playerNum = _dropdownPlayerCount.options[selectedItemIndex].text;

        Debug.Log($"Currently Selected Item: {playerNum} (Index: {selectedItemIndex})");

        return playerNum;
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

    void SaveGameInfo()
    {
        // �L�^�t�@�C���̃p�X���擾
        string filePath = Path.Combine(Application.dataPath, FileName.MetaData);

        // Get game data registered
        string songTitle = GetSongTitle();
        string playerCountStr = GetPlayerNum();
        // Create playerList (default: Player1, Player2, ...)
        List<string> playerList = SetPlayerList(Common.ToInt(playerCountStr));

        // write into file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 1 �s�ڂɉ̖̂��O���L�^
            writer.WriteLine(songTitle);

            // 2 �s�ڂɎQ���l�����L�^
            writer.WriteLine(playerCountStr);

            // 3 �s�ڂ� players' name �� Player1, Player2, Player3 �̌`���ŋL�^
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
