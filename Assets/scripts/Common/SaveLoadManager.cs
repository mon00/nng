using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager : MonoBehaviour {
    [SerializeField]
    private static string savesPath = "Data/Save/";

    static public void Save(Dictionary<string, string> dic, string fileName)
    {
        FileStream fs = new FileStream(savesPath + fileName, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, dic);
        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    static public Dictionary<string,string> Load(string fileName)
    {
        if(!File.Exists(savesPath+fileName)) return null;
        FileStream fs = new FileStream(savesPath+fileName, FileMode.Open);
        Dictionary<string,string> dic = null;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            dic = (Dictionary<string,string>)formatter.Deserialize(fs);

            Debug.Log("From SaveLoadManager");
            foreach(KeyValuePair<string,string> kvp in dic)
            {
                Debug.Log(kvp.Key + " - " + kvp.Value);
            }
            Debug.Log("End of SaveLoadManager");

        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        return dic;
    }
    
}
