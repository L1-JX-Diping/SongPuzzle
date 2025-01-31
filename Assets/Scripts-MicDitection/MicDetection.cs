using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MicDetection : MonoBehaviour
{
    private float _detectionThreshold = 0.01f; // ���������m����U����臒l
    private int _sampleWindow = 128; // ���ʉ�͂Ɏg�p����T���v����
    private string _outputFileName = "MicDetectionLog.txt"; // �L�^����t�@�C����
    private int _index = 0;

    private List<AudioClip> _microphoneClips = new List<AudioClip>(); // �}�C�N����̓��͂�ۑ�
    private List<string> _microphoneNames = new List<string>(); // �ڑ�����Ă���}�C�N�̖��O
    private List<MicVolumeInfo> _detectedList = new List<MicVolumeInfo>(); // ���o�f�[�^�̋L�^

    private List<StartSingInfo> _timingList = new List<StartSingInfo>(); // �̂��o���^�C�~���O���X�g

    [System.Serializable]
    public class StartSingInfo
    {
        public float timing; // �^�C�~���O
        public string color; // �F
    }

    [System.Serializable]
    public class MicVolumeInfo
    {
        public float time; // ���o����
        public string mic; // ���o���ꂽ�}�C�N��
        public float volume; // ���o���ꂽ����
    }

    void Start()
    {
        // PartLog�t�@�C����ǂݍ���
        ReadPartLog();

        // �}�C�N�̏����ݒ�
        InitializeMicrophones();
    }
    //private void InitializeMicrophones()
    //{
    //    // USB�}�C�N��ێ����郊�X�g
    //    List<string> usbMicrophones = new List<string>();

    //    // �ڑ�����Ă��邷�ׂẴ}�C�N���擾
    //    foreach (string device in Microphone.devices)
    //    {
    //        Debug.Log($"Detected microphone: {device}");

    //        // �p�\�R���{�̂̃}�C�N�͖������AUSB �Őڑ������}�C�N�̂݌��o
    //        // �}�C�N���� "WebCam" ���܂ނ��ǂ������`�F�b�N
    //        if (device.ToLower().Contains("WebCam"))
    //        {
    //            usbMicrophones.Add(device);
    //            Debug.Log($"USB Microphone found: {device}");

    //            // USB�}�C�N���̘͂^�����J�n
    //            AudioClip clip = Microphone.Start(device, true, 10, 44100);
    //            _microphoneNames.Add(device);
    //            _microphoneClips.Add(clip);
    //        }
    //        else
    //        {
    //            Debug.Log($"Skipping non-USB microphone: {device}");
    //        }
    //    }

    //    // USB�}�C�N��������Ȃ��ꍇ�̃G���[�n���h�����O
    //    if (usbMicrophones.Count == 0)
    //    {
    //        Debug.LogError("No USB microphones detected. Please connect a USB microphone.");
    //    }
    //}

    /// <summary>
    /// �ڑ����ꂽ�}�C�N�����o
    /// </summary>
    private void InitializeMicrophones()
    {
        // �ڑ�����Ă���}�C�N���擾
        if (Microphone.devices.Length > 0)
        {
            foreach (string device in Microphone.devices)
            {
                _microphoneNames.Add(device);
                Debug.Log($"Found microphone: {device}");

                // �}�C�N���̘͂^�����J�n
                AudioClip clip = Microphone.Start(device, true, 10, 44100);
                _microphoneClips.Add(clip);
            }
        }
        else
        {
            Debug.LogError("No microphones detected.");
        }
    }

    void Update()
    {
        float detectionTime = Time.timeSinceLevelLoad;

        // ���݂̃p�[�g���̂��n�߂�ׂ����ԂɂȂ����琺�̌��o���J�n
        if (_index < _timingList.Count && _timingList[_index].timing <= detectionTime)
        {
            // �e�}�C�N�ɂ��ĉ��������o
            for (int i = 0; i < _microphoneClips.Count; i++)
            {
                string micName = _microphoneNames[i];
                AudioClip micClip = _microphoneClips[i];

                if (micClip != null && Microphone.IsRecording(micName))
                {
                    // ���݂̉��ʂ��擾
                    float volume = GetVolumeLevel(micClip);

                    // ���ʂ�臒l�𒴂����特�������m
                    if (volume > _detectionThreshold)
                    {
                        Debug.Log($"Voice detected from {micName} at {detectionTime:F2} seconds, Volume: {volume:F4}");
                        _detectedList.Add(new MicVolumeInfo
                        {
                            time = detectionTime,
                            mic = micName,
                            volume = volume
                        });
                    }
                }
            }
            _index++;
        }
    }

    private float GetVolumeLevel(AudioClip micClip)
    {
        // �T���v���f�[�^���擾
        float[] data = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - _sampleWindow + 1;
        if (micPosition < 0) return 0;

        micClip.GetData(data, micPosition);

        // �T���v���f�[�^�̍ő�U�����v�Z
        float maxAmplitude = 0;
        foreach (float sample in data)
        {
            maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(sample));
        }

        return maxAmplitude;
    }

    private void ReadPartLog()
    {
        string filePath = Path.Combine(Application.dataPath, "PartLog.txt");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // �R���}�ŋ�؂��ĕ���
            string[] parts = line.Split(',');

            if (parts.Length == 2 && float.TryParse(parts[0].Trim(), out float timing))
            {
                string color = parts[1].Trim();
                _timingList.Add(new StartSingInfo { timing = timing, color = color });
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        // �������m���Ԃ��t�@�C���ɋL�^
        string filePath = Path.Combine(Application.dataPath, _outputFileName);
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"# time, mic, volume");
            foreach (var info in _detectedList)
            {
                writer.WriteLine($"{info.time:F2}, {info.mic}, {info.volume:F4}");
            }
        }

        Debug.Log($"Voice detection log saved to: {filePath}");
    }
}
