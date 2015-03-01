using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[SerializeField]
public class DebugManager : MonoBehaviour {

    protected DebugManager() { }
    private static DebugManager instance = null;
    public static DebugManager Instance
    {
        get
        {
            if (DebugManager.instance == null)
            {
                DebugManager.instance = new DebugManager();
                DontDestroyOnLoad(DebugManager.instance);
            }
            return DebugManager.instance;
        }

    }
    private bool IsStart = true;

    public delegate void DMVoidScriptHolder();
    public event DMVoidScriptHolder OnDebugLogChenge;

    //Visible variables
    public bool DebugOn;
    public GameObject DebugInput;
    public GameObject DebugOutput;

    //Hidden vaariables
    public string[] DebugLog { get; private set; }

    //functions

    private void Awake()
    {
        if (IsStart)
        {
            IsStart = false;
            OnAwake();
        }
    }

    private void OnAwake()
    {

    }
}
