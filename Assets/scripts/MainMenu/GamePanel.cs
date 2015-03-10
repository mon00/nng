using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Lab70_GameManager;

namespace Common
{
    [SerializeField]
    public class GamePanel : MonoBehaviour
    {
        [Header("Inner Elements")]
        public Button[] GameButtons;
        public GameObject Display;
        public Info CurrentGameInfo { get; private set; }
        public Button DeleteButton, LoadButton;
        
        [Header("Panels")]
        public GameObject MainMenu;
        public GameObject NewGamePanel;

        private bool firstStart = true;
        private bool DelLoadActive = false;

        //private 

        void Start()
        {
            DeleteButton.interactable = LoadButton.interactable = false;

            foreach (Button button in GameButtons)
            {
                Info info = GameManager.Instance.LoadInfo(button.name);
                if (info.NewGame)
                {
                    button.GetComponentInChildren<Text>().text = "Start new game";
                    button.onClick.AddListener(() => StartNewGame(info));
                }
                else
                {
                    //button.GetComponentInChildren<Text>().text = info.Name;
                    //button.onClick.AddListener(() => DisplayGame(info));
                }
            }
        }

        public void StartNewGame(Info info)
        {
            CurrentGameInfo = info;
            MainMenu.GetComponent<PanelSwitcher>().ChangePanel(NewGamePanel);
        }

        public void DisplayGame(Info info)
        {
            DeleteButton.interactable = LoadButton.interactable = true;
        }
    }
}