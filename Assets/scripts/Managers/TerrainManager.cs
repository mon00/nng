using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{

    public class TerrainManager : MonoBehaviour
    {

        public GameManager GM;
        public Dictionary<string, string> GameInfo;

        public void Awake()
        {
            GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            
            GameInfo = GM.GameInfo;
            
            if (GameInfo.Count < 1)
            {
                Debug.LogError("No Info Data!");
                GM.QuitApp();
            }

            foreach (KeyValuePair<string, string> kvp in GameInfo)
            {
                Debug.Log(kvp.Key + " -- " + kvp.Value);
            }
            print(GameInfo);
        }
    }
}
