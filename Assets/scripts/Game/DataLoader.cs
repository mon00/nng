using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lab70_GameManager;
using Lab70_WorldGenerator;
using Lab70_Framework;

namespace Common
{
    [SerializeField]
    public class DataLoader : MonoBehaviour
    {
        public GameObject TerrainGeneratorHolder;
        
        [Header("Prefabs")]
        public GameObject StartBiomPrefab;
        public GameObject SecondBiomPrefab;
        public GameObject ThirdBiomPrefab;
        public GameObject FourthBiomPrefab;
        public GameObject FifthBiomPrefab;

        [Header("Factors")]
        public int SizeFactor = 3;

        private Info info;

        void Awake()
        {
            info = GameManager.Instance.GameInfo;

            if (info.NewGame)
            {
                TerrainGenerator TG = TerrainGeneratorHolder.GetComponent<TerrainGenerator>();

                TG.Size = info.WorldSize*75;
                TG.Dispersion = info.WorldDispersion;
                TG.TryCount = 10000;

                List<Cell> CellTypes = new List<Cell>();

                Cell NewCell = new Cell("StartBiom", StartBiomPrefab, info.BiomInfo["StartBiomSize"] * SizeFactor, 2 * SizeFactor, 4, 1f);
                CellTypes.Add(NewCell);


                for (int i = 0; i < info.BiomInfo["SecondBiomCount"]; i++)
                {
                    Cell cell = new Cell("Second", SecondBiomPrefab, info.BiomInfo["SecondBiomSize"] * SizeFactor, 2 * SizeFactor, 3, 0.75f);
                    CellTypes.Add(cell);
                }

                for (int i = 0; i < info.BiomInfo["ThirdBiomCount"]; i++)
                {
                    Cell cell = new Cell("Third", SecondBiomPrefab, info.BiomInfo["ThirdBiomSize"] * SizeFactor, 2 * SizeFactor, 2, 0.6f);
                    CellTypes.Add(cell);
                }

                for (int i = 0; i < info.BiomInfo["FourthBiomCount"]; i++)
                {
                    Cell cell = new Cell("Fourth", SecondBiomPrefab, info.BiomInfo["FourthBiomSize"] * SizeFactor, 2 * SizeFactor, 2, 0.5f);
                    CellTypes.Add(cell);
                }

                for (int i = 0; i < info.BiomInfo["FifthBiomCount"]; i++)
                {
                    Cell cell = new Cell("Fifth", SecondBiomPrefab, info.BiomInfo["FifthBiomSize"] * SizeFactor, 2 * SizeFactor, 1, 0.4f);
                    CellTypes.Add(cell);
                }

                Cell[] CellTypesNew = new Cell[CellTypes.Count];
                CellTypes.CopyTo(CellTypesNew, 0);
                TG.CellTypes = CellTypesNew;

                List<GameObject> TerrainData = TG.Generate();
            }
            else
            {
                
            }
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