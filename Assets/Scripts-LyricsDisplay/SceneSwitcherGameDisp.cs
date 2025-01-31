using UnityEngine;
using UnityEngine.UI; // UI�v�f�ɃA�N�Z�X���邽�߂ɕK�v
using UnityEngine.SceneManagement; // �V�[���؂�ւ��̂��߂ɕK�v

public class SceneSwitcherGameDisp : MonoBehaviour
{
    void Start()
    {
        // DisplayLyrics �V�[���ɂ���{�^���̑���
        GameObject.Find("ButtonHome").GetComponent<Button>().onClick.AddListener(ClickButtonEndGame);
        GameObject.Find("ButtonScore").GetComponent<Button>().onClick.AddListener(ClickButtonViewScore);
    }
    void ClickButtonEndGame()
    {
        // �{�^���N���b�N���ɌĂяo�����
        string sceneName = "Home";
        Debug.Log(sceneName);
        SwitchScene(sceneName);
    }
    void ClickButtonViewScore()
    {
        // �{�^���N���b�N���ɌĂяo�����
        string sceneName = "Score";
        Debug.Log(sceneName);
        SwitchScene(sceneName);
    }

    void SwitchScene(string sceneName)
    {
        // sceneName �Ƃ������O�̃V�[�������[�h
        SceneManager.LoadScene(sceneName);
    }
}
