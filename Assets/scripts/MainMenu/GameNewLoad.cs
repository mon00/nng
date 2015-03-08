using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace game
{
    [SerializeField]
    public class GameNewLoad : MonoBehaviour
    {
        public Button StartGame1;
        public Button StartGame2;
        public Button StartGame3;

        public GameObject camera;
        public GameObject NewGameWindow;
        public GameObject NewGameAnchor;

        private GameInfo[] GameInfoArray = new GameInfo[3];

        void Start()
        {
            GameInfoArray[0] = GameManager.Instance.GameInfoHolder.One;
            GameInfoArray[1] = GameManager.Instance.GameInfoHolder.Two;
            GameInfoArray[2] = GameManager.Instance.GameInfoHolder.Three;

            if (GameInfoArray[0].NewGame)
            {
                StartGame1.GetComponentInChildren<Text>().text = "Start new Game";
                StartGame1.onClick.AddListener(()=>StartNewGame(0));
            }

            if (GameInfoArray[1].NewGame)
            {
                StartGame1.GetComponentInChildren<Text>().text = "Start new Game";
                StartGame1.onClick.AddListener(() => StartNewGame(1));
            }

            if (GameInfoArray[2].NewGame)
            {
                StartGame1.GetComponentInChildren<Text>().text = "Start new Game";
                StartGame1.onClick.AddListener(() => StartNewGame(2));
            }
        }

        public void StartNewGame(int pos)
        {
            switch (pos)
            {
                case 0:
                    GameManager.Instance.GameInfo = GameManager.Instance.GameInfoHolder.One;
                    break;
                case 1:
                    GameManager.Instance.GameInfo = GameManager.Instance.GameInfoHolder.Two;
                    break;
                case 2:
                    GameManager.Instance.GameInfo = GameManager.Instance.GameInfoHolder.Three;
                    break;
                default:
                    return;
            }

            camera.GetComponent<MenuChenger>().ChengePlace(NewGameAnchor);
            camera.GetComponent<MenuChenger>().ChengeWindow(NewGameWindow);
        }
    }
}