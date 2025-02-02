using System;
using File = System.IO.File;
using Path = System.IO.Path;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// Save Game Data
/// </summary>
public class GameData : MonoBehaviour
{
    // �ʂ̃X�N���v�g����A�N�Z�X�ł���悤�� static �ɂ���
    private static string _songTitle = "";
    private static int _playerCount = 0;

    public static string SongTitle { get => _songTitle; set => _songTitle = value; }
    public static int PlayerCount { get => _playerCount; set => _playerCount = value; }

    void Start()
    {
        LoadGameInfo();
    }

    /// <summary>
    /// Load game meta data from file "GameInfo.txt"
    /// </summary>
    public static void LoadGameInfo()
    {
        string filePath = Path.Combine(Application.dataPath, FileName.MetaData);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"PartLog file not found: {filePath}");
        }

        string[] lineList = File.ReadAllLines(filePath);

        // Get song title
        _songTitle = lineList[0];

        // Get player count
        if (Int32.TryParse(lineList[1], out _playerCount))
        {
            Console.WriteLine(_playerCount);
        }
        else
        {
            Console.WriteLine($"Int32.TryParse could not parse num of player '{lineList[1]}' to an int.");
        }
    }

    /// <summary>
    /// get song title without SPACE (string)
    /// </summary>
    /// <returns></returns>
    public static string GetSongTitle()
    {
        LoadGameInfo();
        string noSpace = _songTitle.Replace(" ", ""); // �󔒂����ׂč폜
        return noSpace;
    }
    
    /// <summary>
    /// get number of players (int)
    /// </summary>
    /// <returns></returns>
    public static int GetPlayerCount()
    {
        LoadGameInfo();
        return _playerCount;
    }

}

