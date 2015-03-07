using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{
    [SerializeField]
    public class DataLoader : MonoBehaviour
    {
        public GameObject TerrainGeneratorHolder;
        
        void Awake()
        {
            TerrainGenerator TG = TerrainGeneratorHolder.GetComponent<TerrainGenerator>();
            print(TG.TryCount);
            List<GameObject> TerrainData = TG.Generate();
            if (TerrainData.Count == 0) Debug.LogError("Can`t create this map!");
        }
    }
}