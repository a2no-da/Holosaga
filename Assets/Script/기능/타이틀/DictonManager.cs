using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class DictonManager : MonoBehaviour
{
    public Dicton[] dictons;
    public Dicton[] Mdictons;
    public ArtifactManager artifactManager;
    public static DictonManager instance;
    private static bool hasStarted = false;
    private PlayerData data;
    public Holomoney holomoney;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (holomoney != null)
        {
            data = holomoney.LoadPlayerData();
        }

        if (!hasStarted)
        {
            LoadAllDictons();
            AtiB();
            SaveAllDictons();
            hasStarted = true;
        }
        else
        {
            artifactManager.LoadArtifact();
            AtiB();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Collection" || scene.name == "Shop" || scene.name == "Stage1" || scene.name == "UnitSelect" || scene.name == "Infinite Stage" || scene.name == "Sandbag")
        {
            //SaveAllDictons();
            //LoadAllDictons();
        }

        if (scene.name == "Main" && hasStarted)
        {
            //SaveAllDictons();
            //LoadAllDictons();
        }

        if (!hasStarted)
        {
            LoadAllDictons();
            artifactManager.LoadArtifact();
            AtiB();
            SaveAllDictons();
            hasStarted = true;
        }
        else
        {
            artifactManager.LoadArtifact();
            AtiB();
        }
    }

    public void SaveAllDictons()
    {
        foreach (var dicton in dictons)
        {
            dicton.SaveAll();
        }

        foreach (var dicton in Mdictons)
        {
            dicton.SaveAll();
        }
    }

    public void LoadAllDictons()
    {
        foreach (var dicton in dictons)
        {
            dicton.LoadAll();
        }

        foreach (var dicton in Mdictons)
        {
            dicton.LoadAll();
        }
    }

    public void Reset()
    {
        foreach (var dicton in dictons)
        {
            dicton.Initialize();
        }

        foreach (var dicton in Mdictons)
        {
            dicton.InitializeM();
        }

        data.holomoney = 0;
        data.BoxCount = 0;
        holomoney.SavePlayerData(data);

        SaveAllDictons();
    }

    private void OnApplicationQuit()
    {
        SaveAllDictons();
        artifactManager.SaveArtifact();

        /*if (scene.name == "Stage1")
        {
            artifactManager.SaveArtifact();
        }*/
    }

    /*public void ResetAndRewardLevels()
    {
        int totalHolomoney = 0;
        PlayerData data = holomoney.LoadPlayerData();

        if (!data.Delta)
        {
            foreach (var dicton in dictons)
            {
                if (dicton.Level > 1)
                {
                    totalHolomoney += (dicton.Level - 1) * 100;
                    dicton.Level = 1;
                }
            }
        }

        data.Delta = true;
        data.holomoney += totalHolomoney;
        holomoney.SavePlayerData(data);
    }*/

    public void AtiB()
    {
        if (holomoney != null)
        {
            PlayerData playerData = holomoney.LoadPlayerData();
            if (!playerData.give)
            {
                if (holomoney != null)
                {
                    //playerData.ANormal += 200;
                    //playerData.AEpic += 200;
                    playerData.give = true;
                    holomoney.SavePlayerData(playerData);
                }
            }
        }

        foreach (var dicton in dictons)
        {
            foreach (var artifact in artifactManager.artifacts)
            {
                dicton.myartifact = null;
                if (artifact.artifactId == dicton.myArtifactId)
                {
                    dicton.myartifact = artifact;
                    break;
                }
            }
        }
    }
}
