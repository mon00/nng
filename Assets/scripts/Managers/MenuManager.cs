using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{

    public class MenuManager : MonoBehaviour
    {

        public Dictionary<string, string> GameInfo;
        public static List<string> GameNames;

        public void Awake()
        {
            GameNames = GameManager.Instance.LoadNames();
        }
    }

}