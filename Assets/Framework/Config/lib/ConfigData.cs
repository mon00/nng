using UnityEngine;
using System.Collections;

namespace Lab70_Framework.Config
{
    [System.Serializable]
    public class ConfigData : MonoBehaviour
    {
        public bool FullScreen = Screen.fullScreen;

        public int ScreenWidth = Screen.width;
        public int ScreenHeight = Screen.height;

        public float SoundLevel = 1f;
    }
}