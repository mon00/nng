using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Application.LoadLevel("MainMenu");
    }
}
