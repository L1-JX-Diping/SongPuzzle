using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 扱うので
using UnityEngine.SceneManagement; // Scene の切り替えしたい場合に必要な宣言

/// <summary>
/// Move from Assignment class 
/// </summary>
public class DisplayRole : MonoBehaviour
{
    private Data _data = new Data(); 
    private int _playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // assign role to players and save the data
        Assignment assignment = new Assignment();
        assignment.DoAssignment(); 

        // Get and Set game data registered in previous page from file
        LoadData();

        // display role assignment on the screen
        DisplayOnScreen();
    }

    // Update is called once per frame
    void Update() { }

    /// <summary>
    /// Set game data to use in this class
    /// </summary>
    private void LoadData()
    {
        _data = (Data)Common.LoadXml(_data.GetType(), FileName.XmlGameData);
        _playerCount = _data.Team.CountMembers;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void DisplayOnScreen()
    {
        /* display mic(assignment) information */
        for (int index = 0; index < _playerCount; index++)
        {
            DisplayAssignment(index);
        }
    }

    /// <summary>
    /// Display to screen (index: players' index)
    /// </summary>
    /// <param name="index"></param>
    private void DisplayAssignment(int index)
    {
        string objName = "";
        string avatarName = "";
        Text colorField;
        Text micField;

        // Default: Play as Anoymous
        int playerNo = index + 1; // playerNo: from 1, index: from 0

        // Set text field to display assignment information
        objName = "Color" + playerNo.ToString();
        avatarName = "Avatar" + playerNo.ToString();
        colorField = GameObject.Find(objName).GetComponent<Text>();

        // Set text field to display assignment information
        objName = "MicName" + playerNo.ToString();
        micField = GameObject.Find(objName).GetComponent<Text>();

        /* Display players' role on the screen */
        List<Player> playerList = _data.Team.MemberList; 
        if (colorField != null && micField != null)
        {
            // Get and Set assignment information for this player
            Player player = playerList[index];
            Role role = player.Role;
            Color color = role.Color;
            string colorName = Common.ToColorName(color);

            // Display mic DEVICE name
            micField.text = player.Name + ": \n [mic]: " + role.Mic;

            // Display assigned COLOR name
            colorField.text = colorName; // display text (color name)
            colorField.color = color; // Change text color

            // Change AVATAR color displayed on the screen
            ReflectToAvatar(avatarName, color);
        }
        else
        {
            Debug.LogError($"Textbox {colorField} or {micField} is not assigned.");
        }
    }

    /// <summary>
    /// Set Color to Avatar
    /// </summary>
    /// <param name="avatarName"></param>
    /// <param name="color"></param>
    void ReflectToAvatar(string avatarName, Color color)
    {
        // avatarName = "Avatar1" 
        // Canvas内の objName オブジェクトを探す
        GameObject avatarObject = GameObject.Find(avatarName);

        // オブジェクトが見つかった場合
        if (avatarObject != null)
        {
            // Imageコンポーネントを取得
            Image avatarImage = avatarObject.GetComponent<Image>();

            if (avatarImage != null)
            {
                // ImageのColorプロパティに色を設定
                avatarImage.color = color;
                Debug.Log($"Color {Common.ToColorName(color)} applied to {avatarName}.");
            }
            else
            {
                Debug.LogError($"{avatarName} does not have an Image component.");
            }
        }
        else
        {
            Debug.LogError($"{avatarName} object not found in the scene.");
        }
    }

}
