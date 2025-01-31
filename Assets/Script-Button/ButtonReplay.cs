using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI �����̂�
using UnityEngine.SceneManagement; // Scene �̐؂�ւ��������ꍇ�ɕK�v�Ȑ錾

public class ButtonReplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // �{�^���������ꂽ�炱������s
        this.GetComponent<Button>().onClick.AddListener(SwitchScene);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwitchScene()
    {
        // �V�[�� DisplayLyrics ���J��
        SceneManager.LoadScene("DisplayLyrics");
    }
}
