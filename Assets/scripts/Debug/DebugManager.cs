using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour {
    [Header("Основное")]
    [SerializeField]
    public bool debugOn;
    [SerializeField]
    public GameObject debugWindow;

    [Header("Поля ввода")]
    
    [SerializeField]
    public GameObject oKey;
    
    [SerializeField]
    public GameObject oValue;
    
    [SerializeField]
    public string fileToSave = "DebugSave";

    
    private Dictionary<string,string> d; 
    private bool debugOnLast;

    void Awake()
    {
        debugOnLast = debugOn;
        debugWindow.SetActive(debugOn);
        d = new Dictionary<string, string>();
    }
    void FixedUpdate()
    {
        if (debugOnLast != debugOn)
        {
            debugOnLast = debugOn;
            debugWindow.SetActive(debugOn);
        }
    }

    public void Add()
    {
        Text key = oKey.GetComponent<Text>();
        Text value = oValue.GetComponent<Text>();
        if (key.text!="" && value.text!="")
        {
            this.d.Add(key.text, value.text);
        }
    }
    public void print()
    {
        if (d.Count != 0)
        {
            Debug.Log("Key - Value в словаре d");
            foreach (KeyValuePair<string, string> kvs in d)
            {
                Debug.Log(kvs.Key + " - " + kvs.Value);
            }
            Debug.Log("Конец перечисления");
        }
        else Debug.Log("Словарь пуст!");
    }
    public void Save()
    {
        if (d.Count != 0)
        {
            SaveLoadManager.Save(d, fileToSave);
        }
        else Debug.Log("Словарь пуст");
    }
    public void Load()
    {
        d.Clear();
        d = SaveLoadManager.Load(fileToSave);
    }
    public void Clear()
    {
        d.Clear();
    }
}
