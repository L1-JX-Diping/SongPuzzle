using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{ 
    public static string AvatarToPartMark()
    {

        return "*";
    }

    /// <summary>
    /// color name to COLOR (return Color)
    /// </summary>
    /// <param name="colorName"></param>
    /// <returns></returns>
    public static Color NameToColor(string colorName)
    {
        if (colorName == "RED") return Color.red;
        if (colorName == "GREEN") return Color.green;
        if (colorName == "YELLOW") return Color.yellow;

        return Color.white;
    }

    string MarkToChar(string markName)
    {
        if (markName == "Heart") return "*";
        if (markName == "Spade") return "!";
        if (markName == "Diamond") return "+";
        if (markName == "Club") return "#";

        string chorusMark = "<<";
        //string chorusMark = "";
        //int i = 0;
        //while (i < _playerCount)
        //{
        //    chorusMark += "<";
        //    i++;
        //}
        return chorusMark; // ‘Sˆõ‚Å‰Ì‚¤•”•ª
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
