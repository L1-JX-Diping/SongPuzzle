using System.IO;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public AudioSource _audioSource; // AudioSource���A�^�b�`
    public AudioClip _birthdaySongClip; // "Birthday Song" �� AudioClip
    public AudioClip _otherSongClip;    // ���̋Ȃ�AudioClip

    private Data _data = new Data();
    private string _songName; 

    void Start()
    {
        LoadData();

        // �Ȗ��ɉ����ĉ������Đ�
        PlaySongBasedOnName(_songName);
    }

    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        _songName = _data.Song.Title;
    }

    /// <summary>
    /// Play song based on the song title got from file
    /// </summary>
    /// <param name="songName"></param>
    private void PlaySongBasedOnName(string songName)
    {
        Debug.Log($"{songName} is selected.");

        // �Ȗ��ɉ�����AudioClip��ݒ�
        if (songName == "Birthday Song")
        {
            _audioSource.clip = _birthdaySongClip;
        }
        else
        {
            _audioSource.clip = _otherSongClip; // ���̋Ȃ��Đ�
        }

        // AudioClip���ݒ肳��Ă���΍Đ�
        if (_audioSource.clip != null)
        {
            _audioSource.Play();
            Debug.Log($"Playing song: {_audioSource.clip.name}");
        }
        else
        {
            Debug.LogError("No AudioClip assigned for the given song name.");
        }
    }
}
