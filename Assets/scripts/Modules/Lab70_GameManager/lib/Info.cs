using System.Collections.Generic;

namespace Lab70_GameManager
{
    [System.Serializable]
    public class Info
    {
        public bool NewGame = true;

        public string Name = "";
        public string FileName;

        public int WorldSize = 2;
        public float WorldDispersion = 0.5f;

        public Dictionary<string, int> BiomInfo = new Dictionary<string, int>()
        {
            {"StartBiomSize", 2},

            {"SecondBiomSize", 2},
            {"SecondBiomCount", 2},

            {"ThirdBiomSize", 2},
            {"ThirdBiomCount", 2},

            {"FourthBiomSize", 2},
            {"FourthBiomCount", 2},

            {"FifthBiomSize", 2},
            {"FifthBiomCount", 2},
        };

        public Info(string fileName)
        {
            this.FileName = fileName;
        }
    }
}