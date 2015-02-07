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
    public GameObject oKey;
    public GameObject oValue;

    private Dictionary<string,string> d; 
    private bool debugOnLast;

    void Awake()
    {
        debugOnLast = debugOn;
        debugWindow.SetActive(debugOn);
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
        string key = oKey.guiText.ToString();
        Debug.Log(key);
    }
}
