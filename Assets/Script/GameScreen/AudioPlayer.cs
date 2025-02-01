using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource audioSource; // �Đ�����AudioSource
    public AudioClip audioClip;     // �Đ����鉹���t�@�C��
    public float delayInSeconds = 0f; // �Đ����J�n����x�����ԁi�b�j

    private bool hasPlayed = false; // �������Đ��ς݂��ǂ���

    void Start()
    {
        // AudioSource��AudioClip��ݒ�
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is not assigned.");
        }
    }

    void Update()
    {
        // �V�[�����J�n����X�b���߂����特�����Đ�
        if (!hasPlayed && Time.timeSinceLevelLoad >= delayInSeconds)
        {
            audioSource.Play();
            hasPlayed = true; // �Đ��ς݃t���O��ݒ�
            Debug.Log("Audio started playing.");
        }
    }
}
