using System.Collections.Generic;
using TMPro;
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

        // part division
        Division division = new Division();
        division.DoDivision();

        // load lyrics division data after lyrics divided into parts
        LoadData();
        List<Line> lyrics = _data.Song.Lyrics;

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
        string lineText = "";
        foreach (Part part in line.PartList)
        {
            Role role = part.Player.Role;
            string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
            lineText += $"<color=#{hexColor}>{Common.AvatarToLetter(role.Avatar)}{part.Word}</color> ";
        }

        return lineText;
    }

    /// <summary>
    /// load lyrics division data after lyrics divided
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
