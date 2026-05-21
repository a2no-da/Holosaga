using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class PlayerData
{
    public int holomoney;
    public bool Delta;
    public int BoxCount;
    public int ANormal;
    public int AEpic;
    public int AUnique;
    public int SellastNan;
    public bool give;
}


public class Holomoney : MonoBehaviour
{
    private string dataPath;

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "playerdata.dat");
    }

    public void SavePlayerData(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(dataPath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
    }

    public PlayerData LoadPlayerData()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(dataPath, FileMode.Open))
            {
                return (PlayerData)formatter.Deserialize(stream);
            }
        }

        return new PlayerData();
    }
}
