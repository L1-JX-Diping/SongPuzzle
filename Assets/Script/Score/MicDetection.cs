using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MicDetection : MonoBehaviour
{
    private float _detectionThreshold = 0.01f; // ���������m����U����臒l
    private int _sampleWindow = 128; // ���ʉ�͂Ɏg�p����T���v����
    private int _index = 0;

    private Data _data = new Data();

    private List<AudioClip> _microphoneClips = new List<AudioClip>(); // �}�C�N����̓��͂�ۑ�
    private List<string> _microphoneNames = new List<string>(); // �ڑ�����Ă���}�C�N�̖��O
    private List<Detection> _detectedList = new List<Detection>(); // ���o�f�[�^�̋L�^

    private List<Part> _correctParts = new List<Part>(); // �̂��o���^�C�~���O���X�g

    void Start()
    {
        // Load correct part division
        LoadData();

        // �}�C�N�̏����ݒ�
        InitializeMicrophones();

        // ButtonClicked: Save information and Switch scene
        GameObject.Find("ViewScore").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        List<Line> lyricsList = _data.Song.Lyrics;

        /* Set correct parts */
        foreach (Line line in lyricsList)
        {
            foreach (Part part in line.PartList)
            {
                _correctParts.Add(part);
            }
        }

        Debug.Log("PartLog loaded successfully.");
    }

    private void ButtonClicked()
    {
        // Save mic detection log (List<Detection>)
        SaveLogToXML(); // List of Detection(time, mic, volume)

        //
        SceneManager.LoadScene("Score");
    }


    void Update()
    {
        float detectionTime = Time.timeSinceLevelLoad;

        // ���݂̃p�[�g���̂��n�߂�ׂ����ԂɂȂ����琺�̌��o���J�n
        if (_index < _correctParts.Count && _correctParts[_index].Timing <= detectionTime)
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
                        Debug.Log($"Mic detected from {micName} at {detectionTime:F2} seconds, Volume: {volume:F4}");
                        _detectedList.Add(new Detection
                        {
                            Time = detectionTime,
                            Mic = micName,
                            Volume = volume
                        });
                    }
                }
            }
            _index++;
        }
        // ����ۑ�
        SaveLogToXML();
    }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="micClip"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Save detection log data to xml file
    /// </summary>
    public void SaveLogToXML()
    {
        Common.ExportToXml(_detectedList, FileName.XmlMicLog);
    }

}
