using System;
using System.Collections.Generic; 
using System.IO;
using UnityEngine;
using System.Text;

[Serializable]
public class DictonData
{
    public string id;
    public string Name;
    public bool isPurchasable;
    public int exp;
    public int Holomo = 200;
    public int HolomoL = 50;
    public int MYexp;
    public int LevelSS = 1;
    public int Level = 1;
    public int Lexp;
    public int LMYexp;
    public Dicton.SkinData[] SkinList;
    public int HpUp;
    public int PowerUp;
    public int HpInUp;
    public int PowerInUp;
    public Artifact myartifact = null;
    public string myArtifactId;
}

[Serializable]
public class SkinData
{
    public string skinName;
    public bool isUnlocked;
    public bool isselected;
}

[Serializable]
public class DictonDataCollection
{
    public List<DictonData> dictonDatas = new List<DictonData>();
}

public class DictonManagers : MonoBehaviour
{
    private string dataPath;

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "dictondata.json");
    }

    public void SaveDictonData(DictonData data)
    {
        DictonDataCollection collection = LoadDictonData();
        int index = collection.dictonDatas.FindIndex(d => d.id == data.id);
        if (index != -1)
        {
            collection.dictonDatas[index] = data;
        }
        else
        {
            collection.dictonDatas.Add(data);
        }
        string jsonData = JsonUtility.ToJson(collection, true);
        //string encryptedData = EncryptionUtility.EncryptString(jsonData);
        File.WriteAllText(dataPath, jsonData, Encoding.UTF8);
    }

    public DictonDataCollection LoadDictonData() 
    {
        if (File.Exists(dataPath))
        {
            //string encryptedData = File.ReadAllText(dataPath);
            //string jsonData = EncryptionUtility.DecryptString(encryptedData);
            string jsonData = File.ReadAllText(dataPath, Encoding.UTF8);
            DictonDataCollection loadedDataCollection = JsonUtility.FromJson<DictonDataCollection>(jsonData);
            return loadedDataCollection; 
        }
        else
        {
            return new DictonDataCollection(); 
        }
    }
}