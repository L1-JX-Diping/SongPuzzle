using System.IO;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public AudioSource _audioSource; // AudioSourceをアタッチ
    public AudioClip _birthdaySongClip; // "Birthday Song" の AudioClip
    public AudioClip _otherSongClip;    // 他の曲のAudioClip

    private Data _data = new Data();
    private string _songName; 

    void Start()
    {
        LoadData();

        // 曲名に応じて音声を再生
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

        // 曲名に応じてAudioClipを設定
        if (songName == "Birthday Song")
        {
            _audioSource.clip = _birthdaySongClip;
        }
        else
        {
            _audioSource.clip = _otherSongClip; // 他の曲を再生
        }

        // AudioClipが設定されていれば再生
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
