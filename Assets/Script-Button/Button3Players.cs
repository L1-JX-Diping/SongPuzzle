using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾
using System.IO; // �t�@�C�������̂� (Path, StreamWritter ���g������)

public class Button3Player : MonoBehaviour
{
    private string _outputFileName = "GameInfo.txt";
    private int _playerCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        // �{�^���������ꂽ�炱������s
        this.GetComponent<Button>().onClick.AddListener(SwitchScene);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwitchScene()
    {
        AddSaveGameInfo();
        // �V�[�� ComingSoon ���J��
        SceneManager.LoadScene("ComingSoon");
    }

    void AddSaveGameInfo()
    {
        // �L�^�t�@�C���̃p�X���擾
        string filePath = Path.Combine(Application.dataPath, _outputFileName);

        // �t�@�C���S�̂�ǂݍ���
        string[] lineList;
        if (File.Exists(filePath))
        {
            lineList = File.ReadAllLines(filePath);
        }
        else
        {
            // �t�@�C�������݂��Ȃ��ꍇ�A��̔z����쐬
            lineList = new string[0];
        }

        // 2�s�ڂ�ݒ�
        if (lineList.Length >= 2)
        {
            // 2�s�ڂ����݂���ꍇ�͏㏑��
            lineList[1] = _playerCount.ToString();
        }
        else
        {
            // 2�s�ڂ����݂��Ȃ��ꍇ�͐V���ɒǉ�
            List<string> linesList = new List<string>(lineList);
            while (linesList.Count < 2)
            {
                linesList.Add(""); // ��s��ǉ�
            }
            linesList[1] = _playerCount.ToString();
            lineList = linesList.ToArray();
        }

        // �t�@�C���ɏ����߂�
        File.WriteAllLines(filePath, lineList);

        Debug.Log($"Player count ({_playerCount}) successfully saved to {filePath}");
    }
}
