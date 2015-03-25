using UnityEngine;
using System.Collections;
using Lab70_Framework.Config;
using Lab70_Framework.Error;
using System;
using System.Collections.Generic;

namespace Lab70_Framework
{
    public class FrameworkController : MonoBehaviour
    {
        public static FrameworkController Instance;
        private FrameworkController _instance
        {
            get { return Instance; }
            set
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(this);
                }
                else
                {
                    Debug.Log("Try to put one more FrameworkController!");
                    Destroy(this);
                }
            }
        }

        public ErrorController Error { get; private set; }
        public CacheController Cache { get; private set; }
        public ConfigController Config
        {
            get { return Config; }
            set
            {
                if (OnConfigChange != null) OnConfigChange();
                Config = value;
            }
        }
        public LocalizationController Localization { get; private set; }
        public ClientController Client { get; private set; }
        public StatisticsController Statistics { get; private set; }

        public Dictionary<string, ScriptableObject> Modules { get; private set; }

        //Events

        public delegate void VoidEvent();
        public event VoidEvent OnErrorAction;
        public event VoidEvent OnAppQuit;
        public event VoidEvent OnConfigChange;
        public event VoidEvent OnSceneChange;

        //EndOfEvents

        //Public variables

        [Header("Modules")]
        [Tooltip("Put here Modules to download it in Framework")]
        public ScriptableObject[] GameModulesLoader;

        [Header("Controllers Elements")]
        public GameObject ErrorWindowPrefab;

        // Private variables

        private string dataDirectory = "Data/";
        private string errorDirectory = "Error/";
        private string chachDirectory = "Cache/";
        private string configFileName = "config";
        private string localizationDirectory = "Local/";
        private string clientDirectory = "Client/";
        private string statisticsDirectory = "Stat/";

        public void StartFramework()
        {
            Instance = this;

            Error = new ErrorController(dataDirectory + errorDirectory, ErrorWindowPrefab);
            Cache = new CacheController();
            Config = new ConfigController(dataDirectory, configFileName, false);
            Localization = new LocalizationController();
            Client = new ClientController();
            Statistics = new StatisticsController();

            Modules = new Dictionary<string, ScriptableObject>();
            foreach (ScriptableObject so in GameModulesLoader)
            {
                if (Modules.ContainsKey(so.name))
                {
                    Debug.Log("Попытка загрзить 2 модуля с одинаковыми именами!");
                    continue;
                }
                Modules.Add(so.name, so);
            }
        }

        public void OnError(ErrorData error)
        {
            if (OnErrorAction != null) OnErrorAction();
        }

        public void QuitApp()
        {
            if (OnAppQuit != null) OnAppQuit();
            if (Application.isEditor)
                print("Приложение успушно завершенно!");
            else
                Application.Quit();
        }

        public void TestError()
        {
            string ob = this.name;
            string message = "Hello, This is a test message for you. You can go and fuck your self!";
            Error.GenerateError(ob, message);
        }
    }
}