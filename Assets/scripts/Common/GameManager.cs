using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

    private Dictionary<string, string> masterConfig = new Dictionary<string, string>
    {
        {"SGB_Game1", null},
        {"SGB_Game2", null},
        {"SGB_Game3", null},
    };

    private const string configPath = "Data/";
    private const string configAdd = ".gnn";
    private const string configName = "config";

    public const string startGameButtonString = "SGB_";

    private Dictionary<string, string> config = new Dictionary<string,string>();
    private SaveLoadManager SLM = new SaveLoadManager();

    void Awake()
    {
        if (config.Count == 0)
        {
            LoadConfig();
        }
    }

    private void LoadConfig()
    {
        config = SLM.LoadFile(configName, configPath, configAdd);
        if (config == null)
        {
            config = masterConfig;
            SaveConfig();
        }
    }

    public void SaveConfig()
    {
        SLM.SaveFile(config, configPath, configName, configAdd);
    }

    public bool ChangeConfig(string key, string value)
    {
        if (!config.ContainsKey(key)) return false;
        config[key] = value;
        return true;
    }
}
