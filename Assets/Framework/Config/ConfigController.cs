using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lab70_Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Lab70_Framework.Config
{
    public class ConfigController : MonoBehaviour
	{
        public ConfigData Data;

        private string dir;
        private string file;

        public ConfigController(string directory, string configFile, bool resetOnStart = false)
        {
            dir = directory;
            file = configFile;
            
            if (resetOnStart) Reset();
            else Load();
        }

        public void Reset()
        {
            
        }

        public void Load()
        {
            if (!Directory.Exists(dir))
            {
                Reset();
                return;
            }

            string path = dir + file;
            
            if (!File.Exists(path))
            {
                Reset();
                return;
            }

            FileStream ConfigStream = new FileStream(path, FileMode.Open);

            try
            {
                BinaryFormatter ConfigFormatter = new BinaryFormatter();
                Data = (ConfigData)ConfigFormatter.Deserialize(ConfigStream);
            }
            catch (SerializationException e)
            {
                string error = "000001//ConfigController//SerializationError on Load Config: " + e + "//" + Time.time;
                //GameController.Instance.Error.Decrypt(error);
            }
            finally
            {
                ConfigStream.Close();
            }
        }

        public void Save(out string e)
        {
            e = "";
        }

	}
}
