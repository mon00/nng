using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace game
{

    [SerializeField]
    public class StartNewGame : MenuManager
    {

        [Header("Elements")]
        public InputField PlayerNameField;
        public GameObject WM_NameAlreadyExists;
        public GameObject Main;
        public GameObject Additional;

        private Dictionary<string, string> Info = new Dictionary<string, string>();
        private Dictionary<string, string> InfoBack;

        private string PlayerName = "";
        private string infoParmName;
        private string infoParmValue;
        private ToggleGroup[] ToggleGroups;
        private bool activeAdditionals = false;

        public void Start()
        {
            ToggleGroups = GetComponentsInChildren<ToggleGroup>();

            foreach (ToggleGroup CurentToggleGroup in ToggleGroups)
            {
                infoParmName = CurentToggleGroup.name;

                Toggle[] Toggles = CurentToggleGroup.GetComponentsInChildren<Toggle>();
                foreach (Toggle toggle in Toggles)
                {
                    Toggle tmp = toggle;
                    toggle.onValueChanged.AddListener( delegate {ChangeToggleValue(tmp);});
                    if (toggle.isOn) infoParmValue = toggle.name;
                }

                Info.Add(infoParmName, infoParmValue);

                Debug.Log(infoParmName + " -- " + infoParmValue);
            }

            InfoBack = new Dictionary<string,string>(Info);
            
            if (WM_NameAlreadyExists)
            {
                WM_NameAlreadyExists.SetActive(false);
            }

            Additional.SetActive(activeAdditionals);
        }

        public void StartGame()
        {
            PlayerName = PlayerName.Replace(" ", string.Empty);
            if (PlayerName == "")
            {
                PlayerNameField.GetComponentInChildren<Text>().text = "Can`t be empty!";
                return;
            }

            GameManager.Instance.Save(PlayerName, Info);
            GameManager.Instance.LoadGame(PlayerName);
        }

        public void Clear()
        {
            foreach (ToggleGroup CurentToggleGroup in ToggleGroups)
            {
                infoParmName = CurentToggleGroup.name;

                Toggle[] Toggles = CurentToggleGroup.GetComponentsInChildren<Toggle>();
                foreach (Toggle toggle in Toggles)
                {
                    if (InfoBack[infoParmName] == toggle.name) toggle.isOn = true;
                }
            }

            PlayerNameField.text = PlayerName = PlayerNameField.GetComponentInChildren<Text>().text = "";
            Info = new Dictionary<string, string>(InfoBack);

            Debug.Log("Cleared!");
        }

        public void ChangeToggleValue (Toggle toggle)
        {
            if (!toggle.isOn) return;
            string infoParmName = toggle.transform.GetComponentInParent<ToggleGroup>().name;
            string infoParmValue = toggle.name;
            Info[infoParmName] = infoParmValue;
            Debug.Log(infoParmName + " changed to " + infoParmValue);
        }

        public void ChangeName()
        {
            string st = PlayerNameField.text;
            st.Replace(" ", string.Empty);
            PlayerName = PlayerNameField.text;

            if (GameNames.Contains(PlayerName) && WM_NameAlreadyExists)
            {
                WM_NameAlreadyExists.SetActive(true);
            }
            else if (!GameNames.Contains(PlayerName) && WM_NameAlreadyExists && WM_NameAlreadyExists.activeInHierarchy)
            {
                WM_NameAlreadyExists.SetActive(false);
            }
        }

        public void ActiveAddtionals()
        {
            activeAdditionals = !activeAdditionals;
            Additional.SetActive(activeAdditionals);
        }
    }

}