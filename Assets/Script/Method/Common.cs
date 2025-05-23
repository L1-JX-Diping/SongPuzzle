﻿using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Threading;

public class Common 
{
    // 候補色
    private static List<Color> _availableColors = new List<Color> { Color.green, Color.red, Color.blue, Color.yellow, Color.magenta, Color.cyan };

    public static List<Color> AvailableColors { get => _availableColors; set => _availableColors = value; }

    /// <summary>
    /// Get random index list (player count, allowed number of continuous assign)
    /// </summary>
    /// <param name="roleCount"></param>
    /// <param name="repeatAllowed"></param>
    /// <returns></returns>
    public static List<int> GetRandomRoleOrder(int roleCount, int repeatAllowed)
    {
        List<int> result = new List<int>();
        // candidate 候補
        List<int> candidateNums = GenerateCandidateList(roleCount);
        int listLen = 20; // 作成するリストの長さ for parts(lyrics) of this line 

        while (result.Count < listLen)
        {
            int roleNo = Random.Range(0, candidateNums.Count);
            int selectedNum = candidateNums[roleNo];

            // How many times same player repeated already? 
            if (result.Count >= repeatAllowed)
            {
                int count = 0;
                // Check: 1 つ前, 2 つ前... と辿って比較
                for (int i = 1; i <= repeatAllowed; i++)
                {
                    if (result[result.Count - i] == selectedNum) { count += 1; }
                }
                // Reselect
                if (count == repeatAllowed) continue;
            }

            // maxRepeats 回以上連続同じ人にパート割り当てがない場合 Add
            result.Add(selectedNum);
        }

        return result;
    }

    /// <summary>
    /// 0 から (maxNum - 1) までの整数のリスト: [0, 1, 2,..., maxNum-1]
    /// </summary>
    /// <param name="maxNum"></param>
    /// <returns></returns>
    public static List<int> GenerateCandidateList(int maxNum)
    {
        List<int> candidateList = new List<int>();

        for (int i = 0; i < maxNum; i++)
        {
            candidateList.Add(i);
        }

        return candidateList;
    }

    /// <summary>
    /// Save information to XML file
    /// [parameter1] object that has the information you wanna save
    /// [parameter2] export to where? (file name)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public static void ExportToXml(object obj, string fileName)
    {
        // dont need to make sure that file path exists
        // if not exist, system will make new file automatically
        string path = Path.Combine(Application.dataPath, fileName);

        try
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            //// if using XmlWriter
            //// Set Encording 日本語文字化けしないように
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false) // ★ BOMなしのUTF-8を指定
            };
            // Write to xml with utf-8 
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                serializer.Serialize(writer, obj);
            }

            // if using StreamWriter
            // Write to xml with utf-8 
            //using (StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(false)))
            //{
            //    serializer.Serialize(writer, obj);
            //}

            Debug.Log($"Lyrics data saved to XML {fileName}: {path}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to save XML {fileName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Load information and RETURN object has that information. 
    /// [parameter1] expected object type to be returned 
    /// [parameter2] read from where? (file name)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static object LoadXml(Type type, string fileName)
    {
        object result = null;
        // make sure that file path exists
        string path = GetFilePath(fileName);
        if (path == null) return null;

        try
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (StreamReader reader = new StreamReader(path))
            {
                result = serializer.Deserialize(reader);
            }

            Debug.Log($"Player roles loaded from XML {fileName}: {path}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to load XML {fileName}: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Get file path which is definitely exist 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFilePath(string fileName)
    {
        // Get the file path
        string filePath = Path.Combine(Application.dataPath, fileName);

        // check if the file path exist or not
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File {fileName} not found: {filePath}");
            return null;
        }

        return filePath;
    }

    /// <summary>
    /// Read file and Return string[] which collect file content by line
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string[] GetTXTFileContents(string fileName)
    {
        string filePath = GetFilePath(fileName);
        string[] lineList = null;

        // read line by line from  the file
        lineList = File.ReadAllLines(filePath, Encoding.UTF8);
        return lineList;
    }

    /// <summary>
    /// convert String To Int
    /// </summary>
    /// <param name="numLetter"></param>
    /// <returns></returns>
    public static int ToInt(string numLetter)
    {
        int num = 0;

        if (Int32.TryParse(numLetter, out num))
        {
            return num;
        }
        else
        {
            Debug.Log($"Int32.TryParse could not parse string {numLetter} to an int.");
        }

        return 0;
    }

    /// <summary>
    /// color name to COLOR (return type: Color)
    /// </summary>
    /// <param name="colorName"></param>
    /// <returns></returns>
    public static Color ToColor(string colorName)
    {
        if (colorName == "RED") return Color.red;
        if (colorName == "GREEN") return Color.green;
        if (colorName == "YELLOW") return Color.yellow;
        if (colorName == "BLUE") return Color.blue;
        if (colorName == "MAGENTA") return Color.magenta;
        if (colorName == "CYAN") return Color.cyan;

        return Color.white; // all players have to sing
    }

    /// <summary>
    /// color to color NAME 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ToColorName(Color color)
    {
        if (color == Color.red) return "RED";
        if (color == Color.green) return "GREEN";
        if (color == Color.yellow) return "YELLOW";
        if (color == Color.blue) return "BLUE";
        if (color == Color.magenta) return "MAGENTA";
        if (color == Color.cyan) return "CYAN";

        return "ALL"; // all players have to sing
    }


    /// <summary>
    /// AVATAR name to MARK letter
    /// </summary>
    /// <param name="avatar"></param>
    /// <returns></returns>
    public static string AvatarToLetter(string avatar)
    {
        if (avatar == "Heart") return "♥";
        if (avatar == "Spade") return "♠";
        if (avatar == "Diamond") return "♦";
        if (avatar == "Club") return "♣";

        return "*"; // all players have to sing
    }

    /// <summary>
    /// Generate numList from num(int)
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static List<int> GenerateNumberList(int num)
    {
        List<int> numList = new List<int>();

        for (int i = 0; i < num; i++)
        {
            numList.Add(i);
        }

        return numList;
    }

    /// <summary>
    /// アプリケーションを終了する処理
    /// </summary>
    public static void QuitApp()
    {
        Debug.Log("APP QUIT: アプリケーション終了処理を実行");

#if UNITY_EDITOR
        // エディタ上で実行を停止する（エディタ用）
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 実行ファイルの場合はアプリケーションを終了
        Application.Quit();
#endif
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

}
