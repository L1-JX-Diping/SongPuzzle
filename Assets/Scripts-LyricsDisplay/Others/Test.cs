using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI textComponent;  // Hierarchy�ō쐬����TextMeshProUGUI���A�^�b�`
    public string inputFileName = "Orders.xml";  // �ǂݍ���XML�t�@�C�����iAssets/�z���j
    public string logFileName = "Assets/ColorLog.txt"; // �F�����L�^����t�@�C��
    private string[] lines;  // �ǂݍ��񂾉̎���ێ�
    private int currentLineIndex = 0;  // ���݂̍s��ǐ�
    private Color[] colors = { Color.red, Color.green, Color.blue }; // �g�p����3�F

    void Start()
    {
        // Orders.xml��ǂݍ���
        LoadLyrics();
        // �F���O�t�@�C����������
        if (File.Exists(logFileName)) File.Delete(logFileName);
        File.WriteAllText(logFileName, "Color Log:\n");

        // �R���[�`�����J�n����2�b���Ƃɕ\��
        StartCoroutine(DisplayLyricsCoroutine());
    }

    void LoadLyrics()
    {
        string path = Path.Combine(Application.dataPath, inputFileName);
        if (File.Exists(path))
        {
            // XML�t�@�C�������[�h
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNodeList lineNodes = xmlDoc.SelectNodes("/Lyrics/Line");
            lines = new string[lineNodes.Count];
            for (int i = 0; i < lineNodes.Count; i++)
            {
                lines[i] = lineNodes[i].InnerText.Trim();
            }
        }
        else
        {
            Debug.LogError($"File not found: {path}");
            lines = new string[0];
        }
    }

    IEnumerator DisplayLyricsCoroutine()
    {
        while (currentLineIndex < lines.Length)
        {
            // ���݂̍s���擾
            string line = lines[currentLineIndex];
            currentLineIndex++;

            // �R���}�ŋ�؂�A�e�����Ƀ����_���ȐF��K�p
            string[] parts = line.Split(',');
            string formattedText = "";  // �ŏI�I�ɕ\������e�L�X�g
            string logText = $"Line {currentLineIndex}:\n"; // ���O�p

            foreach (string part in parts)
            {
                // �����_���ȐF��I��
                Color randomColor = colors[Random.Range(0, colors.Length)];
                textComponent.color = randomColor;

                // �e�L�X�g��\��
                textComponent.text = part.Trim();
                formattedText += part.Trim() + " ";

                // ���O�ɒǉ�
                logText += $"  \"{part.Trim()}\" - {randomColor}\n";

                // �����ҋ@���Ď��̕�����\��
                yield return new WaitForSeconds(0.5f);
            }

            // �S�̂̍s���ŏI�\��
            textComponent.text = formattedText.Trim();

            // ���O���t�@�C���ɕۑ�
            File.AppendAllText(logFileName, logText + "\n");

            // �ҋ@����b���̎w��
            // 2�b�ҋ@���Ď��̍s�ɐi��
            yield return new WaitForSeconds(2f);
        }
    }
}
