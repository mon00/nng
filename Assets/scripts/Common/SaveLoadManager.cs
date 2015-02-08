using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager : MonoBehaviour {
   
    private const string generalPath = "Data/";

    public void SaveFile(Dictionary<string, string> dic, string path, string fileName, string add)
    {
        string file = path + fileName + add;

        FileStream fs = new FileStream(file, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            bf.Serialize(fs, dic);
        }
        catch (SerializationException e)
        {
            Debug.Log("Невозможно сохранить файл: " + file + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public Dictionary<string, string> LoadFile(string name, string path, string add)
    {
        string file = path + name + add;
        Dictionary<string, string> dic = null;
        if (File.Exists(file)) return dic;
 
        FileStream fs = new FileStream(file, FileMode.Open);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            dic = (Dictionary<string, string>)bf.Deserialize(fs);
        }
        catch
        {
            Debug.Log("Can`t load file: " + file);
            throw;
        }
        finally
        {
            fs.Close();
        }
        return dic;
    }
}
