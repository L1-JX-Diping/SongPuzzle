using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement;
using TMPro; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾

public class AssignmentManager : MonoBehaviour
{
    private string _outputFileName = "MicColorInfo.txt"; // �o�̓t�@�C����
    private List<string> _microphoneNameList = new List<string>(); // ���o���ꂽ�}�C�N��
    private List<ColorInfo> _assignmentInfoList = new List<ColorInfo>(); // ���蓖�Ă��}�C�N�ƐF�̏��
    private Dictionary<string, string> _markColorDict = new Dictionary<string, string>();
    private List<string> _colorNameList = new List<string> { "GREEN", "RED", "YELLOW" }; // �g�p���� 3 �F
    private List<string> _markList = new List<string> { "Heart", "Spade", "Diamond" }; // Heart, Spade, Diamond, Club �̏��ŌŒ�

    // �C���X�y�N�^�[�Ŏ蓮�Ŋ��蓖�Ă�ꍇ�Ɏg�p
    public Text _textboxMic1Color;
    public Text _textboxMic2Color;

    // Home �V�[���� x = 2 �l�ŗV�Ԃ�I�񂾂� GameInfo.txt �ɂ��ꂪ�o�͂����
    // �����ł� GameInfo.txt �ǂݍ���� x �̃}�C�N�����o���A�F�����蓖�Ă�

    [System.Serializable]
    public class ColorInfo
    {
        public string colorName; // ���蓖�Ă��F
        public string microphone; // �}�C�N��
        public string mark; // mark 
    }

