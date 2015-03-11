using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Lab70_GameManager;

namespace Common
{
    [SerializeField]
    public class NewGame : MonoBehaviour
    {
        public GameObject GamePanel;

        private Info info;

        void Start()
        {
            //info = GamePanel.GetComponent<GamePanel>().CurrentGameInfo;
        }

        public void UpdateName(InputField NameField)
        {
            info.Name = NameField.text;
        }

        public void StartGame()
        {
            if (info.Name == "") return;
            GameManager.Instance.Save(info, new Data());
            GameManager.Instance.Load(info);
        }
    }
}