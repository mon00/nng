using UnityEngine;
using System.Collections;
using Lab70_Framework;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {

    public static GameController Instance;
    private GameController _instance
    {
        get { return Instance; }
        set
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if(Instance != this)
            {
                Debug.Log("Try to put one more GameController!");
                Destroy(this);
            }
        }
    }

    public FrameworkController Framework { get; private set; }

    public delegate void GameControllerEvent();
    public event GameControllerEvent OnSave;
    public event GameControllerEvent OnLoad;
    public event GameControllerEvent OnInfoListChange;
    
    public GameInfo Info { get; private set; }
    public GameData Data { get; private set; }

    private string SavesPath = "Data/Save/";
    private string InfoExtension = ".info";
    private string DataExtension = ".data";
    private readonly List<string> InfoNames = new List<string>{"one", "two", "three"};

    void Awake()
    {
        if (Framework == null) return;
        Framework.StartFramework();
    }

    private List<GameInfo> LoadInfoList()
    {
        List<GameInfo> list = new List<GameInfo>();
        if (!Directory.Exists(SavesPath))
        {
            Directory.CreateDirectory(SavesPath);
            foreach (string st in InfoNames)
            {
                GameInfo info = new GameInfo(st);
                list.Add(info);
            }
            return list;
        }
        foreach (string st in InfoNames)
        {
            if(!File.Exists(SavesPath + st + InfoExtension))
            {
                GameInfo ClearInfo = new GameInfo(st);
                list.Add(ClearInfo);
                continue;
            }

            FileStream fs = new FileStream(SavesPath + st + InfoExtension, FileMode.Open);
            GameInfo info;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                info = (GameInfo)bf.Deserialize(fs);
                list.Add(info);
            }
            catch (SerializationException e)
            {
                Framework.Error.GenerateError(this.name, "Не удалось загрузить Info file. Старый будет удалён!");
                Debug.Log("Ошика чтения info file: " + e.Message);
                info = new GameInfo(st);
                list.Add(info);
            }
            finally
            {
                fs.Close();
            }
        }
        return list;
    }
}
