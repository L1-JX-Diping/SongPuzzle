using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾

public class HomeSceneManager : MonoBehaviour
{
    /* public �ȕϐ� (Inspector �ŃA�^�b�`���K�v) */
    // �V�[������Dropdown���A�^�b�` (Visual Studio �� ComboBox �݂����Ȃ���)
    public Dropdown _dropdownSongTitle;
    public Dropdown _dropdownPlayerCount;
    
    /* private �ȕϐ� */
    private string _songListFileName = "SongTitleList.txt"; // ���X�g�t�@�C���̖��O�iAssets�t�H���_���j
    private string _outputFileName = "GameInfo.txt"; // �L�^�t�@�C���̃p�X
    //private string _songTitle = "Birthday Song";

    void Start()
    {
        // Set select song field 
        SetDropdown();

        // �{�^���������ꂽ�炱������s
        GameObject.Find("ButtonNext").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    private void SetDropdown()
    {
        // �t�@�C���p�X���擾
        string filePath = Path.Combine(Application.dataPath, _songListFileName);

        // �t�@�C����ǂݍ��݁ADropdown�ɒǉ�
        if (File.Exists(filePath))
        {
            string[] songList = File.ReadAllLines(filePath); // �t�@�C�����e���s�P�ʂœǂݍ���
            SetDropdownSongTitles(songList); // Dropdown�ɒǉ�
            SetDropdownPlayerNum();

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
            // Mic-Color �Ή������[�U�Ɍ�����V�[�� "AssignColor" ���J��
            SceneManager.LoadScene("AssignColor");
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
    void SetDropdownPlayerNum()
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
        string filePath = Path.Combine(Application.dataPath, _outputFileName);
        string songTitle = GetSongTitle();
        string playerCount = GetPlayerNum();

        // �����s����x�ɏ�������
        string[] lines = { songTitle, playerCount }; // �z��Ɋe�s�̓��e���i�[
        // �t�@�C�� 1 �s�ڂɉ̖̂��O���L�^
        // �t�@�C�� 2 �s�ڂɎQ���l�����L�^
        File.WriteAllLines(filePath, lines); // �S�s���ꊇ�ŏ�������

        Debug.Log($"Song title and player number saved:\n{songTitle}\n{playerCount}");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
