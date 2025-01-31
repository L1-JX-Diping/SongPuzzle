using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    private string currentMicrophone = "";  // ���ݎg�p���̃}�C�N
    private float startTime = 0f;           // �^���J�n����
    private bool isRecording = false;      // �^�������ǂ����̃t���O
    private AudioClip recordingClip;       // �^���f�[�^
    private Dictionary<string, float> microphoneUsageTimes = new Dictionary<string, float>(); // �}�C�N�g�p����

    void Start()
    {
        // �ڑ�����Ă��邷�ׂẴ}�C�N���擾
        string[] microphones = Microphone.devices;

        if (microphones.Length == 0)
        {
            Debug.LogError("No microphones detected.");
            return;
        }

        // �}�C�N�ꗗ��\��
        Debug.Log("Detected microphones:");
        foreach (var mic in microphones)
        {
            Debug.Log($"- {mic}");
            microphoneUsageTimes[mic] = 0f; // �e�}�C�N�̎g�p���Ԃ�������
        }

        // �ŏ��̃}�C�N��I�����Ę^���J�n
        StartRecording(microphones[0]);
    }

    void Update()
    {
        if (isRecording)
        {
            // ���݂̘^���o�ߎ��Ԃ��擾
            float elapsedTime = Time.time - startTime;

            // ���݂̘^���ʒu���擾�i�T���v���P�ʁj
            int position = Microphone.GetPosition(currentMicrophone);
            Debug.Log($"Recording on {currentMicrophone}: {elapsedTime:F2} seconds (Position: {position} samples)");

            // �G�X�P�[�v�L�[�Ř^�����~
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopRecording();
            }
        }

        // ���̃}�C�N�ɐ؂�ւ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchMicrophone();
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

        // �}�C�N��I�����Ę^���J�n
        currentMicrophone = microphone;
        recordingClip = Microphone.Start(microphone, true, 10, 44100); // �ő�10�b�̃��[�v�^��
        startTime = Time.time;
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
        float elapsedTime = Time.time - startTime;

        // �g�p���Ԃ��L�^
        if (microphoneUsageTimes.ContainsKey(currentMicrophone))
        {
            microphoneUsageTimes[currentMicrophone] += elapsedTime;
        }

        Debug.Log($"Stopped recording with {currentMicrophone}. Total usage time: {microphoneUsageTimes[currentMicrophone]:F2} seconds.");
        isRecording = false;

        // Save the mic usage information
        SaveUsageLog();
    }


    void SwitchMicrophone()
    {
        // ���̃}�C�N�ɐ؂�ւ�
        string[] microphones = Microphone.devices;
        if (microphones.Length < 2)
        {
            Debug.LogWarning("Only one microphone available. Cannot switch.");
            return;
        }

        // ���݂̃}�C�N�̃C���f�b�N�X���擾
        int currentIndex = System.Array.IndexOf(microphones, currentMicrophone);
        int nextIndex = (currentIndex + 1) % microphones.Length;

        Debug.Log($"Switching to microphone: {microphones[nextIndex]}");
        StartRecording(microphones[nextIndex]);
    }

    void SaveUsageLog()
    {
        string logPath = Path.Combine(Application.dataPath, "MicrophoneUsageLog.txt");
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            writer.WriteLine("Microphone Usage Log:");
            foreach (var entry in microphoneUsageTimes)
            {
                writer.WriteLine($"{entry.Key}: {entry.Value:F2} seconds");
            }
        }
        Debug.Log($"Usage log saved to {logPath}");
    }

}
