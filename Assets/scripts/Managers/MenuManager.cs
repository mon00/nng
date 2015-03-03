using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game
{

    public class MenuManager : MonoBehaviour
    {

        [HideInInspector]
        public GameManager GM;

        public void Awake()
        {
            GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

}