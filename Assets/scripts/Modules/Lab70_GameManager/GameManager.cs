using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Lab70_GameManager
{
    public enum Scene { Intro = 0, MainMenu = 1, Game = 2 };

    [SerializeField]
    public class GameManager : MonoBehaviour
    {
        protected GameManager() { }
        public static GameManager Instance { get; private set; }
        private GameManager _instance
        {
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

        private string ConfigFile = "Data/config";
        private string SaveDirectory = "Data/Save/";
        private string InfoDirectory = "Data/Info/";

        public Info GameInfo { get; private set; }
        public Data GameData { get; private set; }

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

            Config = new Dictionary<string, int>();
            LoadConfig();

            CurrentScene = (Scene)Application.loadedLevel;
        }

        //-----------Common functions-----------------//

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
            if (OnQuitApp != null) OnQuitApp();
            if (Application.isEditor)
            {
                Debug.Log("Exit app");
            }
            else
            {
                Application.Quit();
            }
        }

        //------------End of common functions---------//

        //------------- Saves functions --------------//

        public Info LoadInfo(string infoFile)
        {
            if (!Directory.Exists(InfoDirectory)) Directory.CreateDirectory(InfoDirectory);

            if (!File.Exists(InfoDirectory + infoFile))
            {
                Info info = new Info(infoFile);
                SaveInfo(info);
                return info;
            }

            Info OutInfo = new Info(infoFile);
            bool success = true;
            
            FileStream InfoFS = new FileStream(InfoDirectory + infoFile, FileMode.Open);
            try
            {
                BinaryFormatter InfoBF = new BinaryFormatter();
                OutInfo = (Info)InfoBF.Deserialize(InfoFS);
            }
            catch (TypeLoadException e)
            {
                Debug.LogError("Невозможно загрузить " + infoFile + ". Error: " + e.Message);
                success = false;
            }
            finally { InfoFS.Close(); }
            
            if (!success)
            {
                OutInfo = new Info(infoFile);
                SaveInfo(OutInfo);
            }
            
            return OutInfo;
        }

        private void SaveInfo(Info info)
        {
            if (!Directory.Exists(InfoDirectory)) Directory.CreateDirectory(InfoDirectory);
            if (File.Exists(InfoDirectory + info.FileName)) File.Delete(InfoDirectory + info.FileName);
            BinaryFormatter InfoBF = new BinaryFormatter();
            FileStream InfoFS = new FileStream(InfoDirectory + info.FileName, FileMode.CreateNew);
            InfoBF.Serialize(InfoFS, info);
            InfoFS.Close();
        }

        public void Load(Info info)
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                return;
            }

            bool success = false;

            string SaveDataFile = SaveDirectory + info.Name;

            if (!File.Exists(SaveDataFile))
            {
                Debug.LogError("No save " + SaveDataFile);
                return;
            }

            FileStream fsData = new FileStream(SaveDataFile, FileMode.Open);
            
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                GameData = (Data)bf.Deserialize(fsData);
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
                GameInfo = info;
                ChengeScene(Scene.Game);
            }
        }

        public void Save(Info info, Data data)
        {
            bool success = false;

            string SaveDataFile = SaveDirectory + info.Name;

            FileStream FileStreamInfo = new FileStream(InfoDirectory + info.FileName, FileMode.Create);
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

        public void DeleteGame(Info info)
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
                return;
            }
            if (!File.Exists(SaveDirectory + info.Name)) return;
            
            File.Delete(SaveDirectory + info.Name);
            File.Delete(InfoDirectory + info.FileName);
        }

        //------------- End of Saves functions --------------//

        //---------------- Config functions -----------------//

        private void LoadConfig()
        {
            Debug.Log("Start loading config");
            if (File.Exists(ConfigFile))
            {
                StreamReader sr = new StreamReader(ConfigFile);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parms = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parms.Length != 2) continue;
                    int value;
                    if (!Int32.TryParse(parms[1], out value)) continue;
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

            if (File.Exists(ConfigFile)) File.Delete(ConfigFile);

            StreamWriter sr = new StreamWriter(ConfigFile);
            foreach (KeyValuePair<string, int> kvp in config)
            {
                sr.WriteLine(kvp.Key + " " + kvp.Value);
            }
            sr.Close();

            Config = config;

            if (OnChengeConfig != null) OnChengeConfig();
        }

        public void ResetConfig()
        {
            Debug.Log("Make new config");
            Config = _config;
            SaveConfig(Config);
        }

        //---------- End of Config functions -----------//

        //-------------- Statistics functions ---------------//



        //---------- End of statistics functions ------------//
    }

}