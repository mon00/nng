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
            
            int i = 0;
            foreach (Button button in GameButtons)
            {
                if (GameManager.Instance.GameInfoArray[i] == null)
                {
                    button.GetComponentInChildren<Text>().text = "Do not work!";
                }
                else
                {
                    GameInfo gameInfo = GameManager.Instance.GameInfoArray[i];
                    if (gameInfo.NewGame)
                    {
                        button.GetComponentInChildren<Text>().text = "Start new Game";
                        button.onClick.AddListener(() => StartNewGame(gameInfo));
                    }
                    else
                    {
                        button.GetComponentInChildren<Text>().text = gameInfo.Name;
                        button.onClick.AddListener(() => DisplayGame(gameInfo));
                    }
                }
                i++;
            }
        }

        public void StartNewGame(GameInfo info)
        {
            camera.GetComponent<MenuChenger>().ChengeWindow(NewGameWindow);
            camera.GetComponent<MenuChenger>().ChengePlace(NewGameAnchor);
            GameManager.Instance.CurrentGameInfo = info;
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