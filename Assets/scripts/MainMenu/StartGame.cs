using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGame : MonoBehaviour {

    private string name;

    void Start()
    {
        GameObject go = this.gameObject;
        name = ConfigManager.GetFromConfig(go.name);
        go.transform.FindChild("Text").GetComponent<Text>().text = name;

        go.GetComponent<Button>().onClick.AddListener(() => { OnPointerDown(); });
    }

    public void OnPointerDown()
    {
        if (SaveLoadManager.Check("save", name))
        {
            SaveLoadManager.Load("save", name);
        }
        else
        {
            //start new;
        }
    }
}
