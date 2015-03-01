using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

[SerializeField]
public class GameManager : MonoBehaviour
{   
    protected GameManager() { }
    public static GameManager Instance { get; private set; }
    private static bool IsStart = true;

    //------------------- Evets -------------------------//

    public delegate void GMVoidScriptHolder();
    public event GMVoidScriptHolder OnChengeScene;
    public event GMVoidScriptHolder OnQuitApp;
    public event GMVoidScriptHolder OnChangeConfig;

    //------------------- Visible variables -------------//
    
    [Header("Variables")]
    public bool UseDebug = true;
    public bool UseNewConfig = false;
    public bool UseLocalization = false;

    [Header("Debug")]
    public Canvas DebugArea;
    public GameObject DebugInput;
    public GameObject DebugOutput;
    [Range(1f, 5f)]
    public int DebugOutCodeMin;
    [Range(1f, 5f)]
    public int DebugOutCodeMax;

    //----------------- Hidden variables --------------//

    public Dictionary<string, string> Config { get; private set; }
    private string ConfigPlace = "Data/config";

    private bool DebugShow= false;
    public List<string> DebugList { get; private set; }

    private string SavesPath = "Data/Save/";
    private string SavesAdd = ".nng";
    private string SavesInfoAdd = ".info";

    public Dictionary<string, string> GameInfo { get; private set; }
    public Dictionary<string, string> GameData { get; private set; }
    public string GameName { get; private set; }

    //----------------- Functions block ---------------//

    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(this);

        if (!IsStart) return;

