using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    public GameObject[] menuElements;

    void Awake()
    {
        ClickOpenClose(null);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) ClickOpenClose(null);
    }

    public void ClickOpenClose(GameObject curentElement)
    {
        foreach (GameObject g in menuElements)
        {
            if (g == curentElement)
            {
                if (g.activeInHierarchy)
                {
                    g.SetActive(false);
                    break;
                }
                g.SetActive(true);
            }
            else if(g.activeInHierarchy) g.SetActive(false);
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
