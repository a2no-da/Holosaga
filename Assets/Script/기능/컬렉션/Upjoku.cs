using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Upjoku : MonoBehaviour
{
    public GameObject subObject;
    public GameObject hawi;
    public TextMeshProUGUI coldasi;
    public AchievementManager achievementManager;

    void Start()
    {
        achievementManager = AchievementManager.Instance;
        Mais(achievementManager.achievements);
    }

    void Update()
    {
        int unlockedCount = CountUnlockedAchievements(achievementManager.achievements);
        coldasi.text = $"{unlockedCount} / {AchievementManager.Instance.achievements.Length}";
    }

    public void Mais(Achievement[] achievements)
    {
        foreach (var achievement in achievements)
        {
            GameObject newSubObject = Instantiate(subObject, hawi.transform);
            UpjokuSub upjokuSub = newSubObject.GetComponent<UpjokuSub>();
            if (upjokuSub != null)
            {
                upjokuSub.upho(achievement);
            }
        }
    }

    private int CountUnlockedAchievements(Achievement[] achievements)
    {
        int count = 0;
        foreach (var achievement in achievements)
        {
            if (achievement.unlocked) 
            {
                count++;
            }
        }
        return count;
    }
}