        IsStart = false;
        OnAwake();
    }
    
    private void OnAwake()
    {
        DontDestroyOnLoad(this.gameObject);

        Config = new Dictionary<string,string>();
        DebugList = new List<string>();

        if (DebugOutCodeMin > DebugOutCodeMax)
        {
            int tmp = DebugOutCodeMax;
            DebugOutCodeMax = DebugOutCodeMin;
            DebugOutCodeMin = tmp;
        }
        DebugArea.gameObject.SetActive(DebugShow);
        if (UseNewConfig) ResetConfig();
        else LoadConfig();
    }

    private void Update()
    {
        if (UseDebug)
        {
            if (Input.GetKeyDown("`")) EnableDebug();
        }
    }

    //------------- Event functions --------------//

    public void ChengeScene(string _scene)
    {
        OnChengeScene();
        Application.LoadLevel(_scene);
    }
    public void ChengeScene(int scene)
    {
        if (OnChengeScene != null) OnChengeScene();
        Application.LoadLevel(scene);
    }

    public void QuitApp()
    {
        OnQuitApp();
       // GameManager.instance = null;
        Application.Quit();
    }

    public void ChangeConfig()
    {
        if (OnChangeConfig == null) return;
        OnChangeConfig();
    }

    //--------- End of Event functions -----------//

    //------------- Saves functions --------------//

    public Dictionary<string, Dictionary<string, string>> Load()
    {
        Dictionary<string, Dictionary<string, string>> SavesData = new Dictionary<string,Dictionary<string,string>>();

        if(!Directory.Exists(SavesPath))
        {
            Directory.CreateDirectory(SavesPath);
            return SavesData;
        }

        string[] FileNames = Directory.GetFiles(@SavesPath, SavesInfoAdd);
        foreach(string file in FileNames)
        {
            string SaveName = file;
            print("Load File = " + file);

            Dictionary<string, string> SaveInfo = new Dictionary<string,string>();
            FileStream fs = new FileStream(SavesPath + SaveName + SavesInfoAdd, FileMode.Open);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                SaveInfo = (Dictionary<string, string>)bf.Deserialize(fs);
                SavesData.Add(SaveName, SaveInfo);
            }
            catch
            {
                AddDubugMessage("SaveLoadSystem", "Can`t load info of save file: " + file, 3);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        return SavesData;
    }

    public void Load(string name)
    {
        if (!Directory.Exists(SavesPath))
        {
            Directory.CreateDirectory(SavesPath);
            return;
        }


        bool success = false;
        string SaveInfoPath = SavesPath + name + SavesAdd;
        string SaveDataPath = SavesPath + name + SavesInfoAdd;

        if (!File.Exists(SaveInfoPath) || !File.Exists(SaveDataPath)) return;

        FileStream fsData = new FileStream(SaveDataPath, FileMode.Open);
        FileStream fsInfo = new FileStream(SaveInfoPath, FileMode.Open);

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            GameData = (Dictionary<string, string>)bf.Deserialize(fsData);
            GameInfo = (Dictionary<string, string>)bf.Deserialize(fsInfo);
            GameName = name;

            success = true;
        }
        catch (SerializationException e)
        {
            AddDubugMessage("SaveLoadSystem", "Can`t load file " + name + " - " + e.Message, 4);

            GameData = new Dictionary<string, string>();
            GameInfo = new Dictionary<string, string>();
            GameName = "";

            throw;
        }
        finally
        {
            fsData.Close();
            fsInfo.Close();
        }
        if (success)
        {
            ChengeScene(2);
        }
    }

    public void Save(string name, Dictionary<string, string> Info, Dictionary<string, string> Data)
    {
        GameName = name;
        GameInfo = Info;
        GameData = Data;

        string SaveData = SavesPath + GameName + SavesAdd;
        string SaveInfo = SavesPath + GameName + SavesInfoAdd;

        FileStream fsData = new FileStream(SaveData, FileMode.Create);
        FileStream fsInfo = new FileStream(SaveInfo, FileMode.Create);
        
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            bf.Serialize(fsData, Data);
            bf.Serialize(fsInfo, Info);
        }
        catch (SerializationException e)
        {
            AddDubugMessage("SaveLoadSystem", "Can`t save file: " + GameName + " - " + e.Message, 5);
            throw;
        }
        finally
        {
            fsData.Close();
            fsInfo.Close();
        }
    }

    //------------- End of Saves functions --------------//

    //---------------- Config functions -----------------//

    private void LoadConfig()
    {
        AddDubugMessage(this.gameObject.name, "Start loading Config");
        if (File.Exists(ConfigPlace))
        {
            StreamReader sr = new StreamReader(ConfigPlace);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parms = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parms.Length != 2) continue;
                Config.Add(parms[0], parms[1]);
            }
            sr.Close();
            AddDubugMessage(this.gameObject.name, "Config succsessfuly loaded");
        }
        else
        {
            AddDubugMessage(this.gameObject.name, "No Config File.", 2);
            ResetConfig();
        }
    }

    public void SaveConfig()
    {
        if (Config.Count < 1)
        {
            AddDubugMessage(this.gameObject.name, "Can`t save Config. It is clear");
            return;
        }
        if (File.Exists(ConfigPlace)) File.Delete(ConfigPlace);

        StreamWriter sr = new StreamWriter(ConfigPlace);
        foreach (KeyValuePair<string, string> kvp in Config)
        {
            sr.WriteLine(kvp.Key + " " + kvp.Value);
        }
        sr.Close();
        AddDubugMessage(this.gameObject.name, "Config saved", 2);
    }

    public void ResetConfig()
    {
        AddDubugMessage(this.gameObject.name, "Creating new Config", 2);
        if (Config.Count > 0) Config.Clear();
        Config.Add("Swidth", Screen.width.ToString());
        Config.Add("SHeight", Screen.height.ToString());
        SaveConfig();
    }

    //---------- End of Config functions -----------//

    //-------------- Debug functions ---------------//

    private void EnableDebug()
    {
        DebugShow = !DebugShow;
        if (DebugArea != null)
        {
            if (DebugShow)
            {
                DebugArea.gameObject.SetActive(DebugShow);
                ShowDebugMessage();
            }
            else
            {
                DebugOutput.GetComponent<Text>().text = "";
                DebugArea.gameObject.SetActive(DebugShow);
            }
        }
    }

    public void AddDubugMessage(string subject, string message, int code = 1)
    {
        string time = DateTime.Now.ToString("T");
        string line = time + ";" + subject + ";" + message + ";" + code;
        DebugList.Add(line);
        if (code >= DebugOutCodeMin && code <= DebugOutCodeMax && DebugShow)
        {
            line = "[" + time + "] {" + subject + "} " + message + " (" + code + ")" + "\n";
            DebugOutput.GetComponent<Text>().text += line;
        }
    }

    public void ShowDebugMessage(bool all = false)
    {
        if (!DebugShow) return;
        foreach (string line in DebugList)
        {
            string[] lines = line.Split(';');
            int code;
            if(!int.TryParse(lines[3], out code)) break;
            if ((code < DebugOutCodeMin || code > DebugOutCodeMax) && !all) continue;
            string formLine = "[" + lines[0] + "] {" + lines[1] + "} " + lines[2] + " (" + lines[3] + ")" + "\n";
            DebugOutput.GetComponent<Text>().text += formLine;
        }
    }

    public void ShowDebugMessage(string name)
    {

    }

    public void ShowDebugMessage(int keyMin, int keyMax = 5)
    {

    }

    public void DebugCommand(string command)
    {
        string[] commands = command.Split(' ');
        int count = commands.Length;
        bool succsess = false;

        switch (count)
        {
            case 1:
                if (commands[0] == "Clear")
                {
                    DebugList.Clear();
                    DebugOutput.GetComponent<Text>().text = "";
                    succsess = true;
                }
                break;

            default:
                break;
        }

        if (!succsess) AddDubugMessage("System", "No such command: " + command);
    }

    //----------- End of Debug functions ------------//
}
       
