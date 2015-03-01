using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

    [HideInInspector]
    public GameManager GM;

    public Dictionary<string, string> SavedGames = new Dictionary<string,string>();
    
    [HideInInspector]
    public bool NoSaves = false;

    public void Awake()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        SavedGames = GM.Load();
        if (SavedGames.Count == 0) NoSaves = true;
    }
}
