using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DisplayLyrics : MonoBehaviour
{
    // ����: _textField �������O�ύX����Ȃ� Inspector �̂Ƃ���� TextMeshPro �݂����Ȃ� Row1,2,3 ����Ȃ�����
    public TextMeshProUGUI[] _textField; // �̎���\������TextMeshProUGUI�I�u�W�F�N�g�i3�s���j
    private int _currentLyricIndex = 0; // ���݂̉̎��C���f�b�N�X
    private float _loadingTime = 0.8f; // time lag
    private float _lineStartTime = 0f; // intro �O�t�I������ = �̎��\��(�̃X�N���[���p�����v�Z)�J�n����
    // Load _lyricsList 
    private List<Line> _lyricsList = new List<Line>(); // �̎����(�\���J�n�����{�\������̎�)���i�[���郊�X�g
    // Load _playerList 
    private List<Player> _playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        // get division data
        LoadAndSetData();

        InitAvatarColor(); // ��ʏ�ɃA�o�^�[�̐F�𔽉f����

        UpdateLyricsDisplay(); // �����\�����X�V
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadAndSetData()
    {
        _lyricsList = (List<Line>)Common.LoadXml(_lyricsList.GetType(), FileName.XmlLyricsDivision);
        _playerList = (List<Player>)Common.LoadXml(_playerList.GetType(), FileName.XmlPlayerRole);
    }

    // Update is called once per frame
    void Update()
    {
        // ���݂̎����Ɋ�Â��ĉ̎����X�V
        float currentTime = Time.timeSinceLevelLoad;

        /* Display lyrics */
        int maxIndex = _lyricsList.Count - 1;
        Line nextLine = _lyricsList[_currentLyricIndex + 1];
        // ���̉̎��s�ɐi�ނׂ��^�C�~���O���m�F
        if (_currentLyricIndex < maxIndex && currentTime >= nextLine.timing - _loadingTime)
        {
            _currentLyricIndex++;
            UpdateLyricsDisplay();
        }

        /* Update Avatar Color */
        // Avatar �̃p�[�g�̂Ƃ��Ɍ���ȂǕω�����
    }

    /// <summary>
    /// Update screen: set color to avatar when this scene load
    /// </summary>
    private void InitAvatarColor()
    {
        foreach (Player player in _playerList)
        {
            Color color = player.color; // like Color.green, Color.yellow...
            string avatar = player.avatar; // like "Heart", "Spade"...
            Debug.Log($"from _markDict: {Common.ToColorName(color)}, {avatar}");
            ReflectOnScreen(avatar, color); 
        }
        // did not be used
        // Club 
        ReflectOnScreen("Club", Color.gray);
    }

    private void UpdateLyricsDisplay()
    {
        // �^�񒆂̍s���X�V���邽�߂̃C���f�b�N�X
        int middleLineIndex = 1;

        for (int i = 0; i < _textField.Length; i++)
        {
            // �\������̎��s������i�O��1�s + ���ݍs�j
            int lyricIndex = _currentLyricIndex + i - middleLineIndex;

            if (lyricIndex >= 0 && lyricIndex < _lyricsList.Count)
            {
                // �e�L�X�g��F�t���ō\�z
                string coloredText = "";
                foreach (Part part in _lyricsList[lyricIndex].partList)
                {
                    Player player = part.player;
                    string hexColor = ColorUtility.ToHtmlStringRGB(player.color);
                    coloredText += $"<color=#{hexColor}>{player.avatar}{part.word}</color> ";
                }

                _textField[i].text = coloredText.Trim();

                // �^�񒆂̍s�͕s�����A����ȊO�͔�����
                if (i == middleLineIndex)
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 1f); // �s����
                }
                else
                {
                    _textField[i].color = new Color(1f, 1f, 1f, 0.2f); // ������
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
