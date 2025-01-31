using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongSelector : MonoBehaviour
{
    // 
    public GameObject _content; // ScrollView("SelectSong") �� Content
    public GameObject _buttonPrefab; // ���I�ɐ�������{�^���̃v���n�u
    public string _songListFileName = "SongTitleList.txt"; // �̃^�C�g�����X�g�̃t�@�C����
    private string _outputFilePath = "SongToPlay.txt"; // �L�^�t�@�C���̃p�X
    private List<string> _songList = new List<string>(); // �̖̂��O�̃��X�g
    //public float _itemSpacing = 20f; // �I��p�̖��{�^��(item)�Ԃ̊Ԋu�i�c�����j

    void Start()
    {
        // ���X�g�t�@�C���̃p�X��ݒ�
        string listFilePath = Path.Combine(Application.dataPath, _songListFileName);

        // ���X�g��ǂݍ���
        if (File.Exists(listFilePath))
        {
            _songList.AddRange(File.ReadAllLines(listFilePath));
            Debug.Log($"Loaded {_songList.Count} songs from {listFilePath}");
        }
        else
        {
            Debug.LogError($"Song list file not found: {listFilePath}");
            return;
        }

        // �L�^�t�@�C���̃p�X��ݒ�
        _outputFilePath = Path.Combine(Application.dataPath, "SongToPlay.txt");

        // �e�̖̂��O�ɑΉ�����{�^���𐶐�
        for (int i = 0; i < _songList.Count; i++)
        {
            CreateButton(_songList[i], i+1); // i+1 �ɂ���̂͑I�������X�N���[���{�b�N�X��̐^�񒆂ɕ\�������悤��
        }
        
        // Content�̃T�C�Y�𒲐�
        AdjustContentSize();

    }

    void CreateButton(string songName, int index)
    {
        // �{�^���� Content �̎q�I�u�W�F�N�g�Ƃ��Đ���
        GameObject button = Instantiate(_buttonPrefab, _content.transform);

        // �{�^���̃e�L�X�g��ݒ�
        button.GetComponentInChildren<Text>().text = songName;

        // �{�^���̈ʒu�𒲐�
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        // x �����J�n�ʒu 50 �ɂ���̂͑I�������X�N���[���{�b�N�X��̐^�񒆂ɕ\�������悤��
        //buttonRect.anchoredPosition = new Vector2(0, -index * _itemSpacing); // �c�����Ɉ��Ԋu��ݒ�

        // �{�^�����N���b�N���ꂽ�Ƃ��ɋL�^����C�x���g��ݒ�
        button.GetComponent<Button>().onClick.AddListener(() => SaveSongTitle(songName));
    }

    void SaveSongTitle(string songName)
    {
        // �t�@�C���ɉ̖̂��O���L�^
        File.WriteAllText(_outputFilePath, songName);

        // SongTitle �� TextBox"" �ɕ\��
        Text TextSelectedSongTitle = GameObject.Find("DisplaySongTitle").GetComponent<Text>();
        TextSelectedSongTitle.text = songName;
        Debug.Log($"Song title saved: {songName}");
    }

    void AdjustContentSize()
    {
        // Content�̍����𒲐�
        RectTransform contentRect = _content.GetComponent<RectTransform>();
        // Content����Grid Layout Group�R���|�[�l���g���擾
        GridLayoutGroup gridLayoutGroup = _content.GetComponent<GridLayoutGroup>();

        if (gridLayoutGroup != null)
        {
            // Cell Size �� Y (����) ���擾
            float cellHeight = gridLayoutGroup.cellSize.y;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, _songList.Count * cellHeight);

            //Debug.Log($"Cell Height: {cellHeight}");
        }
        else
        {
            Debug.LogError("Grid Layout Group component not found on the Content object.");
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
