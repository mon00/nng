using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lab70_GameManager;

namespace Common
{
    [SerializeField]
    public class DataLoader : MonoBehaviour
    {
        public GameObject TerrainGeneratorHolder;

        void Awake()
        {
            /*
            if (GameManager.Instance.GameInfo.NewGame)
            {
                TerrainGenerator TG = TerrainGeneratorHolder.GetComponent<TerrainGenerator>();
                List<GameObject> TerrainData = TG.Generate();
                if (TerrainData.Count == 0)
                {
                    Debug.LogError("Can`t create this map!");
                    GameManager.Instance.ChengeScene(Scene.Intro);
                    return;
                }
                GameManager.Instance.GameData.TerrainData = TerrainData;
                foreach (GameObject go in TerrainData)
                {
                    print(go.name);
                }
                GameManager.Instance.SaveGame(GameManager.Instance.GameInfo, GameManager.Instance.GameData);
                GameManager.Instance.GameInfo.NewGame = false;
            }
            else
            {
                List<GameObject> TerrainData = GameManager.Instance.GameData.TerrainData;
                foreach (GameObject go in TerrainData)
                {
                    print("Ну почти загрузило " + go.name);
                }
            }
             */
        }
    }
}