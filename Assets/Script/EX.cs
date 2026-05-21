using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

[System.Serializable]
public class AchievementWrapper
{
    public List<AchievementData> items;
}

public class EX : MonoBehaviour
{
    public static AchievementManager Instance;
    private string saveFilePath;
    public Achievement[] achievements;

       private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "achievements.dat");
    }

    void Start()
    {
        var loadedAchievements = Resources.LoadAll<Achievement>("����");

        if (loadedAchievements == null || loadedAchievements.Length == 0)
        {
            return;
        }

        achievements = new List<Achievement>(loadedAchievements)
              .OrderBy(a =>
              {
                  string name = a.name;
                  int number;
                  string numberPart = name.TrimEnd('.');

                  if (int.TryParse(numberPart, out number))
                  {
                      return number; 
                  }
                  return int.MaxValue; 
              })
              .ToArray();

        LoadAchievements();
    }

    public void SaveAchievements()
    {
        List<AchievementData> achievementDataList = new List<AchievementData>();
        foreach (var achievement in achievements)
        {
            achievementDataList.Add(new AchievementData
            {
                num = achievement.num,
                unlocked = achievement.unlocked,
                updat = achievement.updat
            });
        }

       string json = JsonUtility.ToJson(achievementDataList);
       File.WriteAllText(saveFilePath, json);
    }

    public void LoadAchievements()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            AchievementWrapper wrapper = JsonUtility.FromJson<AchievementWrapper>(json);
            AchievementData[] loadedData = wrapper.items.ToArray();
            
            for (int i = 0; i < loadedData.Length; i++)
            {
                achievements[i].num = loadedData[i].num;
                var ach = new Steamworks.Data.Achievement("ACH_" + (i + 1));
                achievements[i].unlocked = loadedData[i].unlocked;
                if (achievements[i].unlocked)
                {
                    if (!ach.State)
                    {
                        ach.Trigger();
                    }
                }
                achievements[i].updat = loadedData[i].updat;
            }
        }
        else
        {
            CreateDefaultAchievements();
        }
    }

    private void CreateDefaultAchievements()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            achievements[i].num = i + 1;
            achievements[i].unlocked = false;
            achievements[i].updat = false;
        }

        SaveAchievements();
    }

    public void Reset()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            var ach = new Steamworks.Data.Achievement("ACH_" + (i + 1));
            ach.Clear();
        }
    }
}
