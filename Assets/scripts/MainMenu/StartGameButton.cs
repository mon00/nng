using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class StartGameButton : GameManager {

    private GameObject go;
    private string name;
    private string label;

    void Awake()
    {
        go = this.gameObject;
        name = go.name;
        label = ReadConfig(startGameButtonString + name);
        string st = ReadConfig(startGameButtonString + name);
        label = (st == null)? "New game" : st;
        go.GetComponentInChildren<Text>().text = label;
        go.GetComponent<Button>().onClick.AddListener(() => SetGamePlace(startGameButtonString + name));
        if (st != null)
        {
            go.GetComponent<Button>().onClick.RemoveAllListeners();
            SetGamePlace(startGameButtonString + name);
            SetGameName(label);
            go.GetComponent<Button>().onClick.AddListener(() => LoadGame(label));
        }
    }
}
