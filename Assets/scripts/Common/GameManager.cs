using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour {

    private string pathToConfig = "Data/Config.gnn";
    private Dictionary<string, string> config;

    void Awake()
    {
        if (config == null)
        {
            if (SaveLoadManager.Check("config")) config = SaveLoadManager.Load("config");
            else
            {
                config = StandartConfig.standartConfig;
                SaveLoadManager.Save(config, "config");
            }
        }
    }
}
