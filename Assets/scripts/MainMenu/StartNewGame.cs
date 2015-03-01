using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[SerializeField]
public class StartNewGame : MenuManager {

    [Header("Elements")]
    public InputField PlayerNameField;
    public Slider WorldSizeSlider;
    public Slider WorldDarknessSlider;

    private string PlayerName;
    private float WorldSize;
    private float WorldDarkness;

    public void StartGame()
    {
        if (PlayerName == "") return;
        
        Dictionary<string, string> Info = new Dictionary<string, string>();
        Dictionary<string, string> Data = new Dictionary<string, string>();

        Info.Add("WorldSize", WorldSize.ToString());
        Info.Add("WorldDarkness", WorldDarkness.ToString());

        GM.Save(PlayerName, Info, Data);
        GM.Load(PlayerName);
    }

    public void Clear()
    {
        WorldDarkness = WorldDarknessSlider.value = 6f;
        WorldSize = WorldSizeSlider.value = 3f;
        PlayerName = PlayerNameField.text = "";
    }

    public void ChangeWorldSize()
    {
        WorldSize = WorldSizeSlider.value;
    }

    public void ChangeDarkness()
    {
        WorldDarkness = WorldDarknessSlider.value;
    }

    public void ChangeName()
    {
        string st = PlayerNameField.text;
        st.Replace(" ",string.Empty);
        PlayerName = st;
        //GM.AddDubugMessage("StartNewGame", "Name Changed to " + PlayerName);
    }
}
