using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    GameManager GM;

    void Awake()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        Dictionary<string, string> test = new Dictionary<string, string>();
        test = GM.Config;

        foreach (KeyValuePair<string, string> kvp in test)
        {
            print(kvp.Key + " --- " + kvp.Value);
        }
    }
}
