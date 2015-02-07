using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigManager : MonoBehaviour {

    private string pathToConfig = "Data/Config.gnn";
    
    public static Dictionary<string, string> config;

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

    public static string GetFromConfig(string key)
    {
        if (!config.ContainsKey(key)) return null;
        return config[key];
    }
}
