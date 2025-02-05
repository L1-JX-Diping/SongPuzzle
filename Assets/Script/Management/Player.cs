using UnityEngine;

/// <summary>
/// Role of Player: name, color, avatar, mic
/// </summary>
[System.Serializable]
public class Player 
{
    public string name;
    public Color color; // 割り当てた色
    public string avatar; // mark 
    public string micDevice; // マイク名
}
