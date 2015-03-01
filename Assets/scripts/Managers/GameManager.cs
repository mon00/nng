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
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                GameManager.instance = new GameManager();
                DontDestroyOnLoad(GameManager.instance);
            }
            return GameManager.instance;
        }

    }
    private static bool IsStart = true;

    //Evets
    public delegate void GMVoidScriptHolder();
    public event GMVoidScriptHolder OnSceneChenge;
    public event GMVoidScriptHolder OnAppQuit;
    public event GMVoidScriptHolder OnConfigChange;

    //Visible variables
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

    //Hidden variables
    public Dictionary<string, string> Config { get; private set; }
    private string ConfigPlace = "Data/config";
    private bool DebugShow= false;
    public List<string> DebugList { get; private set; }

    private string SavesPath = "Data/Save/";
    private string SavesAdd = ".nng";
    private string SavesInfoAdd = ".info";

    //Functions

    private void Awake()
    {
        if (!IsStart) return;
        IsStart = false;
        OnAwake();
    }
    
    private void OnAwake()
    {

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

    public void ChengeScene(string _scene)
    {
        OnSceneChenge();
        Application.LoadLevel(_scene);
    }

    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

    //SAVES

    public Dictionary<string, string> Load(string name = "")
    {
        Dictionary<string, string> SavesData = new Dictionary<string,string>();
        if (!Directory.Exists(SavesPath))
        {
            Directory.CreateDirectory(SavesPath);
            return SavesData;
        }
        if (name == "")
        {
            string[] SavesNames = Directory.GetFiles(@SavesPath, SavesInfoAdd);
            foreach (string file in SavesNames)
            {
                string Save = SavesPath + file;
                string SaveInfo;

                FileStream fs = new FileStream(Save, FileMode.Open);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    SaveInfo = (string)bf.Deserialize(fs);
                    SavesData.Add(file, SaveInfo);
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
        }
        else
        {
            string Save = SavesPath + name + SavesAdd;
            if (!File.Exists(Save)) return SavesData;

            FileStream fs = new FileStream(Save, FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                SavesData = (Dictionary<string, string>)bf.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                AddDubugMessage("SaveLoadSystem", "Can`t load file " + Save + " - " + e.Message, 4);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        return SavesData;
    }

    public void Save(Dictionary<string, string> Data, string Info, string Name)
    {
        string SaveData = SavesPath + name + SavesAdd;
        string SaveInfo = SavesPath + name + SavesInfoAdd;

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
            AddDubugMessage("SaveLoadSystem", "Can`t save file: " + Name + " - " + e.Message, 5);
            throw;
        }
        finally
        {
            fsData.Close();
            fsInfo.Close();
        }
    }
    //CONFIG

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

    //DEBUG

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
}
       
