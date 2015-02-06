using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    public Canvas menu;
    [SerializeField]
    public GameObject[] menuElements;
    [SerializeField]
    public GameObject gameImage;

    void Awake()
    {
        foreach (GameObject g in menuElements) { g.SetActive(false); }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) this.ClickOpenClose(null);
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

    public void ShowGameImage(GameObject curentButton)
    {
        gameImage.AddComponent<ImageEffectOpaque>
    }
}
