using UnityEngine;
using TMPro;  // TextMeshPro���g�p���邽�߂ɕK�v

public class DisplayTextUI : MonoBehaviour
{
    public TextMeshProUGUI textComponent;  // Hierarchy�ō쐬����TextMeshProUGUI���A�^�b�`

    void Start()
    {
        // �e�L�X�g��ݒ�
        textComponent.text = "Hello, Unity!";
        textComponent.color = Color.red;
    }

    void Update()
    {
        // �V�[���J�n����̌o�ߎ��Ԃ��擾
        float elapsedTime = Time.time;

        // ���Ԃɉ����ăe�L�X�g���X�V
        if (elapsedTime > 2)
        {
            textComponent.color = Color.blue;
            textComponent.text = elapsedTime.ToString();
        }
        // ���Ԃɉ����ăe�L�X�g���X�V
        // textComponent.text = "Time: " + Time.time.ToString("F2");
    }
    // �e�L�X�g���e��ύX����֐�
    public void UpdateLyricsText(string newText)
    {
        if (textComponent != null)
        {
            textComponent.text = newText;
        }
    }

    // �e�L�X�g�F��ύX����֐�
    public void UpdateTextColor(Color newColor)
    {
        if (textComponent != null)
        {
            textComponent.color = newColor;
        }
    }
}
