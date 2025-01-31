using UnityEngine;
using TMPro;  // TextMeshPro���g�����߂ɕK�v
public class LyricsColorController : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
    public TextMeshProUGUI textComponent;  // �V�[���ɂ���TextMeshProUGUI���A�^�b�`
    public Color colorToSet = Color.red;   // �C�ӂ̐F��ݒ�ł���

    void Start()
    {
        // textComponent���ݒ肳��Ă��邩�`�F�b�N
        if (textComponent != null)
        {
            textComponent.color = colorToSet;  // �F��ύX
        }
        else
        {
            Debug.LogWarning("Text Component���ݒ肳��Ă��܂���");
        }
    }

    // �F��ύX����֐���ǉ�
    public void ChangeTextColor(Color newColor)
    {
        if (textComponent != null)
        {
            textComponent.color = newColor;
        }
    }
}
