using UnityEngine;
using System.Collections;

namespace Common
{
    [SerializeField]
    public class PanelSwitcher : MonoBehaviour
    {
        public GameObject[] Panels;
        public GameObject ActivePanel;

        void Start()
        {
            foreach (GameObject panel in Panels)
            {
                if (panel == ActivePanel) panel.SetActive(true);
                else panel.SetActive(false);
            }
        }

        public void ChangePanel(GameObject NewPanel)
        {
            ActivePanel.SetActive(false);
            ActivePanel = NewPanel;
            ActivePanel.SetActive(true);
        }
    }
}