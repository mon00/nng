using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGame : MonoBehaviour {

    void Start()
    {
        GameObject go = this.gameObject;
        string name = ConfigManager.GetFromConfig(go.name);
        go.transform.FindChild("Text").GetComponent<Text>().text = name;

        go.GetComponent<Button>().onClick.AddListener(() => { OnPointerDown(); });
    }

    public void OnPointerDown()
    {
        print("123");
    }
}
