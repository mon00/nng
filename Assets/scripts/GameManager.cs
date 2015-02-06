using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private bool isConfig;

    void Awake()
    {
        if(!isConfig) CheckConfig();
    }

    void CheckConfig()
    {

    }
}
