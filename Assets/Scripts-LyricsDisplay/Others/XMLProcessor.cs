using System.IO;
using System.Xml;
using UnityEngine;
public class XMLProcessor : MonoBehaviour
{
    public string inputFileName = "Lyrics"; // Resources�t�H���_����XML�t�@�C�����i�g���q�s�v�j
    public string outputFileName = "Orders.xml"; // �o�͂���XML�t�@�C����
    void Start()
    {
        // Resources�t�H���_������̓t�@�C�������[�h
        TextAsset xmlAsset = Resources.Load<TextAsset>(inputFileName);
        if (xmlAsset == null)
        {
            Debug.LogError($"Input file not found in Resources: {inputFileName}.xml");
            return;
        }
        // �o�̓p�X��`Assets/`�z���ɐݒ�
        string outputFilePath = Path.Combine(Application.dataPath, outputFileName);
        try
        {
            // ���̓f�[�^���������A�o�̓t�@�C���𐶐�
            using (StringReader stringReader = new StringReader(xmlAsset.text))
            using (XmlReader reader = XmlReader.Create(stringReader))
            using (XmlWriter writer = XmlWriter.Create(outputFilePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Lyrics");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Line")
                    {
                        string originalLine = reader.ReadElementContentAsString();
                        string processedLine = RandomWordSplit(originalLine); // �P��P�ʂŏ���
                        writer.WriteElementString("Line", processedLine);
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                Debug.Log($"Processed XML saved to {outputFilePath}");
            }
#if UNITY_EDITOR
            // Unity�G�f�B�^��Assets�t�H���_���X�V
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing XML: {ex.Message}");
        }
    }
    // �����_���ɐ��P�ꂲ�ƂɃJ���}��}��
    private string RandomWordSplit(string line)
    {
        // �P��P�ʂŕ���
        string[] words = line.Split(' ');
        string result = "";
        int wordCount = 0;
        for (int i = 0; i < words.Length; i++)
        {
            result += words[i].Trim();
            wordCount++;
            // �����_���ɋ�؂蕶����ǉ��i1����2�P��P�ʂŕ����j
            if (wordCount >= UnityEngine.Random.Range(1, 3) && i < words.Length - 1)
            {
                result += ",";
                wordCount = 0;
            }
            else if (i < words.Length - 1)
            {
                result += " ";
            }
        }
        return result;
    }
}