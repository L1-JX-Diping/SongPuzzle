using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾
using System.IO;

public class ButtonNext : MonoBehaviour
{
    // �{�^���ɒ��ڃA�^�b�`����o�[�W�����̃X�N���v�g
    // �ł���肭�����Ȃ�����
    // HomeSceneManager �œ����悤�Ȋ֐��g�����ꍇ���܂������� (Birthday Song �̂�)
    private string _gameInfoFileName = "GameInfo.txt"; // �t�@�C����
    
    // Start is called before the first frame update
    void Start()
    {
        // �{�^���������ꂽ�炱������s
        this.GetComponent<Button>().onClick.AddListener(SwitchScene);
    }

    /// <summary>
    /// Read file to get song title (string)
    /// </summary>
    /// <returns></returns>
    string GetSongTitle()
    {
        // �t�@�C���p�X�̐���
        string filePath = Path.Combine(Application.dataPath, _gameInfoFileName);

        // �t�@�C�������݂��邩�m�F
        if (!File.Exists(filePath))
        {
            Debug.LogError($"GameInfo file not found: {filePath}");
            return "NONE";
        }

        // �t�@�C�����s�P�ʂœǂݍ���
        string[] lines = File.ReadAllLines(filePath);
        string songTitle = "";

        // 1�s�ڂ��� songTitle ���擾
        if (lines.Length > 0)
        {
            songTitle = lines[0].Trim(); // 1�s�ڂ̋Ȗ����擾
        }
        else
        {
            Debug.LogError("GameInfo.txt is empty.");
        }
        return songTitle;
    }

    void SwitchScene()
    {
        // songTitle ���擾
        string songTitle = GetSongTitle();
        Debug.Log($"Song Title: {songTitle}");

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
