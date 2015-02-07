using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGame : MonoBehaviour {
    public GameObject[] startButtons;

    void Awake()
    {
        foreach (GameObject go in startButtons)
        {
            string name = go.name;

        }
    }

}
