using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾


public class DisplayLyrics : MonoBehaviour
{
    // ����: _textField �������O�ύX����Ȃ� Inspector �̂Ƃ���� TextMeshPro �݂����Ȃ� Row1,2,3 ����Ȃ�����
    public TextMeshProUGUI[] _textField; // �̎���\������TextMeshProUGUI�I�u�W�F�N�g�i3�s���j

    Data _data = new Data();
    private int _currentLyricIndex = 0; // ���݂̉̎��C���f�b�N�X
    private float _loadingTime = 0.8f; // time lag
    // Load _lyricsList // �̎����(�\���J�n�����{�\������̎�)���i�[���郊�X�g
    private List<Line> _lyricsLines = new List<Line>(); 
    // Load _playerList 
    private List<Player> _playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        Division division = new Division();
        division.DoDivision();

        // get division data
        LoadAndSetData();

        InitAvatarColor(); // ��ʏ�ɃA�o�^�[�̐F�𔽉f����

        UpdateLyricsDisplay(); // �����\�����X�V
    }

    // Update is called once per frame
    void Update()
    {
        // ���݂̎����Ɋ�Â��ĉ̎����X�V
        float currentTime = Time.timeSinceLevelLoad;

        /* Update(Scroll) lyrics displayed on the screen */
        int maxIndex = _lyricsLines.Count - 1;
        if (_currentLyricIndex >= maxIndex)
        {
            SwitchScene();
            return;
        }
        Line nextLine = _lyricsLines[_currentLyricIndex + 1];

        // ���̉̎��s�ɐi�ނׂ��^�C�~���O���m�F
        if (_currentLyricIndex < maxIndex && currentTime >= nextLine.Timing - _loadingTime)
        {
            _currentLyricIndex++;
            UpdateLyricsDisplay();
        }

        /* Update Avatar Color */
        // Avatar �̃p�[�g�̂Ƃ��Ɍ���ȂǕω�����
    }

    /// <summary>
    /// 
    /// </summary>
    private void SwitchScene()
    {
        SceneManager.LoadScene("Score");

        //if (true)
        //{
        //    // if finish to play this song, swith scene to "Score"
        //    SceneManager.LoadScene("Score");
        //}
        //else
        //{
        //    // 
        //    SceneManager.LoadScene("Home");
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadAndSetData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);

        _lyricsLines = _data.Song.Lines;
        _playerList = _data.Team.MemberList;

        // for Debug
        foreach (Line line in _lyricsLines)
        {
            Debug.Log($"Load _lyricsList: {line.Timing}: {line.Text}");
        }
        foreach (Player player in _playerList)
        {
            Debug.Log($"Load _playerList: {player.Name} is expected to sing {Common.ToColorName(player.Role.Color)} parts.");
        }
    }

    /// <summary>
    /// Update screen: set color to avatar when this scene load
    /// </summary>
    private void InitAvatarColor()
    {
        foreach (Player player in _playerList)
        {
            Color color = player.Role.Color; // like Color.green, Color.yellow...
            string avatar = player.Role.Avatar; // like "Heart", "Spade"...
            Debug.Log($"set _markDict: {Common.ToColorName(color)}, {avatar}");
            ReflectOnScreen(avatar, color); 
        }
        // did not be used
        // Club 
        ReflectOnScreen("Club", Color.gray);
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateLyricsDisplay()
    {
        // �^�񒆂̍s���X�V���邽�߂̃C���f�b�N�X
        int middleLineIndex = 1;
        for (int i = 0; i < _textField.Length; i++)
        {
            // �\������̎��s������i�O��1�s + ���ݍs�j
            int lyricIndex = _currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < _lyricsLines.Count)
            {
                // �e�L�X�g��F�t���ō\�z
                string coloredText = "";
                foreach (Part part in _lyricsLines[lyricIndex].PartList)
                {
                    Role role = part.Player.Role;
                    string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
                    coloredText += $"<color=#{hexColor}>{Common.AvatarToLetter(role.Avatar)}{part.Word}</color> ";
                }

                _textField[i].text = coloredText.Trim();

                // handle the level of transparent
                // �^�񒆂̍s�͕s�����A����ȊO�͔�����
                if (i == middleLineIndex)
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 1f); // �s���� opaque
                }
                else
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 0.2f); // ������ translucent
                }
            }
            else
            {
                // �̎����Ȃ��ꍇ�͋󔒂ɐݒ�
                _textField[i].text = "";
            }
        }
    }

    /// <summary>
    /// Set color to object place on the screen
    /// </summary>
    /// <param name="avatarName"></param>
    /// <param name="assignedColor"></param>
    void ReflectOnScreen(string avatarName, Color assignedColor)
    {
        // Canvas���� objName �I�u�W�F�N�g��T��
        GameObject obj = GameObject.Find(avatarName);

        // �I�u�W�F�N�g�����������ꍇ
        if (obj != null)
        {
            // Image�R���|�[�l���g���擾
            Image objImage = obj.GetComponent<Image>();

            if (objImage != null)
            {
                // Image��Color�v���p�e�B�ɐF��ݒ�
                objImage.color = assignedColor;
                Debug.Log($"Color {assignedColor} applied to {avatarName}.");
            }
            else
            {
                Debug.LogError($"{avatarName} does not have an Image component.");
            }
        }
        else
        {
            Debug.LogError($"{avatarName} object not found in the scene.");
        }
    }


}
