using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {
   [SerializeField]
    public GameObject[] startButtons;
    


    void Awake()
    {
        foreach (GameObject go in startButtons)
        {
            string st = ConfigManager.GetFromConfig(go.name);
            Text t = go.GetComponent<Text>();
            print(st);
            t.text = st;
        }
    }

}
