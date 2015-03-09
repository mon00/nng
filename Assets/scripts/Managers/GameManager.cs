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

        private int GameSavesCount = 3;
        public GameInfo CurrentGameInfo;
        public GameInfo[] GameInfoArray { get; private set; }
        public GameData CurrentGameData { get; private set; }

        public Dictionary<string, int> Config { get; private set; }
        private Dictionary<string, int> _config = new Dictionary<string, int>
        {
            {"FullScreen", 1},
            {"Soundvalue", 100},
            {"QualityLevel", 3},
        };

        public Scene CurrentScene { get; private set; }

        //------------------- Evets -------------------------//

        public delegate void GMVoidScriptHolder();
        public event GMVoidScriptHolder OnChengeScene;
        public event GMVoidScriptHolder OnQuitApp;
        public event GMVoidScriptHolder OnChengeConfig;
        //----------------- Functions block ---------------//

        private void Awake()
        {
            _instance = this;

            print(configFile);

            Config = new Dictionary<string, int>();

            if (ResetConfigOnStart) ResetConfig();
            else LoadConfig();

            GameInfoArray = new GameInfo[GameSavesCount];
            for (int i = 0; i < GameSavesCount; i++)
            {
                GameInfoArray[i] = new GameInfo(i);
            }
            CurrentGameData = new GameData();

            LoadInfoArray();

            foreach (GameInfo info in GameInfoArray)
            {
                print("Info index = " + info.Index);
            }

            CurrentScene = (Scene)Application.loadedLevel;
            Debug.Log("Current scene is " + CurrentScene);
        }

        //------------- Event functions --------------//

        public void ChengeScene(Scene NewScene)
        {
            Debug.Log("Try to chenge scene to " + NewScene);
            if (CurrentScene == NewScene)
            {
                Debug.LogWarning("New scene is same with Current Scene!");
                return;
            }
            if (OnChengeScene != null) OnChengeScene();
            CurrentScene = NewScene;
            Application.LoadLevel((int)CurrentScene);
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
                GameInfoArray = (GameInfo[])infoFormatter.Deserialize(infoStream);
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

        private void SaveInfoArray()
        {
            FileStream infoStream = new FileStream(InfoFile, FileMode.Open);
            BinaryFormatter infoFormatter = new BinaryFormatter();
            GameInfoArray = (GameInfo[])infoFormatter.Deserialize(infoStream);
            infoStream.Close();
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

            if (!File.Exists(SaveDataFile))
            {
                Debug.LogError("No save " + SaveDataFile);
                return;
            }

            FileStream fsData = new FileStream(SaveDataFile, FileMode.Open);

            Debug.Log("Try to load save  " + SaveDataFile);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                CurrentGameData = (GameData)bf.Deserialize(fsData);
                success = true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("Fail! " + e.Message);
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

            GameInfoArray[info.Index] = info;
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
                CurrentGameInfo = info;
                CurrentGameData = data;
                Debug.Log("Save success!");
            }
        }

        public void DeleteGame (GameInfo info)
        {
            if (!Directory.Exists(SaveDirectory)) return;
            if (!File.Exists(SaveDirectory + info.Name)) return;

            File.Delete(SaveDirectory + info.Name);
            GameInfoArray[info.Index] = new GameInfo(info.Index);
            SaveInfoArray();
            Debug.Log("Game " + info.Name + " deleted!");
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
    public class GameInfo
    {
        public static int InfoCount = 0;

        public bool NewGame;
        public string Name;
        public int Index;

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

        public GameInfo(int index)
        {
            this.Index = index;
            this.NewGame = true;
            this.Name = "";
        }
    }
}