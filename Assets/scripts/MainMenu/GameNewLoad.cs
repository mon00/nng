using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace game
{
    [SerializeField]
    public class GameNewLoad : MonoBehaviour
    {
        [Header("Inner Elements")]
        public Button[] GameButtons;
        public GameObject Display;
        public Button LoadButton, DeleteButton;

        [Header("Out Elements")]
        public GameObject camera;
        public GameObject NewGameWindow;
        public GameObject NewGameAnchor;

        private GameInfo CurrentGameInfo;
        private bool DelLoadActive = false;

        void Start()
        {
            DeleteButton.enabled = LoadButton.enabled = false;

            foreach (Button button in GameButtons)
            {
                GameInfo info = GameManager.Instance.LoadInfo(button.name);
                if (info.NewGame)
                {
                    button.GetComponentInChildren<Text>().text = "Start new Game";
                    button.onClick.AddListener(() => StartNewGame(info));
                }
                else
                {
                    button.GetComponentInChildren<Text>().text = info.Name;
                    button.onClick.AddListener(() => DisplayGame(info));
                }
            }
            
        }

        public void StartNewGame(GameInfo info)
        {
            camera.GetComponent<MenuChenger>().ChengeWindow(NewGameWindow);
            camera.GetComponent<MenuChenger>().ChengePlace(NewGameAnchor);
            GameManager.Instance.TmpGameInfo = info;
        }

        public void DisplayGame(GameInfo info)
        {
            if (DeleteButton.enabled == false || LoadButton.enabled == false)
            {
                DeleteButton.enabled = LoadButton.enabled = true;
                DeleteButton.onClick.AddListener(() => GameManager.Instance.DeleteGame(info));
                LoadButton.onClick.AddListener(() => GameManager.Instance.LoadGame(info));
            }
        }
    }
}