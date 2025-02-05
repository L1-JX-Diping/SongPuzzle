using System;
using File = System.IO.File;
using Path = System.IO.Path;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections.Generic;

/// <summary>
/// Save Game Data
/// </summary>
public class Data 
{
    // game meta data
    private static string _songTitle = "";
    private static int _playerCount = 0;
    private static List<string> _playerList = new List<string>();

    // selected song parameter
    private static float _clock = 3f; // Second per Beat
    private static float _beat = 4; // ‰½”q‚©H Birthday song ‚Í 3 ”q

    /// <summary>
    /// constructor: will be called at the first
    /// </summary>
    public Data() 
    {
        SetAllDataFromTXT();
    }

    // For Accessing game data from other class
    // but it doesn't work...

    /// <summary>
    /// Get song title without SPACE (string)
    /// </summary>
    public static string SongTitle { get => _songTitle; set => _songTitle = value; }

    /// <summary>
    /// Get player count (int)
    /// </summary>
    public static int PlayerCount { get => _playerCount; set => _playerCount = value; }

    /// <summary>
    /// Get a list of player name (List<string>)
    /// Player1, Player2, Player3...
    /// </summary>
    public static List<string> PlayerList { get => _playerList; set => _playerList = value; }

    void Start()
    {
        //SetAllData();
    }

    /// <summary>
    /// You should call this function at the first
    /// Load game meta data from file "GameInfo.txt"
    /// </summary>
    public static void SetAllDataFromTXT()
    {
        string[] lineList = Common.GetTXTFileLineList(FileName.MetaData);

        // Get song title without SPACE
        string noSpace = lineList[0].Replace(" ", ""); // ‹ó”’‚ğ‚·‚×‚Äíœ
        _songTitle = noSpace;

        // Get player count
        _playerCount = Common.ToInt(lineList[1]);

        // Get players' name (Player1, Player2, ...)
        string[] playerNameList = lineList[2].Split(", ");
        foreach (string playerName in playerNameList)
        {
            _playerList.Add(playerName);
        }

    }
    
    //public static List<string> GetPlayerList()
    //{
    //    //SetPlayerList();
    //    return _playerList;
    //}

    ///// <summary>
    ///// get song title without SPACE (string)
    ///// </summary>
    ///// <returns></returns>
    //public static string GetSongTitle()
    //{
    //    //SetGameDataFromFile();
    //    string noSpace = _songTitle.Replace(" ", ""); // ‹ó”’‚ğ‚·‚×‚Äíœ
    //    return noSpace;
    //}

    ///// <summary>
    ///// get number of players (int)
    ///// </summary>
    ///// <returns></returns>
    //public static int GetPlayerCount()
    //{
    //    //SetGameDataFromFile();
    //    return _playerCount;
    //}

}

