using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CalculateScore : MonoBehaviour
{
    // MicColorInfo.txt ��ǂݍ���ŐF�̃��X�g�쐬�E�X�R�A�v�Z����K�v
    //private string _micColorInfoFileName = "MicColorInfo.txt"; // �}�C�N�ƃp�[�g�F�̑Ή����L���ꂽ�t�@�C����
                                                               // MicColorInfo.txt ����}�C�N�ƃp�[�g�F�̑Ή��� Dictionary �Ɋi�[
                                                               // PartLog.txt ���玞���Ɛ����̃p�[�g�F�����擾
                                                               // MicDetectionLog.txt ���玞���Ǝg�p���ꂽ�}�C�N�̏��擾
                                                               // Dictionary �g���ă}�C�N�̕��� �p�[�g�F �ɍ����ւ��ĕϐ��Ɋi�[
                                                               // Volume�ł��̎����ǂ̃}�C�N(�F)������
                                                               // PartLog.txt, MicDetectionLog.txt �ƍ����邱�ƂŃX�R�A�v�Z
                                                               // ����: Dictionary �ɂȂ��F�́A�����I�ɐ����������ƂƂ��Čv�Z

    // MicColorInfo.txt ��ǂݍ���ŐF�̃��X�g�쐬�E�X�R�A�v�Z����K�v
    private string _micColorInfoFileName = "MicColorInfo.txt"; // �}�C�N�ƃp�[�g�F�̑Ή����L���ꂽ�t�@�C����
    // MicColorInfo.txt ����}�C�N�ƃp�[�g�F�̑Ή��� Dictionary �Ɋi�[
    private Dictionary<string, string> _micColorDict = new Dictionary<string, string>(); // key:mic, value:colorName
    private string _robotPartColor = "";
    // �g�p����F
    private List<string> _colorNameList = new List<string> 
    { 
        "GREEN", 
        "RED", 
        "YELLOW" 
    };
    // PartLog.txt ���玞���Ɛ����̃p�[�g�F�����擾
    private string _partLogFileName = "PartLog.txt"; // �����Ɗ��蓖�Ă�ꂽ�����̃p�[�g�F���L���ꂽ�t�@�C����
    List<TimeColorInfo> _timeColorInfoList = new List<TimeColorInfo>();

    private int _countBottom = 0; // Bottom ����
    private int _countCorrect = 0; // Top ���q

    // MicDetectionLog.txt ���玞���Ǝg�p���ꂽ�}�C�N�̏��擾
    private string _micDetectionLogFileName = "MicDetectionLog.txt";

    [System.Serializable]
    public class TimeColorInfo
    {
        public float time; // �\�������i�b�P�ʁj
        public string assignedColor; // �����̃p�[�g�F���蓖��
        public List<Detection> micVolumeList; // ���ۂɉ̂����l �ł�volume�̑傫��mic�𔻒�
    }

    [System.Serializable]
    public class Detection
    {
        public string micColor; // �̂����l�̎����F �}�C�N�����画��
        public float volume; // volume
    }

    // Start is called before the first frame update
    void Start()
    {
        // _micColorDict 
        SetMicColorDict();
        
        // 
        SetCorrectPartInfo();
        ProcessMicDetectionLog();

        // 
        SetRobotPart();
        CalcScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CalcScore()
    {
        _countBottom = 0; // ����i�S�̂̔��萔�j
        _countCorrect = 0; // ���q�i���𐔁j

        foreach (TimeColorInfo timeColorInfo in _timeColorInfoList)
        {
            // ����𑝉��i�����萔�j
            _countBottom++;

            float maxVolume = 0f;
            Detection micUsed = null;

            // micVolumeList����ő剹�ʂ̃}�C�N��������
            foreach (Detection micStatus in timeColorInfo.micVolumeList)
            {
                if (micStatus.volume > maxVolume)
                {
                    maxVolume = micStatus.volume;
                    micUsed = micStatus;
                }
            }

            // �ő剹�ʂ̃}�C�N�����݂���ꍇ
            if (micUsed != null)
            {
                string correctColor = timeColorInfo.assignedColor;
                if (correctColor == _robotPartColor)
                {
                    _countCorrect++; // ���𐔂𑝉�
                }
                // �����̃p�[�g�F�Ɣ�r���Ĕ���
                if (correctColor == micUsed.micColor)
                {
                    _countCorrect++; // ���𐔂𑝉�
                }
            }
        }

        // �X�R�A�v�Z (���ꂪ�[���̏ꍇ���l��)
        float scoreResult = _countBottom > 0 ? (float)_countCorrect / _countBottom : 0f;

        // Text �Ƃ��ĉ�ʏ�ɕ`��
        DisplayScore(scoreResult);

        // �X�R�A���f�o�b�O���O�ɏo��
        Debug.Log($"Score: {_countCorrect}/{_countBottom} ({scoreResult * 100:F2}%)");
    }

    // _robotPartColor �� _colorNameList �ɂ���O�F�̂����A_micColorDict �̂ǂ� value �ɂ���v���Ȃ��F��ݒ�
    void SetRobotPart()
    {
        // �g�p���̐F�����W
        HashSet<string> usedColors = new HashSet<string>(_micColorDict.Values);

        // _colorNameList���疢�g�p�̐F��������
        foreach (string color in _colorNameList)
        {
            if (!usedColors.Contains(color))
            {
                _robotPartColor = color;
                Debug.Log($"Robot part assigned color: {_robotPartColor}");
                return;
            }
        }

        // �S�F���g�p���̏ꍇ�̃f�t�H���g����
        Debug.LogWarning("All colors are used. No color assigned to the robot part.");
        _robotPartColor = "NONE"; // �f�t�H���g�l
    }

    void DisplayScore(float scoreResult)
    {
        //float scoreResult = _countBottom > 0 ? (float)_countCorrect / _countBottom : 0f;

        // �X�R�A���f�o�b�O���O�ɏo��
        Debug.Log($"Final Score: {_countCorrect}/{_countBottom} ({scoreResult * 100:F2}%)");

        // UI�v�f�ɕ\������ꍇ�iTextMeshProUGUI���g�p�j
        GameObject scoreTextObj = GameObject.Find("ScoreText"); // �V�[����ScoreText�I�u�W�F�N�g�����邱�Ƃ�z��
        if (scoreTextObj != null)
        {
            var scoreText = scoreTextObj.GetComponent<TMPro.TextMeshProUGUI>();
            if (scoreText != null)
            {
                scoreText.text = $"Score: {_countCorrect}/{_countBottom}\n({scoreResult * 100:F2}%)";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on ScoreText object.");
            }
        }
        else
        {
            Debug.LogWarning("ScoreText object not found in the scene.");
        }
    }


    void ProcessMicDetectionLog()
    {
        // �t�@�C���p�X���擾
        string filePath = Path.Combine(Application.dataPath, _micDetectionLogFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"MicDetectionLog file not found: {filePath}");
            return;
        }

        // �t�@�C����ǂݍ���
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // �s���J���}�ŕ���
            string[] parts = line.Split(',');

            if (parts.Length == 3)
            {
                // �������擾
                // time, micName, volume �̌`�� 
                // ��: 4.00, �}�C�N (Logi C615 HD WebCam), 0.0109
                if (float.TryParse(parts[0].Trim(), out float detectedTime))
                {
                    string micName = parts[1].Trim();
                    // Dict ���}�C�N������Ή�����F�֕ϊ�
                    string micColorName = _micColorDict[micName];
                    if (float.TryParse(parts[2].Trim(), out float volume))
                    {
                        // �Ή����� TimeColorInfo ��T���Ēǉ�
                        foreach (TimeColorInfo timeColorInfo in _timeColorInfoList)
                        {
                            // ��������v����ꍇ
                            if (Mathf.Approximately(timeColorInfo.time, detectedTime))
                            {
                                timeColorInfo.micVolumeList.Add(new Detection
                                {
                                    micColor = micColorName,
                                    volume = volume
                                });
                                Debug.Log($"Added Detection: Time={detectedTime}, Mic={micName}, Volume={volume}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid volume format in line: {line}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid time format in line: {line}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }

        Debug.Log("Processing of MicDetectionLog completed.");
    }

    void SetCorrectPartInfo()
    {
        // �t�@�C���p�X�𐶐�
        string filePath = Path.Combine(Application.dataPath, _partLogFileName);

        // �t�@�C�������݂��邩�m�F
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Part log file not found: {filePath}");
            return;
        }

        // �t�@�C����ǂݍ���
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // �s���J���}�ŕ���
            string[] parts = line.Split(',');

            if (parts.Length == 2) // �������`�����m�F
            {
                // �������p�[�X
                if (float.TryParse(parts[0].Trim(), out float time))
                {
                    string colorName = parts[1].Trim();

                    // ���X�g�ɒǉ�
                    _timeColorInfoList.Add(new TimeColorInfo
                    {
                        time = time,
                        assignedColor = colorName
                    });
                }
                else
                {
                    Debug.LogWarning($"Invalid time format in line: {line}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }

        Debug.Log($"Loaded {_timeColorInfoList.Count} entries from part log.");
    }

    void SetMicColorDict()
    {
        string filePath = Path.Combine(Application.dataPath, _micColorInfoFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"MicColorInfo file not found: {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 2)
            {
                // RED, micName �̌`��
                string colorName = parts[0].Trim();
                string micName = parts[1].Trim();

                _micColorDict[micName] = colorName;
                Debug.Log($"'{micName}' '{colorName}'");
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }
    }

}
