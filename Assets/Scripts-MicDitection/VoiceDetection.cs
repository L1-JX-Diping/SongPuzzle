using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class VoiceDetection : MonoBehaviour
{
    private float _detectionThreshold = 0.01f; // ���������m����U����臒l
    private int _sampleWindow = 128; // ���ʉ�͂Ɏg�p����T���v����
    private string _outputFileName = "VoiceDetectionLog.txt"; // �L�^����t�@�C����
    private int _index = 0;

    private AudioClip _microphoneClip; // �}�C�N����̓��͂�ۑ�
    private List<AudioClip> _microphoneClipList; // �����}�C�N����̓��͂�ۑ�
    
    // �ǂݍ��݃t�@�C�� �̂��o�������̏��
    private string _partLogFileName = "PartLog.txt"; // �t�@�C����
    // �ǂݍ��񂾊e�p�[�g�̂��o�������f�[�^���i�[���郊�X�g
    private List<StartSingInfo> _timingList = new List<StartSingInfo>(); 
    // �}�C�N�𕡐��g���ꍇ�p
    private List<string> _microphoneNameList = new List<string>();

    // Time Mic �i�[���郊�X�g // ���������o���ꂽ�b����Volume���L�^
    private List<MicVolumeInfo> _detectedList = new List<MicVolumeInfo>();
    string _microphoneName = "";

    [System.Serializable]
    public class StartSingInfo
    {
        public float timing; // �^�C�~���O, ����
        public string color; // �p�[�g��, �F
    }

    [System.Serializable]
    public class MicVolumeInfo
    {
        public float time; // �^�C�~���O, ����
        public string mic; // �p�[�g��, �F
        public float volume; // ���o�������̑傫��
    }

    // Start is called before the first frame update
    void Start()
    {
        // set info from PartLog file
        ReadPartLog();

        // Mic �֘A�̏����ݒ���L�^�p�ݒ菔�X
        SetMic();
    }

    private void SetMic()
    {
        // �}�C�N���ڑ�����Ă��邩�m�F
        if (Microphone.devices.Length > 0)
        {
            _microphoneName = Microphone.devices[0];
            Debug.Log($"Using microphone: {_microphoneName}");

            // �}�C�N���̘͂^�����J�n
            _microphoneClip = Microphone.Start(_microphoneName, true, 10, 44100);
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }

        //// �}�C�N���ڑ�����Ă��邩�m�F
        //int micNum = Microphone.devices.Length;
        //if (micNum > 0)
        //{
        //    //string microphoneName = Microphone.devices[0];

        //    // mic �������ڑ�����Ă���ꍇ
        //    int micIndex = 0;
        //    while (micIndex < micNum)
        //    {
        //        _microphoneNameList[micIndex] = Microphone.devices[micIndex];
        //        Debug.Log($"Using microphone: {_microphoneNameList[micIndex]}");
        //        // �}�C�N���̘͂^�����J�n
        //        _microphoneClipList.Add(Microphone.Start(_microphoneNameList[micIndex], true, 10, 44100));
        //    }
        //}
        //else
        //{
        //    Debug.LogError("No microphone detected.");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        float detectionTime = Time.timeSinceLevelLoad;

        // ���݂̃p�[�g���̂��n�߂�ׂ����ԂɂȂ����琺�̌��o���J�n
        if (_timingList[_index].timing <= detectionTime)
        {
            // ��񂾂����m �s�b�^���^�C�~���O�ŉ̂��Ă��Ȃ������甽�f����Ȃ�
            // ���\�n�[�h���[�h
            if (_microphoneClip != null && Microphone.IsRecording(null))
            {
                // ���݂̉��ʂ��擾
                float volume = GetVolumeLevel();

                // ���ʂ�臒l�𒴂����特�������m
                if (volume > _detectionThreshold)
                {
                    //Debug.Log($"Voice detected at {detectionTime:F2} seconds, Volume: {volume:F4}");

                    // �L�^
                    _detectedList.Add(new MicVolumeInfo { 
                        time = detectionTime, 
                        mic = _microphoneName, 
                        volume = volume
                    });
                }
            }
            _index++;
            //Debug.Log($"_index: {_index}");
        }
    }
    void ReadPartLog()
    {
        string filePath = Path.Combine(Application.dataPath, _partLogFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string[] lineList = File.ReadAllLines(filePath);

        foreach (string line in lineList)
        {
            // �R���}�ŋ�؂��ĕ���
            string[] partInfo = line.Split(',');

            if (partInfo.Length == 2)
            {
                // �^�C�~���O�ƐF����͂��ă��X�g�ɒǉ�
                if (float.TryParse(partInfo[0].Trim(), out float startTime))
                {
                    string color = partInfo[1].Trim();
                    _timingList.Add(new StartSingInfo { timing = startTime, color = color });
                }
                else
                {
                    Debug.LogWarning($"Invalid timing format in line: {line}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid line format: {line}");
            }
        }
    }

    private float GetVolumeLevel()
    {
        // �T���v���f�[�^���擾
        float[] data = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - _sampleWindow + 1;
        if (micPosition < 0) return 0;

        _microphoneClip.GetData(data, micPosition);

        // �T���v���f�[�^�̍ő�U�����v�Z
        float maxAmplitude = 0;
        foreach (float sample in data)
        {
            maxAmplitude = Mathf.Max(maxAmplitude, Mathf.Abs(sample));
        }

        return maxAmplitude;
    }

    private void OnApplicationQuit()
    {
        // �������m���Ԃ��t�@�C���ɋL�^
        string filePath = System.IO.Path.Combine(Application.dataPath, _outputFileName);
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
        {
            //writer.WriteLine("Voice Detection Log:");

            // time �� mic �̏����o�͂ł����炢��
            foreach (var info in _detectedList)
            {
                writer.WriteLine($"{info.time:F2}, {info.mic}, {info.volume}");
            }
        }

        Debug.Log($"Voice detection times saved to: {filePath}");
    }

}
