using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace game
{

    public enum Scene { Intro = 0, MainMenu = 1, Game = 2 };

    [SerializeField]
    public class GameManager : MonoBehaviour
    {
        protected GameManager() { }
        public static GameManager Instance { get; private set; }
        private GameManager _instance { 
            get 
            {
                return Instance;
            }

            set
            {
                if (Instance == null) Instance = this;
                else Destroy(this.gameObject);
            }
        }
        private static bool IsStart = true;

        //------------------- Evets -------------------------//

        public delegate void GMVoidScriptHolder();
        public event GMVoidScriptHolder OnChengeScene;
        public event GMVoidScriptHolder OnQuitApp;
        public event GMVoidScriptHolder OnChengeConfig;

        //------------------- Visible variables -------------//

        [Header("Variables")]
        //public bool UseDebug = true;
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

        private string ConfigPlace = "Data/config";
        private string SavesPath = "Data/Save/";
        private string SavesAdd = ".nng";
        private string SavesInfoAdd = ".info";

        public Dictionary<string, string> Config { get; private set; }
        public Dictionary<string, string> GameInfo { get; private set; }
        public Dictionary<string, string> GameData { get; private set; }

        public string GameName { get; private set; }

        public Scene CurentScene { get; private set; }

        //public List<string> DebugList { get; private set; }

        //----------------- Functions block ---------------//

        private void Awake()
        {

            _instance = this;

            if (!IsStart) return;
            IsStart = false;

            OnAwake();
        }

        private void OnAwake()
        {
            DontDestroyOnLoad(this.gameObject);

            Config = new Dictionary<string, string>();

            if (DebugOutCodeMin > DebugOutCodeMax)
            {
                int tmp = DebugOutCodeMax;
                DebugOutCodeMax = DebugOutCodeMin;
                DebugOutCodeMin = tmp;
            }

            if (UseNewConfig) ResetConfig();
            else LoadConfig();

            CurentScene = (Scene)Application.loadedLevel;
            Debug.Log("Curent scene is " + CurentScene);
        }

        //------------- Event functions --------------//

        public void ChengeScene(Scene NewScene)
        {
            Debug.Log("Try to chenge scene to " + NewScene);
            if (CurentScene == NewScene)
            {
                Debug.LogWarning("New scene is same with Curent Scene!");
                return;
            }
            if (OnChengeScene != null) OnChengeScene();
            CurentScene = NewScene;
            Application.LoadLevel((int)CurentScene);
        }

        public void QuitApp()
        {
            Debug.Log("Quit App");
            if (OnQuitApp != null) OnQuitApp();
            Application.Quit();
        }

        public void ChengeConfig()
        {
            Debug.Log("Change Config");
            if (OnChengeConfig != null) return;
            OnChengeConfig();
        }

        //--------- End of Event functions -----------//

        //------------- Saves functions --------------//

        public Dictionary<string, Dictionary<string, string>> Load()
        {
            Debug.Log("Try to load save files Info");

            Dictionary<string, Dictionary<string, string>> SavesData = new Dictionary<string, Dictionary<string, string>>();

            if (!Directory.Exists(SavesPath))
            {
                Directory.CreateDirectory(SavesPath);
                return SavesData;
            }

            string[] FileNames = Directory.GetFiles(@SavesPath, "*" + SavesInfoAdd, SearchOption.TopDirectoryOnly);
            foreach (string file in FileNames)
            {
                string SaveName = Path.GetFileNameWithoutExtension(SavesPath + file);
                print("Load File Info = " + SaveName);

                Dictionary<string, string> SaveInfo = new Dictionary<string, string>();
                FileStream fs = new FileStream(SavesPath + SaveName + SavesInfoAdd, FileMode.Open);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    SaveInfo = (Dictionary<string, string>)bf.Deserialize(fs);
                    SavesData.Add(SaveName, SaveInfo);
                }
                catch (SerializationException e)
                {
                    Debug.Log("Cant load file Info " + name + ". Error: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
            Debug.Log("End of loading save files Info");
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
            string SaveDataPath = SavesPath + name + SavesAdd;
            string SaveInfoPath = SavesPath + name + SavesInfoAdd;

            if (!File.Exists(SaveInfoPath) || !File.Exists(SaveDataPath)) return;

            FileStream fsData = new FileStream(SaveDataPath, FileMode.Open);
            FileStream fsInfo = new FileStream(SaveInfoPath, FileMode.Open);

            Debug.Log("Try to load Files: " + SaveDataPath + " and " + SaveInfoPath);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                GameData = (Dictionary<string, string>)bf.Deserialize(fsData);
                GameInfo = (Dictionary<string, string>)bf.Deserialize(fsInfo);

                success = true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("Fail! " + e.Message); ;

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
                Debug.Log("Loading success!");
                ChengeScene(Scene.Game);
            }
        }

        public void Save(string name, Dictionary<string, string> Info, Dictionary<string, string> Data = null)
        {
            if (Data == null) Data = new Dictionary<string, string>();

            GameName = name;
            GameInfo = Info;
            GameData = Data;

            string SaveData = SavesPath + GameName + SavesAdd;
            string SaveInfo = SavesPath + GameName + SavesInfoAdd;

            FileStream fsData = new FileStream(SaveData, FileMode.Create);
            FileStream fsInfo = new FileStream(SaveInfo, FileMode.Create);

            Debug.Log("Try to save " + SaveData + " and " + SaveInfo);

            BinaryFormatter bfInfo = new BinaryFormatter();
            BinaryFormatter bfData = new BinaryFormatter();
            try
            {
                bfInfo.Serialize(fsData, Data);
                bfData.Serialize(fsInfo, Info);
            }
            catch (SerializationException e)
            {
                Debug.Log("Cant save file " + name + ". Error: " + e.Message);
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
            Debug.Log("Start loading config");
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
                Debug.Log("Config successfly loaded!");
            }
            else
            {
                Debug.Log("No config file");
                ResetConfig();
            }
        }

        public void SaveConfig()
        {
            if (Config.Count < 1)
            {
                Debug.Log("Can`t save config. It is clear!");
                return;
            }
            if (File.Exists(ConfigPlace)) File.Delete(ConfigPlace);

            StreamWriter sr = new StreamWriter(ConfigPlace);
            foreach (KeyValuePair<string, string> kvp in Config)
            {
                sr.WriteLine(kvp.Key + " " + kvp.Value);
            }
            sr.Close();
            Debug.Log("Config saved");
        }

        public void ResetConfig()
        {
            Debug.Log("Reset config");
            if (Config.Count > 0) Config.Clear();
            Config.Add("Swidth", Screen.width.ToString());
            Config.Add("SHeight", Screen.height.ToString());
            SaveConfig();
        }

        //---------- End of Config functions -----------//



        //-------------- Debug functions ---------------//
        /*
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
        */
        //----------- End of Debug functions ------------//
    }

}