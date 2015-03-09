using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace game
{
    public class NewGame : MonoBehaviour
    {

        private GameInfo info;

        void Start()
        {
            info = GameManager.Instance.TmpGameInfo;

        }

        public void UpdateName(InputField NameField)
        {
            info.Name = NameField.text;
        }

        public void StartGame()
        {
            if (info.Name == "") return;
            GameManager.Instance.SaveGame(info, new GameData());
            GameManager.Instance.LoadGame(info);
        }
    }
}