    void Start()
    {
        // color assignment 
        AssignColorsToMicrophones();

        // ��ʂɃ}�C�N�ƐF�̑Ή���\������
        WriteIntoTextbox();

        // �p�[�g�̐F�ƃ}�C�N�̑Ή���ۑ�
        SaveColorInfoToFile();
        // �}�[�N�ƐF�̑Ή���ۑ�
        SaveMarkDictToFile();

        // �{�^���������ꂽ�炱������s
        //GameObject.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    void ButtonClicked()
    {
        // �Q�[����ʂ� GO
        SwitchScene();
    }

    void SwitchScene()
    {
        // Game�V�[�� "DisplayLyrics" ���J��
        SceneManager.LoadScene("DisplayLyrics");
    }

    private void WriteIntoTextbox()
    {
        // ���O�� UI �I�u�W�F�N�g��T��
        Text mic1ColorField = GameObject.Find("Color1").GetComponent<Text>();
        Text mic1NameField = GameObject.Find("MicName1").GetComponent<Text>();

        Text mic2ColorField = GameObject.Find("Color2").GetComponent<Text>();
        Text mic2NameField = GameObject.Find("MicName2").GetComponent<Text>();

        // 
        Text mic3ColorField = GameObject.Find("Color3").GetComponent<Text>();
        Text mic3NameField = GameObject.Find("MicName3").GetComponent<Text>();

        // mic 1
        // �e�L�X�g��ݒ�
        if (mic1ColorField != null)
        {
            string colorName = _assignmentInfoList[0].colorName;
            Color color = NameToColor(colorName);
            mic1ColorField.text = colorName; // Color-Mic1 �Ƀp�[�g�F�\��
            mic1ColorField.color = color;
            ReflectToAvatar("Avatar1", color);
            //_markColorDict
            if (!_markColorDict.ContainsKey(colorName)) _markColorDict[colorName] = _markList[0];
        }
        else
        {
            Debug.LogError("Textbox Color-Mic1 is not assigned.");
        }
        if (mic1NameField != null)
        {
            mic1NameField.text = _assignmentInfoList[0].microphone; // Name-Mic1 �Ƀ}�C�N���\��
        }
        else
        {
            Debug.LogError("Textbox Name-Mic1 is not assigned.");
        }

        // mic 2
        if (mic2ColorField != null)
        {
            string colorName = _assignmentInfoList[1].colorName;
            Color color = NameToColor(colorName);
            mic2ColorField.text = colorName; // Color-Mic2 �Ƀp�[�g�F�\��
            mic2ColorField.color = color;
            ReflectToAvatar("Avatar2", color);
            //_markColorDict
            if (!_markColorDict.ContainsKey(colorName)) _markColorDict[colorName] = _markList[1];
        }
        else
        {
            Debug.LogError("Textbox Color-Mic2 is not assigned.");
        }
        if (mic2NameField != null)
        {
            mic2NameField.text = _assignmentInfoList[1].microphone; // Name-Mic2 �Ƀ}�C�N���\��
        }
        else
        {
            Debug.LogError("Textbox Name-Mic2 is not assigned.");
        }

        // mic 3 �@�B�p
        if (mic3ColorField != null)
        {
            string colorName = "";
            string assignedColor0 = _assignmentInfoList[0].colorName;
            string assignedColor1 = _assignmentInfoList[1].colorName;

            // _colorNameList���疢�g�p�̐F������
            foreach (string name in _colorNameList)
            {
                if (name != assignedColor1 && name != assignedColor0)
                {
                    colorName = name; // ���g�p�̐F��ݒ�
                    break; // �ŏ��Ɍ��������g�p�̐F�Ń��[�v���I��
                }
            }
            Color color = NameToColor(colorName);
            mic3ColorField.text = colorName; // Color-Mic3 �Ƀp�[�g�F�\��
            mic3ColorField.color = color;
            ReflectToAvatar("Avatar3", color);
            //_markColorDict
            if (!_markColorDict.ContainsKey(colorName)) _markColorDict[colorName] = _markList[2];
        }
        else
        {
            Debug.LogError("Textbox Color-Mic1 is not assigned.");
        }
        if (mic3NameField != null)
        {
            mic3NameField.text = "Robot Part"; // Name-Mic3 �͋@�B���S��, �����Ŗ��_���Z
        }
        else
        {
            Debug.LogError("Textbox Name-Mic1 is not assigned.");
        }

    }

    private void AssignColorsToMicrophones()
    {
        // �}�C�N�f�o�C�X���擾
        foreach (string deviceName in Microphone.devices)
        {
            // �p�\�R���{�̂̃}�C�N�͊܂߂Ȃ�
            // USB �Őڑ����ꂽ�}�C�N�̂݃��X�g�ɒǉ�������
            if (deviceName == "�}�C�N�z�� (Realtek(R) Audio)")
            {
                continue;
            }

            _microphoneNameList.Add(deviceName);
        }

        if (_microphoneNameList.Count == 0)
        {
            //string deviceName = "No_Mic";
            //_microphoneNameList.Add(deviceName);
            Debug.LogError("No microphones detected.");
            return;
        }

        Debug.Log($"Detected {_microphoneNameList.Count} microphones.");

        // �F�������_���Ɋ��蓖��

        // ���ƂȂ鐔�� index of color
        List<int> numberList = new List<int> { 0, 1, 2 };

        // ���ʂ��i�[���郊�X�g
        List<int> indexList = SelectTwoRandomIndexes(numberList);

        // ���ʂ����O�o��
        Debug.Log($"Selected indexes: {indexList[0]}, {indexList[1]}");

        // �F�����蓖��
        for (int i = 0; i < _microphoneNameList.Count; i++)
        {
            string assignedColor = _colorNameList[indexList[i]]; // �F�����ԂɊ��蓖��

            // �\���̂ɒǉ�
            _assignmentInfoList.Add(new ColorInfo
            {
                microphone = _microphoneNameList[i],
                colorName = assignedColor,
                mark = _markList[i]
            });

            Debug.Log($"Assigned color {assignedColor} to {_microphoneNameList[i]}");
        }
    }

    List<int> SelectTwoRandomIndexes(List<int> numberList)
    {
        List<int> result = new List<int>();

        // �V���b�t�����ď��2���擾
        for (int i = numberList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (numberList[i], numberList[randomIndex]) = (numberList[randomIndex], numberList[i]);
        }

        result.Add(numberList[0]);
        result.Add(numberList[1]);

        return result;
    }
    
    private void SaveMarkDictToFile()
    {
        string filePath = Path.Combine(Application.dataPath, "MarkColorDict.txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var markAssign in _markColorDict)
            {
                writer.WriteLine($"{markAssign.Key}, {markAssign.Value}");
            }
        }

        Debug.Log($"Color information saved to {filePath}");
    }

    private void SaveColorInfoToFile()
    {
        string filePath = Path.Combine(Application.dataPath, _outputFileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (ColorInfo colorInfo in _assignmentInfoList)
            {
                writer.WriteLine($"{colorInfo.colorName}, {colorInfo.microphone}, {colorInfo.mark}");
                // RED, �}�C�N (Logi C615 HD WebCam)
                // �݂����Ȍ`���ŕۑ������
            }
        }

        Debug.Log($"Color information saved to {filePath}");
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ReflectToAvatar(string objName, Color color)
    {
        //objName = "Avatar1" 
        // Canvas���� objName �I�u�W�F�N�g��T��
        GameObject avatarObject = GameObject.Find(objName);

        // �I�u�W�F�N�g�����������ꍇ
        if (avatarObject != null)
        {
            // Image�R���|�[�l���g���擾
            Image avatarImage = avatarObject.GetComponent<Image>();

            if (avatarImage != null)
            {
                // Image��Color�v���p�e�B�ɐF��ݒ�
                avatarImage.color = color;
                Debug.Log($"Color {color} applied to {objName}.");
            }
            else
            {
                Debug.LogError($"{objName} does not have an Image component.");
            }
        }
        else
        {
            Debug.LogError($"{objName} object not found in the scene.");
        }
    }

    Color NameToColor(string colorName)
    {
        if (colorName == "RED") return Color.red;
        if (colorName == "GREEN") return Color.green;
        if (colorName == "YELLOW") return Color.yellow;
        return Color.white;
    }
    string MarkToChar(string markName)
    {
        if (markName == "Heart") return "*";
        if (markName == "Spade") return "!";
        if (markName == "Diamond") return "+";
        if (markName == "Club") return "#";

        string chorusMark = "<<";
        //string chorusMark = "";
        //int i = 0;
        //while (i < _playerCount)
        //{
        //    chorusMark += "<";
        //    i++;
        //}
        return chorusMark; // �S���ŉ̂�����
    }
}
