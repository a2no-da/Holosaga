using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveData
{

    public static void Save(Dictionary<string, int> data, string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName);
        bf.Serialize(file, data);
        file.Close();
    }

    public static Dictionary<string, int> Load(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            Dictionary<string, int> data = (Dictionary<string, int>)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new Dictionary<string, int>();
    }
}
