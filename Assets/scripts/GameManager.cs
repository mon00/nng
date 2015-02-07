using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    public bool checkBeforeStart;

    void Start()
    {
        if (checkBeforeStart)
        {
            Debug.Log("Start check...");
        }
    }

}
