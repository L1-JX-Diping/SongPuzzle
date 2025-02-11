using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Division 
{
    // divided lyrics
    List<Part> _lyrics = new List<Part>();
    int repeatAllowed = 3;

    /// <summary>
    /// Get divided lyrics (for accessing from other classes)
    /// </summary>
    /// <param name="songTitle"></param>
    /// <returns></returns>
    public List<Part> DivideLyrics(string songTitle)
    {
        // load whole lyrics from file
        string lyricsText = GetWholeLyrics(songTitle);

        // divide lyrics randomly
        RandomLyricsDivision(lyricsText, 3, 8);

        // assign role randomly
        RandomRoleAssignment();

        return _lyrics;
    }

    /// <summary>
    /// Get lyrics and set as string
    /// </summary>
    /// <param name="songTitle"></param>
    /// <returns></returns>
    private string GetWholeLyrics(string songTitle)
    {
        string songName = songTitle.Replace(" ", "");
        string fileName = songName + ".txt";

        // Load whole lyrics line by line
        string[] lyricsLineList = Common.GetTXTFileContents(fileName);
        string lyricsWholeText = "";

        foreach (string line in lyricsLineList)
        {
            lyricsWholeText += line + "\n";
        }

        return lyricsWholeText;
    }

    /// <summary>
    /// Divide lyrics randomly and Set to _lyrics
    /// </summary>
    /// <param name="lyricsText"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    private void RandomLyricsDivision(string lyricsText, int min, int max)
    {
        // letter of each Part > 0: player has to sing, each part must have more than one letter
        if (min <= 0) { min = 1; }

        string dividedText = "";
        int count = 0;

        // lyricsText.Length: the number of Char Object in string text
        for (int charIndex = 0; charIndex < lyricsText.Length; )
        {
            // count of letter
            count = Random.Range(min, max);

            // for no error such as "out of range"
            //if (count > lyricsText.Length - charIndex)
            //{
            //    count = lyricsText.Length - charIndex;
            //}

            // clear dividedText 
            dividedText = "";

            // divide lyrics
            for (int x = 0; x < count; charIndex++, x++)
            {
                if (charIndex > lyricsText.Length - 1) break;
                dividedText += lyricsText[charIndex];
            }
            // debug
            Debug.Log($"Lyrics divided : {dividedText}, at index {charIndex} of whole lyrics");

            // treat the segmentation as a music part
            Part part = new Part
            {
                Text = dividedText
            };

            // Add to _lyrics
            _lyrics.Add(part);
        }
    }

    /// <summary>
    /// Update _lyrics by adding role assignment information
    /// </summary>
    private void RandomRoleAssignment()
    {
        // get game data
        Data data = new Data();
        data = (Data)Common.LoadXml(data.GetType(), FileName.XmlGameData);

        // the number of players
        int playerCount = data.Team.CountMembers;

        // order of player No 
        List<int> roleOrder = Common.GetRandomRoleOrder(playerCount, repeatAllowed);

        // Assign role to each Part following order[]
        int index = 0;
        foreach (Part part in _lyrics)
        {
            // if index out of range --> order ‚ð‚à‚¤ˆê“xŽn‚ß‚©‚ç‰ñ‚·
            if (index > roleOrder.Count - 1) { index = 0; }

            // get player(role) information 
            int playerNo = roleOrder[index];
            Player player = data.Team.MemberList[playerNo];

            // add player(role) information to each Part in _lyrics
            part.Player = player;

            // update index
            index++;
        }
    }

}
