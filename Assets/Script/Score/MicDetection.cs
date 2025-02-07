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
    private float _detectionThreshold = 0.01f; // 音声を検知する振幅の閾値
    private int _sampleWindow = 128; // 音量解析に使用するサンプル数
    private int _index = 0;

    private Data _data = new Data();

    private List<AudioClip> _microphoneClips = new List<AudioClip>(); // マイクからの入力を保存
    private List<string> _microphoneNames = new List<string>(); // 接続されているマイクの名前
    private List<Detection> _detectedList = new List<Detection>(); // 検出データの記録

    private List<Part> _correctParts = new List<Part>(); // 歌い出しタイミングリスト

    void Start()
    {
        // Load correct part division
        LoadData();

        // マイクの初期設定
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

        // 現在のパートを歌い始めるべき時間になったら声の検出を開始
        if (_index < _correctParts.Count && _correctParts[_index].Timing <= detectionTime)
        {
            // 各マイクについて音声を検出
            for (int i = 0; i < _microphoneClips.Count; i++)
            {
                string micName = _microphoneNames[i];
                AudioClip micClip = _microphoneClips[i];

                if (micClip != null && Microphone.IsRecording(micName))
                {
                    // 現在の音量を取得
                    float volume = GetVolumeLevel(micClip);

                    // 音量が閾値を超えたら音声を検知
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
        // 逐一保存
        SaveLogToXML();
    }

    /// <summary>
    /// 接続されたマイクを検出
    /// </summary>
    private void InitializeMicrophones()
    {
        // 接続されているマイクを取得
        if (Microphone.devices.Length > 0)
        {
            foreach (string device in Microphone.devices)
            {
                _microphoneNames.Add(device);
                Debug.Log($"Found microphone: {device}");

                // マイク入力の録音を開始
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
        // サンプルデータを取得
        float[] data = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - _sampleWindow + 1;
        if (micPosition < 0) return 0;

        micClip.GetData(data, micPosition);

        // サンプルデータの最大振幅を計算
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
