using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assign role to players and Save(Update) data
/// </summary>
public class Assignment 
{
    // access to game data
    Data _data = new Data();
    // 
    private List<string> _micList = new List<string>(); // 検出されたマイク名
    // 候補となるアバター(今は順番に割り当て) // ***Update*** random 割り当てしなきゃ
    private List<string> _avatarList = new List<string> { "Heart", "Spade", "Diamond", "Club" }; // Heart, Spade, Diamond, Club の順で固定
    // to access game data
    private int _playerCount = 0;
    private List<Player> _playerList = new List<Player>(); // 割り当て情報

    /// <summary>
    /// assign role to players and save the data
    /// </summary>
    public void DoAssignment()
    {
        // Get and Set game data registered in previous page from file
        LoadData();

        // color assignment 
        AssignRoleToPlayers();

        // Save Player role (name, color, avatar(mark),mic)
        SaveDataToFile();
    }

    /// <summary>
    /// Set game data to parameter used in this class
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        _playerCount = _data.Team.CountMembers;
        _playerList = _data.Team.MemberList; // already include players' name
    }

    /// <summary>
    /// Save data to file
    /// </summary>
    private void SaveDataToFile()
    {
        _data.Team.MemberList = _playerList; // player role added
        Common.ExportToXml(_data, FileName.XmlGameData); // update player role
    }

    /// <summary>
    /// Set Players' role (MIC to use, AVATAR, COLOR displayed on the game screen)
    /// </summary>
    private void AssignRoleToPlayers()
    {
        // detect mic devices connected to PC and set them to _micList 
        SetMicList();

        /* Random Color Assignment */
        // 候補となる色の中から mic(player) の数だけ色を選ぶ
        List<Color> colors = RandomSelectColor(Common.AvailableColors, _playerCount);

        // For debug
        Debug.Log($"Selected colors:\n");
        foreach (Color color in colors)
        {
            Debug.Log($"{Common.ToColorName(color)}");
        }

        // Set player role
        for (int i = 0; i < _playerCount; i++)
        {
            Role role = new Role() { 
                Color = colors[i],
                Avatar = _avatarList[i],
                Mic = _micList[i]
            };
            // if REAL player exist
            if (!_micList[i].Contains("Robot"))
            {
                role.IsRobot = false;
            }

            // add to player list
            _playerList[i].Role = role;

            // for debug
            Debug.Log($"Assignment: {_playerList[i].Name}, {Common.ToColorName(colors[i])}, {_micList[i]}, {_avatarList[i]}");
        }
    }

    /// <summary>
    /// Set _micList by detecting mic devices
    /// </summary>
    private void SetMicList()
    {
        // the CASE that NO mic detected (without default mic of PC)
        if (Microphone.devices.Length == 1)
        {
            // おそらくパソコン本体のマイク
            Debug.Log($"No USB mic detected.\n{Microphone.devices[0]} is detected.");

            for (int i = 0; i < _playerCount; i++)
            {
                string micName = "Mic" + (i + 1).ToString() + "(Robot part)";
                _micList.Add(micName);
            }

            // for debug
            Debug.Log($"No microphones detected.\n{_micList.Count} content of _micList :\n");
            foreach (string name in _micList) Debug.Log($"{name}");

            return;
        }

        // the CASE that mic detected
        IfMicDetected();
    }

    /// <summary>
    /// If there are more than one of the mics connected to APP
    /// </summary>
    private void IfMicDetected()
    {
        // Get mic devices that connected to PC with USB
        foreach (string deviceName in Microphone.devices)
        {
            // PC 本体のマイクは含めない 
            if (deviceName == "マイク配列 (Realtek(R) Audio)") continue;

            // USB で接続されたマイクのみ追加
            _micList.Add(deviceName);
        }
        Debug.Log($"Detected {_micList.Count} microphones.");

        // If (the number of MIC devices < the number of PLAYERs registered in the previous page)
        // Set "Robot part" for the number of the distinction between these two
        // e.g. playerCount = 3, 2 mic detected --> There is one robot part exist
        while (_micList.Count < _playerCount)
        {
            string micName = "Mic" + (_micList.Count + 1).ToString() + "(Robot part)";
            _micList.Add(micName);
        }
        Debug.Log($"There are {_playerCount} players and {_micList.Count} mics are assigned to each player.");
    }

    /// <summary>
    /// Selecting X colors from the input List of colors (x = count, the 2nd parameter)
    /// </summary>
    /// <param name="colorsForSelect"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private List<Color> RandomSelectColor(List<Color> availableColors, int count)
    {
        // to collect Selected Colors 結果を格納するリスト
        List<Color> result = new List<Color>();

        // 選択する色数 count が候補色の要素数 availableColors.Count を超えないようにする
        count = Mathf.Min(count, availableColors.Count);

        // Select X colors (X = count)
        for (int i = 0; i < count; i++)
        {
            // while 文使う場合の終了条件
            //if (selectedColors.Count == count) break; // count 個選択できたら任務完了

            // ランダムに 1 つ選択
            int index = Random.Range(0, availableColors.Count);
            result.Add(availableColors[index]);

            // 選択済みの色を候補色リストから削除（重複を避ける）
            availableColors.RemoveAt(index);
        }

        return result;
    }

}
