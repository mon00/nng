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
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
                else Destroy(this.gameObject);
            }
        }

        //------------------- Visible variables -------------//

        public bool ResetConfigOnStart;

        //----------------- Hidden variables --------------//

        private string configFile = "Data/config";
        private string SaveDirectory = "Data/Save/";
        private string InfoFile = "Data/info";

        public GameInfo GameInfo = new GameInfo();
        public GameInfoHolder GameInfoHolder = new GameInfoHolder();
        public GameData GameData { get; private set; }

        public Dictionary<string, int> Config { get; private set; }
        private Dictionary<string, int> _config = new Dictionary<string, int>
        {
            {"FullScreen", 1},
            {"Soundvalue", 100},
            {"QualityLevel", 3},
        };

        public Scene CurentScene { get; private set; }

        //------------------- Evets -------------------------//

        public delegate void GMVoidScriptHolder();
        public event GMVoidScriptHolder OnChengeScene;
        public event GMVoidScriptHolder OnQuitApp;
        public event GMVoidScriptHolder OnChengeConfig;
        //----------------- Functions block ---------------//

        private void Awake()
        {
            _instance = this;

            Config = new Dictionary<string, int>();

            if (ResetConfigOnStart) ResetConfig();
            else LoadConfig();

            LoadInfoArray();

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
            if (OnChengeConfig != null) OnChengeConfig();
            
        }

        //--------- End of Event functions -----------//

        //------------- Saves functions --------------//

        public void LoadInfoArray()
        {
            Debug.Log("Try to load Info Array");

            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                return;
            }

            if (!File.Exists(InfoFile))
            {
                return;
            }

            FileStream infoStream = new FileStream(InfoFile, FileMode.Open);

            try
            {
                BinaryFormatter infoFormatter = new BinaryFormatter();
                GameInfoHolder = (GameInfoHolder)infoFormatter.Deserialize(infoStream);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Cant read info. Error: " + e.Message);
            }
            finally
            {
                infoStream.Close();
            }
        }

        public void LoadGame(GameInfo gameInfo)
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                return;
            }

            bool success = false;
            string SaveDataFile = SaveDirectory + gameInfo.Name;

            if (!File.Exists(SaveDataFile)) return;

            FileStream fsData = new FileStream(SaveDataFile, FileMode.Open);

            Debug.Log("Try to load save  " + SaveDataFile);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                GameData = (GameData)bf.Deserialize(fsData);
                success = true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("Fail! " + e.Message); ;
                GameData = null;
                throw;
            }
            finally
            {
                fsData.Close();
            }
            if (success)
            {
                Debug.Log("Loading success!");
                ChengeScene(Scene.Game);
            }
        }

        public void Save(GameInfo info, GameData data)
        {
            bool success = false;

            switch (info.Position)
            {
                case 1:
                    GameInfoHolder.One = info;
                    break;
                case 2:
                    GameInfoHolder.Two = info;
                    break;
                case 3:
                    GameInfoHolder.Three = info;
                    break;
                default:
                    return;
            }

            string SaveDataFile = SaveDirectory + info.Name;

            FileStream FileStreamInfo = new FileStream(InfoFile, FileMode.Create);
            FileStream FileStreamData = new FileStream(SaveDataFile, FileMode.Create);

            Debug.Log("Start saving  " + name);

            BinaryFormatter BinaryFormatterInfo = new BinaryFormatter();
            BinaryFormatter BinaryFormatterData = new BinaryFormatter();

            try
            {
                BinaryFormatterInfo.Serialize(FileStreamInfo, info);
                BinaryFormatterData.Serialize(FileStreamData, data);
                success = true;
            }
            catch (SerializationException e)
            {
                Debug.Log("Cant save file " + info.Name + ". Error: " + e.Message);
                throw;
            }
            finally
            {
                FileStreamInfo.Close();
                FileStreamData.Close();
            }
            if (success)
            {
                GameInfo = info;
                GameData = data;
                Debug.Log("Save success!");
            }

        }

        //------------- End of Saves functions --------------//

        //---------------- Config functions -----------------//

        private void LoadConfig()
        {
            Debug.Log("Start loading config");
            if (File.Exists(configFile))
            {
                StreamReader sr = new StreamReader(configFile);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parms = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parms.Length != 2) continue;
                    int value;
                    if(!Int32.TryParse(parms[1], out value)) continue;
                    Config.Add(parms[0], value);
                }
                sr.Close();
                Debug.Log("Config successfuly loaded!");
            }
            else
            {
                Debug.Log("No config file");
                ResetConfig();
            }
        }

        public void SaveConfig(Dictionary<string, int> config)
        {
            if (config.Count < 1)
            {
                Debug.Log("Can`t save config. It is clear!");
                return;
            }

            if (File.Exists(configFile)) File.Delete(configFile);

            StreamWriter sr = new StreamWriter(configFile);
            foreach (KeyValuePair<string, int> kvp in config)
            {
                sr.WriteLine(kvp.Key + " " + kvp.Value);
            }
            sr.Close();
            
            Config = config;

            if(OnChengeConfig != null) ChengeConfig();
        }

        public void ResetConfig()
        {
            Debug.Log("Make new config");
            Config = _config;
            SaveConfig(Config);
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

    [System.Serializable]
    public class GameData
    {
        public bool IsNewGame = true;
    }

    [System.Serializable]
    public class GameInfoHolder
    {
        public GameInfo One = new GameInfo();
        public GameInfo Two = new GameInfo();
        public GameInfo Three = new GameInfo();
    }

    [System.Serializable]
    public class GameInfo
    {
        public bool NewGame = true;
        public string Name = "";
        public int Position;

        public int WorldSize;
        public int WorldDispersion;

        public int StartBiomSize;

        public int SecondBiomSize;
        public int SecondBiomCount;

        public int ThirdBiomSize;
        public int ThirdBiomCount;

        public int FourthBiomSize;
        public int FourthBiomCount;

        public int FifthBiomSize;
        public int FifthBiomCount;
    }
}