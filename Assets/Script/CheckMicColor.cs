using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CheckMicColorUI : MonoBehaviour
{
    // �t�@�C����
    private string _micColorInfoFileName = "MicColorInfo.txt";

    // �}�C�N����Image�̃}�b�s���O
    private Dictionary<string, Image> _micToImageMapDict = new Dictionary<string, Image>();

    // �}�C�N����AudioClip�̑Ή�
    private Dictionary<string, AudioClip> _micClipDict = new Dictionary<string, AudioClip>();

    // �T���v���f�[�^�擾�p�̃E�B���h�E�T�C�Y
    private int _sampleWindow = 128;

    // ���点��^�C�}�[�̊Ǘ��p
    private float _glowDuration = 0.5f; // ���点�鎞�� (�b)
    private float _timer = 0f; // �^�C�}�[

    private Image _currentGlowingImage = null; // ���݌����Ă���Image

    public class MicVolumeInfo
    {
        public string mic; // ���o���ꂽ�}�C�N��
        public float volume; // ���o���ꂽ����
    }

    void Start()
    {
        // �t�@�C������}�C�N��Image�̑Ή���ǂݍ��� Dict �ɃZ�b�g����
        SetMicObjDict();

        // �}�C�N��������
        InitializeMicrophones();
    }

    void Update()
    {
        // �^�C�}�[���X�V
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            // �^�C�}�[��0�ȉ��Ȃ玟�̃}�C�N�����o���Č��点��
            MicVolumeInfo maxMic = DetectLoudestMic();

            if (maxMic != null && _micToImageMapDict.ContainsKey(maxMic.mic))
            {
                // �O����点�Ă���Image�̐F�����Z�b�g
                if (_currentGlowingImage != null)
                {
                    _currentGlowingImage.color = new Color(1f, 1f, 1f, 0.5f); // �������̔��F
                }

                // �����Image�����点��
                _currentGlowingImage = _micToImageMapDict[maxMic.mic];
                _currentGlowingImage.color = new Color(1f, 1f, 0f, 1f); // ���鉩�F

                // �^�C�}�[�����Z�b�g
                _timer = _glowDuration;
            }
        }
    }

    void SetMicObjDict()
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
            int num = -1;
            num++;

            if (parts.Length == 2)
            {
                string micName = parts[1].Trim();
                string objectName = "Image" + num.ToString();

                GameObject obj = GameObject.Find(objectName);

                if (obj != null && obj.GetComponent<Image>() != null)
                {
                    _micToImageMapDict[micName] = obj.GetComponent<Image>();
                    Debug.Log($"Mapped mic '{micName}' to UI Image '{objectName}'");
                }
                else
                {
                    Debug.LogError($"UI Image '{objectName}' not found in the scene.");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }
    }

    void InitializeMicrophones()
    {
        foreach (string micName in Microphone.devices)
        {
            Debug.Log($"Initializing microphone: {micName}");

            AudioClip clip = Microphone.Start(micName, true, 10, 44100);
            _micClipDict[micName] = clip;
        }
    }

    MicVolumeInfo DetectLoudestMic()
    {
        MicVolumeInfo micVolume = null;
        float maxVolume = 0;

        foreach (var mic in _micClipDict)
        {
            string micName = mic.Key;
            AudioClip clip = mic.Value;

            if (clip != null && Microphone.IsRecording(micName))
            {
                float volume = GetVolumeLevel(clip);

                if (volume > maxVolume)
                {
                    maxVolume = volume;
                    micVolume = new MicVolumeInfo
                    {
                        mic = micName,
                        volume = volume
                    };
                }
            }
        }

        return micVolume;
    }

    float GetVolumeLevel(AudioClip clip)
    {
        float[] data = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - _sampleWindow + 1;

        if (micPosition < 0) return 0;

        clip.GetData(data, micPosition);

        float maxAmplitude = 0;
        foreach (float sample in data)
        {
            maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(sample));
        }

        return maxAmplitude;
    }
}
