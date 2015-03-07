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
        private string saveDirectory = "Data/Save/";
        private string saveDataExtension = ".nng";
        private string saveInfoExtension = ".info";

        public Dictionary<string, string> GameInfo { get; private set; }
        public GameData GameData { get; private set; }
        public string GameName { get; private set; }

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

            CurentScene = (Scene)Application.loadedLevel;
            Debug.Log("Curent scene is " + CurentScene);
        }


        public void Print()
        {
            print("Ok!");
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

        public List<string> LoadNames()
        {
            Debug.Log("Try to load save`s files names");

            List<string> names = new List<string>();

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
                return names;
            }

            string[] fullNames = Directory.GetFiles(@saveDirectory, "*" + saveInfoExtension, SearchOption.TopDirectoryOnly);

            foreach (string file in fullNames)
            {
                names.Add(Path.GetFileNameWithoutExtension(file));
            }
            return names;
        }

        public Dictionary<string, string> LoadInfo(string fileName)
        {
            Debug.Log("Try to load Info of " + fileName);

            Dictionary<string, string> saveInfo = new Dictionary<string, string>();

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
                return saveInfo;
            }

            string file = saveDirectory + fileName + saveInfoExtension;

            if (!File.Exists(file)) return saveInfo;

            FileStream infoStream = new FileStream(file, FileMode.Open);

            try
            {
                BinaryFormatter infoFormatter = new BinaryFormatter();
                saveInfo = (Dictionary<string, string>)infoFormatter.Deserialize(infoStream);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Cant read info of " + fileName + ". Error: " + e.Message);
            }
            finally
            {
                infoStream.Close();
            }

            return saveInfo;
        }

        public void LoadGame(string name)
        {
            if (!Directory.Exists(saveDirectory))
            {
                
                Directory.CreateDirectory(saveDirectory);
                return;
            }

            bool success = false;
            string SaveDataFile = saveDirectory + name + saveDataExtension;
            string SaveInfoFile = saveDirectory + name + saveInfoExtension;

            if (!File.Exists(SaveInfoFile) || !File.Exists(SaveDataFile)) return;

            FileStream fsData = new FileStream(SaveDataFile, FileMode.Open);
            FileStream fsInfo = new FileStream(SaveInfoFile, FileMode.Open);

            Debug.Log("Try to load Files: " + SaveDataFile + " and " + SaveInfoFile);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                GameData = (GameData)bf.Deserialize(fsData);
                GameInfo = (Dictionary<string, string>)bf.Deserialize(fsInfo);
                GameName = name;
                success = true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("Fail! " + e.Message); ;

                GameData = null;
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

        public void Save(string name, Dictionary<string, string> info, GameData data = null)
        {
            if (data == null) data = new GameData();
            bool success = false;

            string SaveDataFile = saveDirectory + name + saveDataExtension;
            string SaveInfoFile = saveDirectory + name + saveInfoExtension;

            FileStream FileStreamInfo = new FileStream(SaveInfoFile, FileMode.Create);
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
                Debug.Log("Cant save file " + name + ". Error: " + e.Message);
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
                GameName = name;
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
}