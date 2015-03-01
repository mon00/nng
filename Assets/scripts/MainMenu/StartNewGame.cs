using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[SerializeField]
public class StartNewGame : MenuManager {

    [Header("Elements")]
    public Slider[] InfoSliders;
    public InputField PlayerNameField;
    public GameObject WarningMessage;

    private Dictionary<string, string> Info = new Dictionary<string, string>();
    private Dictionary<string, string> InfoBack;
    List<string> InfoSaved;
    private string PlayerName = "";

    public void Start()
    {
        foreach (Slider slider in InfoSliders)
        {
            Info.Add(slider.name, slider.value.ToString());
            Slider tmp = slider;
            slider.onValueChanged.AddListener(delegate { ChangeSliderValue(tmp); });
        }
        InfoBack = new Dictionary<string, string>(Info);
        InfoSaved = new List<string>(GM.Load().Keys);

        if (WarningMessage)
        {
            WarningMessage.SetActive(false);
        }
    }

    public void StartGame()
    {
        PlayerName = PlayerName.Replace(" ", string.Empty);
        if (PlayerName == "")
        {
            PlayerName = "Name can`t be empty";
            return;
        }

        GM.Save(PlayerName, Info);
        GM.Load(PlayerName);
    }

    public void Clear()
    {
        foreach (Slider slider in InfoSliders)
        {
            int value = int.Parse(InfoBack[slider.name].ToString());
            slider.value = value;
        }
        PlayerNameField.text = PlayerName = "";
        Info = new Dictionary<string, string>(InfoBack);
    }

    public void ChangeSliderValue(Slider slider)
    {
        if (!Info.ContainsKey(slider.name)) return;
        Info[slider.name] = slider.value.ToString();
        Debug.Log("Chenge value of " + slider.name + " to " + slider.value.ToString());
    }

    public void ChangeName()
    {
        string st = PlayerNameField.text;
        st.Replace(" ",string.Empty);
        PlayerName = PlayerNameField.text;

        if (InfoSaved.Contains(PlayerName) && WarningMessage)
        {
            WarningMessage.SetActive(true);
        }
        else if(!InfoSaved.Contains(PlayerName) && WarningMessage && WarningMessage.activeInHierarchy)
        {
            WarningMessage.SetActive(false);
        }
    }
}
