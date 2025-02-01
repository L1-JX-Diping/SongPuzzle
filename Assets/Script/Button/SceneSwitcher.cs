using UnityEngine;
using UnityEngine.UI; // UI�v�f�ɃA�N�Z�X���邽�߂ɕK�v
using UnityEngine.SceneManagement; // �V�[���؂�ւ��̂��߂ɕK�v

public class SceneSwitcher : MonoBehaviour
{
    void Start()
    {
        // AssignColor �V�[���ɂ���{�^���̑���
        GameObject.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(ClickButtonStartGame);
        GameObject.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(ClickButtonBackToHome);
    }
    void ClickButtonStartGame()
    {
        // �{�^���N���b�N���ɌĂяo�����
        string sceneName = "DisplayLyrics";
        SwitchScene(sceneName);
    }
    void ClickButtonBackToHome()
    {
        // �{�^���N���b�N���ɌĂяo�����
        string sceneName = "Home";
        SwitchScene(sceneName);
    }

    void SwitchScene(string sceneName)
    {
        // sceneName �Ƃ������O�̃V�[�������[�h
        SceneManager.LoadScene(sceneName);
    }
}
