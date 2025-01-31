using System.IO;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    public AudioSource _audioSource; // AudioSource���A�^�b�`
    public AudioClip _birthdaySongClip; // "Birthday Song" �� AudioClip
    public AudioClip _otherSongClip;    // ���̋Ȃ�AudioClip
    private string _songName; // GameInfo.txt����擾����Ȗ�
    private int _playerCount;
    private string _gameInfoFileName = "GameInfo.txt";

    void Start()
    {
        ReadAndSetGameData();

        // �Ȗ��ɉ����ĉ������Đ�
        PlaySongBasedOnName(_songName);
    }

    private void ReadAndSetGameData()
    {
        // GameInfo.txt�̃t�@�C���p�X
        string filePath = Path.Combine(Application.dataPath, _gameInfoFileName);

        // GameInfo.txt��ǂݍ���
        if (File.Exists(filePath))
        {
            // �t�@�C���̓��e���s���Ƃɓǂݍ���
            string[] lineList = File.ReadAllLines(filePath);

            if (lineList.Length >= 2) // �t�@�C����2�s�ȏ゠�邱�Ƃ��m�F
            {
                // 1�s�ڂ�_songName�Ɋi�[
                _songName = lineList[0];

                // 2�s�ڂ�_playerNum�Ɋi�[�i�����񂩂�int�^�ɕϊ��j
                if (int.TryParse(lineList[1], out int playerNum))
                {
                    _playerCount = playerNum;
                }
                else
                {
                    Debug.LogError($"Invalid number format in line 2: {lineList[1]}");
                    _playerCount = 0; // �f�t�H���g�l
                }

                Debug.Log($"Song name: {_songName}, Player number: {_playerCount}");
            }
            else
            {
                Debug.LogError("GameInfo.txt does not contain enough lines.");
            }
        }
        else
        {
            Debug.LogError("GameInfo.txt not found.");
        }
    }

    void PlaySongBasedOnName(string songName)
    {
        // �Ȗ��ɉ�����AudioClip��ݒ�
        if (songName == "Birthday Song")
        {
            _audioSource.clip = _birthdaySongClip;
            Debug.Log("Birthday Song is selected.");
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
