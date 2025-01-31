using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MicUsageRecorder : MonoBehaviour
{
    private string[] microphones; // ���o���ꂽ�}�C�N�̃��X�g
    private string currentMicrophone = ""; // ���ݎg�p���̃}�C�N
    private bool isRecording = false; // �^�����t���O
    private float startTime = 0f; // �^���J�n����
    private List<MicUsage> micUsageLog = new List<MicUsage>(); // �}�C�N�g�p���̃��X�g
    private AudioClip recordingClip;

    // �}�C�N�g�p����ێ�����N���X
    private class MicUsage
    {
        public string Mic;       // �}�C�N��
        public float TimeCount;  // �g�p����
        public float StartTime;  // �J�n����
        public float EndTime;    // �I������
    }

    void Start()
    {
        // �ڑ�����Ă���}�C�N�����o
        microphones = Microphone.devices;

        if (microphones.Length == 0)
        {
            Debug.LogError("No microphones detected.");
            return;
        }

        Debug.Log("Detected microphones:");
        foreach (var mic in microphones)
        {
            Debug.Log($"- {mic}");
        }

        // �ŏ��̃}�C�N�Ř^���J�n
        StartRecording(microphones[0]);
    }

    void Update()
    {
        if (isRecording)
        {
            // ���݂̘^���o�ߎ��Ԃ��擾
            float currentTime = Time.timeSinceLevelLoad;

            Debug.Log($"Recording on {currentMicrophone}: StartTime = {startTime:F2}, Elapsed = {currentTime - startTime:F2}");

            // �^����؂�ւ���ꍇ�i��F�X�y�[�X�L�[�Ő؂�ւ��j
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopRecording();
                SwitchMicrophone();
            }

            // �^�����I���i��F�G�X�P�[�v�L�[�ŏI���j
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopRecording();
                SaveUsageLog();
            }
        }
    }

    void StartRecording(string microphone)
    {
        if (Microphone.devices.Length == 0 || string.IsNullOrEmpty(microphone))
        {
            Debug.LogError("No microphone available to start recording.");
            return;
        }

        if (isRecording)
        {
            StopRecording();
        }

        currentMicrophone = microphone;
        recordingClip = Microphone.Start(microphone, true, 10, 44100); // �ő�10�b�̃��[�v�^��
        startTime = Time.timeSinceLevelLoad;
        isRecording = true;

        Debug.Log($"Started recording with microphone: {currentMicrophone}");
    }

    void StopRecording()
    {
        if (string.IsNullOrEmpty(currentMicrophone) || !isRecording)
        {
            return;
        }

        // �^�����~
        Microphone.End(currentMicrophone);
        float endTime = Time.timeSinceLevelLoad;
        float timeCount = endTime - startTime;

        // �g�p�����L�^
        micUsageLog.Add(new MicUsage
        {
            Mic = currentMicrophone,
            TimeCount = timeCount,
            StartTime = startTime,
            EndTime = endTime
        });

        Debug.Log($"Stopped recording with {currentMicrophone}. TimeCount = {timeCount:F2}");
        isRecording = false;
    }

    void SwitchMicrophone()
    {
        // ���̃}�C�N�ɐ؂�ւ���
        if (microphones.Length < 2)
        {
            Debug.LogWarning("Only one microphone available. Cannot switch.");
            return;
        }

        int currentIndex = System.Array.IndexOf(microphones, currentMicrophone);
        int nextIndex = (currentIndex + 1) % microphones.Length;

        StartRecording(microphones[nextIndex]);
    }

    void SaveUsageLog()
    {
        string logPath = Path.Combine(Application.dataPath, "MicUsage.txt");

        using (StreamWriter writer = new StreamWriter(logPath))
        {
            // 1�s�ڂɕϐ������L�^
            writer.WriteLine("Mic,TimeCount,StartTime,EndTime");

            // �e�g�p�����L�^
            foreach (var usage in micUsageLog)
            {
                writer.WriteLine($"{usage.Mic},{usage.TimeCount:F2},{usage.StartTime:F2},{usage.EndTime:F2}");
            }
        }

        Debug.Log($"Mic usage log saved to {logPath}");
    }
}
