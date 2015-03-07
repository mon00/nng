using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{
    [SerializeField]
    public class LoadGame : MenuManager
    {
        public Transform LoadPrefab;
        public GameObject Display;
        public Transform LoadHolder;

        private Dictionary<string, string> DisplayInfo = new Dictionary<string, string>();

        void Start()
        {
            if (LoadPrefab == null || Display == null || LoadHolder == null) Destroy(this);

            int loadsCount = 0;

            foreach(string save in GameNames)
            {
                GameObject load = (GameObject)Instantiate(Resources.Load("load"));
                load.name = save;
                load.transform.SetParent(LoadHolder);
                load.transform.localPosition = new Vector3(0, 5 + 55 * loadsCount, 0);
                loadsCount++;
            }
        }

        public void DisplayGame(string gameName)
        {
            DisplayInfo = GameManager.Instance.LoadInfo(gameName);

            foreach (KeyValuePair<string, string> kvp in DisplayInfo)
            {
                Debug.Log(kvp.Key + " == " + kvp.Value);
            }
        }
    }
}