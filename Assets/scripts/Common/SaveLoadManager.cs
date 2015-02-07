using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager : MonoBehaviour {
    
    private const string savesPath = "Data/Save/";
    private const string add = ".nng";
    private const string pathToConfig = "Data/Config.gnn";

    static public void Save(Dictionary<string, string> dic, string type, string fileName = null)
    {
        string path = getPath(type, fileName);
        if (path == null) return;

        FileStream fs = new FileStream(path, FileMode.Create);
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

    static public Dictionary<string,string> Load(string type, string fileName=null)
    {
        string path = getPath(type, fileName);
        if(!File.Exists(path)) return null;

        FileStream fs = new FileStream(path, FileMode.Open);
        Dictionary<string,string> dic = null;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            dic = (Dictionary<string,string>)formatter.Deserialize(fs);

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


    static public bool Check(string type, string fileName = null)
    {

        if(File.Exists(getPath(type, fileName))) return true;
        return false;
    }

   static private string getPath(string type, string fileName)
    {
        string path = null;

        if (type == "save") path = savesPath + fileName + add;
        else if (type == "config") path = pathToConfig;

        return path;
    }
}
