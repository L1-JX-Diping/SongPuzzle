using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneSwitcher : MonoBehaviour
{
    // �C���X�y�N�^�[�Őݒ�\�ȃV�[����
    public string sceneName = "DisplayLyrics";

    // �{�^���N���b�N���ɌĂяo�����֐�
    public void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set in the inspector.");
        }
    }
}
