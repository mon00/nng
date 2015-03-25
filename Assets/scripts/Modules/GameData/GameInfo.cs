using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[SerializeField]
public class GameInfo{

    public bool IsNewGame;
    public string File;
    public string PlayerName;
    public string WorldSize;
    public Image ScreenImage;

    public GameInfo(string playerName, string worldSize)
    {
        PlayerName = playerName;
        WorldSize = worldSize;
    }

    public GameInfo(string file)
    {
        File = file;
        IsNewGame = true;
    }
    
}
