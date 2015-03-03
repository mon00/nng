using UnityEngine;
using System.Collections;

namespace game
{

    [SerializeField]
    public class MainMenu : MenuManager
    {

        public GameObject[] MainMenuElements;

        public void Start()
        {
            OnElementClick(null);

        }

        public void OnElementClick(GameObject TargetObject)
        {
            if (MainMenuElements.Length == 0) return;
            foreach (GameObject go in MainMenuElements)
            {
                if (go == TargetObject)
                {
                    bool Active;
                    Active = !go.activeInHierarchy;
                    go.SetActive(Active);
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }
    }

}
