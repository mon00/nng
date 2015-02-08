using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGameButton : MainMenu {

    private GameObject place;

    void Awake()
    {
        place = this.gameObject;
        place.GetComponentInChildren<Text>().text = "123";
        print(startGameButtonString);
    }
}
