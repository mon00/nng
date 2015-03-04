using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{
    [SerializeField]
    public class LoadGame : MonoBehaviour
    {
        GameManager GM;

        public Transform LoadPrefab;
        public GameObject Display;
        public Transform LoadHolder;

        private List<string> Saves;
        private Dictionary<string, string> displayInfo = new Dictionary<string, string>();

        void Start()
        {
            if (LoadPrefab == null || Display == null || LoadHolder == null) Destroy(this);
            GM = GameObject.Find("GameManager").GetComponent<GameManager>();

            Saves = GM.LoadNames();

            int loadsCount = 0;

            foreach(string save in Saves)
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
            displayInfo = GM.LoadInfo(gameName);

            foreach (KeyValuePair<string, string> kvp in displayInfo)
            {
                Debug.Log(kvp.Key + " == " + kvp.Value);
            }
        }
    }
}