using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Show sample assignment(game data: registered in Home screen)
public class ShowSample : MonoBehaviour
{
    private Data _data = new Data();

    // Start is called before the first frame update
    void Start()
    {
        // assign role to players and save the data
        Assignment assignment = new Assignment();
        assignment.DoAssignment();

        // load lyrics division data after lyrics divided into parts
        LoadData();

        // part division
        //SetLyricsTiming division = new SetLyricsTiming();
        //List<Line> lyrics = division.DoDivisionWithTiming();
        Division division = new Division();
        List<Part> lyrics = division.DivideLyrics(_data.Song.Title);

        //List<Line> lyrics = _data.Song.Lyrics;

        // Display the result (not in scroll, whole lyrics will appear at the same time)
        DisplayWholeLyrics(lyrics);

        // ボタンが押されたらこれを実行 Switch Scene
        GameObject.Find("GoHome").GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    /// <summary>
    /// If button clicked, switch scene
    /// </summary>
    void ButtonClicked()
    {
        // ゲーム画面へ GO
        SceneManager.LoadScene("Home");
    }

    /// <summary>
    /// Display the result (not in scroll, whole lyrics will appear at the same time)
    /// </summary>
    /// <param name="lyrics"></param>
    private void DisplayWholeLyrics(List<Part> lyrics)
    {
        // A line of colored lyrics
        string wholeLyrics = _data.Song.Title + "\n";

        // Create colored text
        foreach (Part part in lyrics)
        {
            string text = GetColoredPartText(part);
            wholeLyrics += text;
        }

        // Display on the screen 
        //TextMeshProUGUI textField = GameObject.Find("Lyrics").GetComponent<TextMeshProUGUI>();
        Text textField = GameObject.Find("Line").GetComponent<Text>();
        textField.text = wholeLyrics.Trim();
    }

    /// <summary>
    /// Display the result (not in scroll, whole lyrics will appear at the same time)
    /// </summary>
    /// <param name="lyrics"></param>
    private void DisplayWholeLyrics(List<Line> lyrics)
    {
        // A line of colored lyrics
        string wholeLyrics = _data.Song.Title;

        // Create colored text
        foreach (Line line in lyrics)
        {
            string lineText = GetColoredLine(line);
            wholeLyrics += "\n" + lineText;
        }

        // Display on the screen
        TextMeshProUGUI textField = GameObject.Find("Lyrics").GetComponent<TextMeshProUGUI>();
        textField.text = wholeLyrics.Trim();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static string GetColoredLine(Line line)
    {
        string lyricsText = "";
        foreach (Part part in line.PartList)
        {
            Role role = part.Player.Role;
            string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
            lyricsText += $"<color=#{hexColor}>{Common.AvatarToLetter(role.Avatar)}{part.Text}</color> ";
        }

        return lyricsText;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    private static string GetColoredPartText(Part part)
    {
        string lyricsText = "";
        Role role = part.Player.Role;
        string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
        lyricsText += $"<color=#{hexColor}>{Common.AvatarToLetter(role.Avatar)}{part.Text}</color> ";

        return lyricsText;
    }

    /// <summary>
    /// load lyrics division data after lyrics divided
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
    }

}
