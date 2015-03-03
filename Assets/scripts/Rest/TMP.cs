using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class TMP : MonoBehaviour {

    [SerializeField]
    public Dictionary<string, string> TestD = new Dictionary<string,string>{{"Name", "Oeg"},{"Sername", "Timofeev"},};

    private string Path = "Data/";
    private string Name = "TMP";

    void Start()
    {
        Debug.Log("Start programm");
        OutDic(TestD);

        string file = Path + Name;

        FileStream fs = new FileStream(file, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(fs, TestD);

        fs.Close();

        Debug.Log("Dictionary writed!");

        FileStream fs2 = new FileStream(file, FileMode.Open);
        BinaryFormatter bf2 = new BinaryFormatter();
        Dictionary<string, string> NewD; 

        NewD =  (Dictionary<string, string>)bf2.Deserialize(fs2);

        Debug.Log("Exit dictionary is ");
        OutDic(NewD);
    }

    void OutDic(Dictionary<string, string> dic)
    {
        Debug.Log("Start Dictionary");
        foreach (KeyValuePair<string, string> kvp in dic)
        {
            Debug.Log("Element " + kvp.Key + " is " + kvp.Value);
        }
        Debug.Log("End Dictioary");
    }
}
