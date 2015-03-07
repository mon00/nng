using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace game
{

    public class TMP : MonoBehaviour
    {

        void Awake()
        {
            GameManager.Instance.Print();
        }

    }

}